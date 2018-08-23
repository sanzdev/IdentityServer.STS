using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Microsoft.AspNetCore.Hosting.WindowsServices;

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

            return WebHost.CreateDefaultBuilder(args)
              .UseContentRoot(pathToContentRoot)
              .UseStartup<Startup>()
              .UseApplicationInsights()
              .Build();
        }
    }
}
