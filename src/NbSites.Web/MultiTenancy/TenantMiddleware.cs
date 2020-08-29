//using System;
//using System.Collections.Concurrent;
//using System.Threading.Tasks;
//using Lamar;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;

//namespace NbSites.Web.MultiTenancy
//{
//    public class TenantMiddleware
//    {
//        private readonly RequestDelegate next;
//        public TenantMiddleware(RequestDelegate next)
//        {
//            this.next = next;
//        }

//        public async Task Invoke(HttpContext context, ILogger<TenantMiddleware> logger)
//        {
//            var tenantContext = context.RequestServices.GetService<TenantContext>();
//            if (tenantContext == null)
//            {
//                await next.Invoke(context);
//                return;
//            }

//            var tenant = string.IsNullOrWhiteSpace(tenantContext.Tenant) ? string.Empty : tenantContext.Tenant;
//            var tenantContainerCache = context.RequestServices.GetService<TenantContainerCache>();
//            if (!tenantContainerCache.Containers.TryGetValue(tenant, out var tenantContainer))
//            {
//                var tenantContainerBuilder = context.RequestServices.GetService<ITenantContainerBuilder>();
//                var serviceRegistry = context.RequestServices.GetService<ServiceRegistry>();
//                tenantContainer = await tenantContainerBuilder.BuildAsync(tenantContext, serviceRegistry);
//                tenantContainerCache.Containers.TryAdd(tenant, tenantContainer);
//            }

//            using (var requestContainer = tenantContainer.GetNestedContainer())
//            {
//                //context.RequestServices = requestContainer.GetInstance<IServiceProvider>();

//                var tenantConfig = requestContainer.GetService<TenantConfig>();
//                logger.LogDebug(">>>  " + tenantConfig.TenantName + ", " + tenantConfig.Id);
//                logger.LogDebug(">>> nested container in " + tenantContext.Tenant);
//                await next.Invoke(context);
//            }
//        }
//    }

//    public interface ITenantContainerBuilder
//    {
//        Task<IContainer> BuildAsync(TenantContext tenant, ServiceRegistry service);
//    }

//    public class TenantContainerBuilder : ITenantContainerBuilder
//    {
//        public async Task<IContainer> BuildAsync(TenantContext tenant, ServiceRegistry service)
//        {
//            var container = await Container.BuildAsync(service);
//            container.Configure(service);
//            container.Configure(config =>
//            {
//                if (tenant.Tenant == "")
//                {
//                    config.AddScoped<IHelloService, HelloService>();
//                }
//                else
//                {
//                    config.AddScoped<IHelloService, Hello2Service>();
//                }

//                config.AddSingleton(sp =>
//                {
//                    var tenantContext = sp.GetService<TenantContext>();
//                    return new TenantConfig { TenantName = tenantContext.Tenant };
//                });
//            });

//            return container;
//        }
//    }

//    public class TenantContainerCache
//    {
//        public ConcurrentDictionary<string, IContainer> Containers { get; set; } = new ConcurrentDictionary<string, IContainer>(StringComparer.OrdinalIgnoreCase);
//    }
//    public interface IHelloService
//    {
//        Guid Id { get; set; }
//        string Hello();
//    }
//    public class HelloService : IHelloService
//    {
//        public Guid Id { get; set; } = Guid.NewGuid();

//        public string Hello()
//        {
//            return "Hello";
//        }
//    }
//    public class Hello2Service : IHelloService
//    {
//        public Guid Id { get; set; } = Guid.NewGuid();
//        public string Hello()
//        {
//            return "Hello2";
//        }
//    }
//}
