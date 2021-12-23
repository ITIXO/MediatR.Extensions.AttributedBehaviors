using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MediatR.Extensions.AttributedBehaviors
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddMediatRAttributedBehaviors(this IServiceCollection services, Assembly assembly)
            => services.AddMediatRAttributedBehaviors(new[] { assembly });

        public static IServiceCollection AddMediatRAttributedBehaviors(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            var queriesWithAttributes = assemblies.Distinct().SelectMany(a => a.DefinedTypes)
                                                  .Where(ti => (ti.ImplementedInterfaces.Contains(typeof(IRequest)) ||
                                                                ti.IsAssignableToGenericType(typeof(IRequest<>))) &&
                                                                Attribute.IsDefined(ti, typeof(MediatRBehaviorAttribute)));

            foreach (var query in queriesWithAttributes)
            {
                foreach (var attr in query.GetCustomAttributes<MediatRBehaviorAttribute>(false).OrderBy(attr => attr.Order))
                {
                    services.Add(new ServiceDescriptor(attr.InterfaceType, attr.BehaviorType, attr.ServiceLifetime));
                }
            }

            return services;
        }

        private static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            if (givenType.GetInterfaces().Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
            {
                return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            var baseType = givenType.BaseType;
            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }
    }
}
