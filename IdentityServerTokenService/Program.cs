using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;

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
                host.RunAsService();
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            var pathToContentRoot = Path.GetDirectoryName(pathToExe);

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            var hostOptions = config.GetSection("HostOptions");

            return WebHost.CreateDefaultBuilder(args)
              .UseUrls($"{hostOptions.GetValue<string>("Url")}:{hostOptions.GetValue<string>("Port")}")
              .UseContentRoot(pathToContentRoot)
              .UseStartup<Startup>()
              .UseApplicationInsights()
              .Build();
        }
    }
}
