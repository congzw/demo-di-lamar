using System;
using System.Collections.Generic;
using System.Linq;
using Lamar;
using Lamar.IoC.Instances;
using Microsoft.Extensions.DependencyInjection;
using NbSites.Web.MultiTenancy;

namespace NbSites.Web.DI.Lamar
{
    public static class TenantSingletonExtensions
    {
        public static object GetTenantSingleton(this IServiceProvider provider, Type serviceType, string tenant)
        {
            var container = provider as IContainer;
            if (container == null)
            {
                throw new InvalidOperationException("无效的DI类型，此实现接受类型：" + typeof(IContainer).FullName);
            }
            var tenantKey = string.Empty;
            if (!string.IsNullOrWhiteSpace(tenant))
            {
                tenantKey = tenant.ToLower();
            }
            
            return container.GetInstance(serviceType, tenantKey);
        }
        
        public static IServiceCollection AddTenantSingletons(this IServiceCollection services, Type serviceType, Func<IServiceProvider, string, object> factory, IList<string> tenants)
        {
            var serviceRegistry = services as ServiceRegistry;
            if (serviceRegistry == null)
            {
                throw new InvalidOperationException("无效的DI类型，此实现接受类型：" + typeof(ServiceRegistry).FullName);
            }

            foreach (var tenant in tenants)
            {
                serviceRegistry.AddTenantSingleton(serviceType, provider => factory(provider, tenant), tenant);
            }

            //hack: resolve current by context
            serviceRegistry.For(serviceType).Use(new LambdaInstance(serviceType, provider =>
            {
                var tenantContext = provider.GetRequiredService<TenantContext>();
                return provider.GetTenantSingleton(serviceType, tenantContext.Tenant);
            }, ServiceLifetime.Transient));

            return services;
        }

        private static ServiceRegistry AddTenantSingleton(this ServiceRegistry serviceRegistry, Type serviceType, Func<IServiceProvider, object> factory, string tenant)
        {
            var tenantKey = string.Empty;
            if (!string.IsNullOrWhiteSpace(tenant))
            {
                tenantKey = tenant.ToLower();
            }
            serviceRegistry.For(serviceType).Use(new LambdaInstance(serviceType, factory, ServiceLifetime.Singleton).Named(tenantKey));
            return serviceRegistry;
        }

        #region for easy use
        
        public static IServiceCollection AddTenantSingletons<TService>(this IServiceCollection services, Func<IServiceProvider, string, object> factory, IList<string> tenants)
        {
            return services.AddTenantSingletons(typeof(TService), factory, tenants);
        }
        
        public static T GetTenantSingleton<T>(this IServiceProvider provider, string tenant)
        {
            return (T)provider.GetTenantSingleton(typeof(T), tenant);
        }

        #endregion
    }
}
