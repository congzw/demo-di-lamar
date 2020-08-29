using System;
using System.Collections.Generic;
using Lamar;
using Lamar.Scanning.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NbSites.Libs.Services;
using NbSites.Web.DI.Lamar;
using NbSites.Web.MultiTenancy;

namespace NbSites.Web
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        private readonly IHostingEnvironment _env;

        public Startup(ILogger<Startup> logger, IHostingEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddTransient<EmptyService>();
        //    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        //}

        public void ConfigureContainer(ServiceRegistry services)
        {
            //step => 2
            services.AddScoped<ITenantContextService, TenantContextService>();
            services.AddScoped(sp => sp.GetService<ITenantContextService>().GetCurrentTenantContext());

            var tenantKeyHold = TenantKeyHold.Build("A", "b");
            services.AddSingleton(tenantKeyHold);

            services.AddTenantSingletons<TenantConfig>((provider, tenant) =>
            {
                var tenantConfig = new TenantConfig()
                {
                    TenantKey = tenant,
                    TenantName = string.IsNullOrWhiteSpace(tenant) ? "租户:默认" : "租户:" + tenant
                };
                return tenantConfig;
            }, tenantKeyHold.TenantKeys);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Also exposes Lamar specific registrations
            // and functionality
            services.Scan(s =>
            {
                s.TheCallingAssembly();
                s.Assembly(typeof(BarService).Assembly);
                s.WithDefaultConventions(ServiceLifetime.Scoped);
                s.WithDefaultConventions(OverwriteBehavior.Never);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            //step => 4
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            MyTempHold.RootProvider = app.ApplicationServices;

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}");
            });

        }
    }

    public class MyTempHold
    {
        public static IServiceProvider RootProvider = null;
    }
}
