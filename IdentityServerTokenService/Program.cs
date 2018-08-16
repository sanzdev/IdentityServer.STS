using System.Diagnostics;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace IdentityServerTokenService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            if (Debugger.IsAttached || args.Contains("--debug"))
            {
                host.Run();
            }
            else
            {
                host.Run();
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", false)
              .Build();

            var hostOptions = config.GetSection("HostOptions");

            return WebHost.CreateDefaultBuilder(args)
              .UseIISIntegration()
              .UseStartup<Startup>()
              .UseApplicationInsights()
              .Build();
        }
    }
}
