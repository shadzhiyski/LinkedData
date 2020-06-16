using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using LinkedData.Data.External;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LinkedData.RestService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) 
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("certificate.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"certificate.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                .Build();

            var certificateSettings = config.GetSection("certificateSettings");
            var certificateFileName = certificateSettings.GetValue<string>("filename");
            var certificatePassword = certificateSettings.GetValue<string>("password");

            //var certificate = new X509Certificate2(certificateFileName, certificatePassword);

            return new WebHostBuilder()//WebHost.CreateDefaultBuilder(args)
                .UseKestrel(
                    options =>
                    {
                        options.AddServerHeader = false;
                        options.Listen(IPAddress.Loopback, Startup.Port, listenOptions =>
                        {
                            listenOptions.UseHttps();
                        });
                    }
                )
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .UseUrls("https://localhost:" + Startup.Port);
        }
    }
}
