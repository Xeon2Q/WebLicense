using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebLicense.Access;
using WebLicense.Server.Auxiliary.Extensions;

namespace WebLicense.Server
{
    public class Program
    {
        #region Properties

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                                                              .SetBasePath(Directory.GetCurrentDirectory())
                                                              .AddJsonFile("appsettings.json", false, true)
                                                              .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
                                                              .Build();

        #endregion

        public static void Main(string[] args)
        {
            Log.Logger = CreateLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();

                try
                {
                    using var scope = host.Services.CreateScope();

                    scope.ServiceProvider.GetRequiredService<DatabaseContext>().Database.Migrate();
                }
                catch (Exception)
                {
                    // ignore
                }

                host.ApplyMigrations().Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, null);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .UseSerilog()
                       .ConfigureWebHostDefaults(ConfigureDefaults);
        }

        private static void ConfigureDefaults(IWebHostBuilder builder)
        {
            builder.UseStartup<Startup>();
        }

        private static ILogger CreateLogger()
        {
            var config = Configuration;

            return new LoggerConfiguration()
                   .ReadFrom.Configuration(config)
                   .CreateLogger();
        }
    }
}
