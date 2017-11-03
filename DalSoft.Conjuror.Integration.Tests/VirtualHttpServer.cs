using System;
using System.Threading.Tasks;
using DalSoft.Conjuror.Extensions;
using DalSoft.Conjuror.Middleware;
using DalSoft.Conjuror.ServiceVirtualizations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DalSoft.Conjuror.Integration.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IApplicationBuilder HttpServiceVirtualizationMiddleware(IApplicationBuilder app)
            {
                return app.UseMiddleware<HttpServiceVirtualizationMiddleware>((Action<HttpServiceVirtualizations>)SetupVirtualization);
            }

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                
            var configurationRoot = builder.Build();
            
            new WebHostBuilder()
                .UseKestrel()
                .UseEnvironment("Development")
                .ConfigureServices(collection =>
                {
                    collection.AddSingleton((Func<IApplicationBuilder, IApplicationBuilder>) HttpServiceVirtualizationMiddleware);
                })
                .UseUrls("http://localhost:1001")
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        private static void SetupVirtualization(HttpServiceVirtualizations virtualization)
        {
            virtualization.UseFileServiceVirtualization("/virtual/responses/","application/json", "json");
            virtualization["/comms/sendsms"] = new HttpServiceVirtualization(httpContext => Task.FromResult(true));
        }
    }

    public class Startup //This would be in the project your testing or stand alone
    {
        private readonly IHostingEnvironment __environment;
        public IConfigurationRoot Configuration { get; private set; }

        public Startup(IHostingEnvironment environment)
        {
            __environment = environment;
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //Overwrite config if injected by test project
            if (services.BuildServiceProvider().GetService<IConfigurationRoot>() != null)
                Configuration = services.BuildServiceProvider().GetService<IConfigurationRoot>() ?? Configuration;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseInjectedApplicationBuilder();
        }
    }
}
