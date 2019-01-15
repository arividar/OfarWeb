using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OfarWeb
{
    public class Program
    {
        internal class HostFilteringStartupFilter: IStartupFilter
        {
            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                return app =>
                {
                    app.UseHostFiltering();
                    next(app);
                };
            }
        }

        public static void Main()
        {
            var builder = new WebHostBuilder();

            builder.UseContentRoot(Directory.GetCurrentDirectory());

            builder.UseKestrel((builderContext, options) => { options.Configure(builderContext.Configuration.GetSection("Kestrel")); });

            builder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    if (env.IsDevelopment())
                    {
                        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        if (appAssembly != null)
                        {
                            config.AddUserSecrets(appAssembly, optional: true);
                        }
                    }
                    config.AddEnvironmentVariables();
                });

            builder.ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                });

            builder.ConfigureServices((hostingContext, services) =>
                {
                    // Fallback
                    services.PostConfigure<HostFilteringOptions>(options =>
                    {
                        if (options.AllowedHosts == null || options.AllowedHosts.Count == 0)
                        {
                            // "AllowedHosts": "localhost;127.0.0.1;[::1]"
                            var hosts = hostingContext.Configuration["AllowedHosts"]?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            // Fall back to "*" to disable.
                            options.AllowedHosts = (hosts?.Length > 0 ? hosts : new[] { "*" });
                        }
                    });

                    // Change notification
                    services.AddSingleton<IOptionsChangeTokenSource<HostFilteringOptions>>(
                        new ConfigurationChangeTokenSource<HostFilteringOptions>(hostingContext.Configuration));

                    services.AddTransient<IStartupFilter, HostFilteringStartupFilter>();
                });

            builder.UseIIS();

            builder.UseIISIntegration();

            builder.UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                });

            builder.UseStartup<Startup>();

            IWebHost wh = builder.Build();
            wh.Run();
        }
    }
}
