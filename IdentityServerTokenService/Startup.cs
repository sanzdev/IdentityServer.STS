using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using CorrelationId;
using IdentityServerTokenService.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServerTokenService
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }

        private readonly IHostingEnvironment _hostingEnvironment;

        public void ConfigureServices(IServiceCollection services)
        {
            var databaseConfiguration = Configuration.GetSection("DatabaseConfiguration");
            var connectionString = databaseConfiguration.GetValue<string>("ConnectionString");

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddCorrelationId();

            services.AddIdentityServer(Configuration.GetSection("IdentityServerOptions"))

                .AddOperationalStore(options =>
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)))

                .AddConfigurationStore(options =>
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)))

                .When(_hostingEnvironment.IsDevelopment, AddDeveloperSigningCredential)
                .When(_hostingEnvironment.IsProduction, AddSigningCredential);
        }

        private static IIdentityServerBuilder AddDeveloperSigningCredential(IIdentityServerBuilder builder) => builder.AddDeveloperSigningCredential();

        private IIdentityServerBuilder AddSigningCredential(IIdentityServerBuilder arg) => arg.AddSigningCredential(GetCertificateFromStore());

        private X509Certificate2 GetCertificateFromStore()
        {
            var certificateName = Configuration.GetValue<string>("IDSCertificateName");

            var x509Store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            x509Store.Open(OpenFlags.ReadOnly);
            var certificates = x509Store.Certificates.Find(X509FindType.FindBySubjectName, certificateName, false);

            if (certificates.Count < 1)
            {
                throw new FileNotFoundException($"Certificate file '{certificateName}' not found in the local store.");
            }
            return certificates[0];
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }

            UseForwardedHeaders(app);
            app.UseDeveloperExceptionPage();
            app.UseCorrelationId();
            app.UseIdentityServer();
        }

        private static void UseForwardedHeaders(IApplicationBuilder app)
        {
            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                RequireHeaderSymmetry = false,
                ForwardedHeaders = ForwardedHeaders.All,
            };

            forwardedHeadersOptions.KnownNetworks.Clear();
            forwardedHeadersOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(forwardedHeadersOptions);
        }
    }
}
