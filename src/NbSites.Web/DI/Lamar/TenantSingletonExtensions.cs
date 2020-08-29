using System;
using System.Collections.Generic;
using System.Linq;
using Lamar;
using Lamar.IoC.Instances;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            
            var notEmptyTenants = tenants.Where(x => !string.IsNullOrWhiteSpace(x));
            foreach (var tenant in notEmptyTenants)
            {
                var tenantCopy = tenant;
                services.AddTenantSingleton(serviceType, provider => factory(provider, tenantCopy), tenantCopy);
            }

            //hack default resolve => use same tenant with ""
            services.AddTenantSingleton(serviceType, provider => factory(provider, string.Empty), string.Empty);

            return services;
        }

        private static IServiceCollection AddTenantSingleton(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> factory, string tenant)
        {
            var serviceRegistry = services as ServiceRegistry;
            if (serviceRegistry == null)
            {
                throw new InvalidOperationException("无效的DI类型，此实现接受类型：" + typeof(ServiceRegistry).FullName);
            }

            var tenantKey = string.Empty;
            if (!string.IsNullOrWhiteSpace(tenant))
            {
                tenantKey = tenant.ToLower();
            }

            //特殊处理EMPTY
            if (string.IsNullOrWhiteSpace(tenant))
            {
                serviceRegistry.For(serviceType).Use(new LambdaInstance(serviceType, factory, ServiceLifetime.Singleton).Named(tenantKey));
                serviceRegistry.For(serviceType).Use(new LambdaInstance(serviceType, provider =>
                {
                    var tenantContext = provider.GetRequiredService<TenantContext>();
                    return provider.GetTenantSingleton(serviceType, tenantContext.Tenant);
                    }, ServiceLifetime.Transient));
            }
            else
            {
                serviceRegistry.For(serviceType).Use(new LambdaInstance(serviceType, factory, ServiceLifetime.Singleton).Named(tenantKey));
            }
            return services;
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

        public static T GetCurrentTenantSingleton<T>(this IServiceProvider provider)
        {
            var tenant = string.Empty;
            var context = provider.GetRequiredService<TenantContext>();
            if (!string.IsNullOrWhiteSpace(context.Tenant))
            {
                tenant = context.Tenant;
            }
            return (T)provider.GetTenantSingleton(typeof(T), tenant);
        }

        #endregion
    }
}
