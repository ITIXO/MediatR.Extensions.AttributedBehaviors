using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.AttributedBehaviors.Tests
{
    public class CustomOrderingTests
    {
        [MediatRBehavior(typeof(SecondBehavior), order: 1)]
        [MediatRBehavior(typeof(FirstBehavior), order: 0)]
        public class Ping : IRequest<Pong>
        {
            public string Message { get; set; } = "";
        }

        public class Pong
        {
            public string Message { get; set; } = "";
        }

        public class PingHandler : IRequestHandler<Ping, Pong>
        {
            private readonly TestsLogger _logger;

            public PingHandler(TestsLogger logger)
            {
                _logger = logger;
            }
            public Task<Pong> Handle(Ping message, CancellationToken cancellationToken)
            {
                _logger.Messages.Add("Handler");

                return Task.FromResult(new Pong { Message = message.Message + " Pong" });
            }
        }
        public class FirstBehavior : IPipelineBehavior<Ping, Pong>
        {
            private readonly TestsLogger _output;

            public FirstBehavior(TestsLogger output)
            {
                _output = output;
            }

            public async Task<Pong> Handle(Ping request, RequestHandlerDelegate<Pong> next, CancellationToken cancellationToken)
            {
                _output.Messages.Add("First before");
                var response = await next();
                _output.Messages.Add("First after");

                return response;
            }
        }

        public class SecondBehavior : IPipelineBehavior<Ping, Pong>
        {
            private readonly TestsLogger _output;

            public SecondBehavior(TestsLogger output)
            {
                _output = output;
            }

            public async Task<Pong> Handle(Ping request, RequestHandlerDelegate<Pong> next, CancellationToken cancellationToken)
            {
                _output.Messages.Add("Second before");
                var response = await next();
                _output.Messages.Add("Second after");

                return response;
            }
        }

        [Fact]
        public async Task Should_have_correct_order()
        {
            var output = new TestsLogger();
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(output);
            var assembly = typeof(Ping).GetTypeInfo().Assembly;
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            services.AddMediatRAttributedBehaviors(assembly);

            var provider = services.BuildServiceProvider();

            var mediator = provider.GetRequiredService<IMediator>();

            var response = await mediator.Send(new Ping { Message = "Ping" });

            response.Message.ShouldBe("Ping Pong");

            output.Messages.ShouldBe(new[]
            {
                "First before",
                "Second before",
                "Handler",
                "Second after",
                "First after"
            });
        }
    }
}
