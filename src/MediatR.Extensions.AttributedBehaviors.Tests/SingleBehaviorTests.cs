using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.AttributedBehaviors.Tests
{
    public partial class SingleBehaviorTests
    {
        [MediatRBehavior(typeof(SingleBehavior))]
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

        public class SingleBehavior : IPipelineBehavior<Ping, Pong>
        {
            private readonly TestsLogger _output;

            public SingleBehavior(TestsLogger output)
            {
                _output = output;
            }

            public async Task<Pong> Handle(Ping request, CancellationToken cancellationToken, RequestHandlerDelegate<Pong> next)
            {
                _output.Messages.Add("Behavior before");
                var response = await next();
                _output.Messages.Add("Behavior after");

                return response;
            }
        }

        [Fact]
        public async Task Should_wrap_with_behavior()
        {
            var output = new TestsLogger();
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(output);
            var assembly = typeof(Ping).GetTypeInfo().Assembly;
            services.AddMediatR(assembly);
            services.AddMediatRAttributedBehaviors(assembly);

            var provider = services.BuildServiceProvider();

            var mediator = provider.GetRequiredService<IMediator>();

            var response = await mediator.Send(new Ping { Message = "Ping" });
            
            response.Message.ShouldBe("Ping Pong");

            output.Messages.ShouldBe(new[]
            {
                "Behavior before",
                "Handler",
                "Behavior after",
            });
        }
    }
}
