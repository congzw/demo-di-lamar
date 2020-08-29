using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace NbSites.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //step => 1
            var webHost = CreateWebHostBuilder(args).Build();

            using (var testScope = webHost.Services.CreateScope())
            {
                //step => 3
            }

            webHost.Run();


            //CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseLamar()
                .UseStartup<Startup>();
    }
}
