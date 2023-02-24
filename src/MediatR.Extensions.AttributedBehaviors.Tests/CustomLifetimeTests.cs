using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.AttributedBehaviors.Tests
{
    public class CustomLifetimeTests
    {
        [MediatRBehavior(typeof(TransientBehavior), ServiceLifetime.Transient)]
        [MediatRBehavior(typeof(ScopedBehavior), ServiceLifetime.Scoped)]
        [MediatRBehavior(typeof(SingletonBehavior), ServiceLifetime.Singleton)]
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
            public Task<Pong> Handle(Ping message, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Pong { Message = message.Message + " Pong" });
            }
        }

        public class TransientBehavior : IPipelineBehavior<Ping, Pong>
        {
            public Task<Pong> Handle(Ping request, RequestHandlerDelegate<Pong> next, CancellationToken cancellationToken)
            {
                return next();
            }
        }

        public class SingletonBehavior : IPipelineBehavior<Ping, Pong>
        {
            public Task<Pong> Handle(Ping request, RequestHandlerDelegate<Pong> next, CancellationToken cancellationToken)
            {
                return next();
            }
        }
        
        public class ScopedBehavior : IPipelineBehavior<Ping, Pong>
        {
            public Task<Pong> Handle(Ping request, RequestHandlerDelegate<Pong> next, CancellationToken cancellationToken)
            {
                return next();
            }
        }

        [Fact]
        public async Task Should_be_singleton()
        {
            (IServiceCollection services, IMediator mediator) = Setup();

            var response = await mediator.Send(new Ping { Message = "Ping" });
            
            response.Message.ShouldBe("Ping Pong");

            services.Where(x => x.ServiceType == typeof(SingletonBehavior)).ShouldAllBe(s => s.Lifetime == ServiceLifetime.Singleton);
        }

        [Fact]
        public async Task Should_be_transient()
        {
            (IServiceCollection services, IMediator mediator) = Setup();

            var response = await mediator.Send(new Ping { Message = "Ping" });
            
            response.Message.ShouldBe("Ping Pong");

            services.Where(x => x.ServiceType == typeof(TransientBehavior)).ShouldAllBe(s => s.Lifetime == ServiceLifetime.Transient);
        }

        [Fact]
        public async Task Should_be_scoped()
        {
            (IServiceCollection services, IMediator mediator) = Setup();

            var response = await mediator.Send(new Ping { Message = "Ping" });
            
            response.Message.ShouldBe("Ping Pong");

            services.Where(x => x.ServiceType == typeof(ScopedBehavior)).ShouldAllBe(s => s.Lifetime == ServiceLifetime.Scoped);
        }

        private (IServiceCollection services, IMediator mediator) Setup()
        {
            IServiceCollection services = new ServiceCollection();
            var assembly = typeof(Ping).GetTypeInfo().Assembly;
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            services.AddMediatRAttributedBehaviors(assembly);

            var provider = services.BuildServiceProvider();

            var mediator = provider.GetRequiredService<IMediator>();

            return (services, mediator);
        }
    }
}
