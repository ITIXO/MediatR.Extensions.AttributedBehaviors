using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MediatR.Extensions.AttributedBehaviors
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MediatRBehaviorAttribute : Attribute
    {
        public Type BehaviorType { get; }
        public Type InterfaceType { get; }
        public ServiceLifetime ServiceLifetime { get; }
        public int Order { get; }

        public MediatRBehaviorAttribute(Type behaviorType, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, [CallerLineNumber] int order = 0)
        {
            BehaviorType = behaviorType;
            ServiceLifetime = serviceLifetime;
            Order = order;
            InterfaceType = behaviorType.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));
        }
    }
}
