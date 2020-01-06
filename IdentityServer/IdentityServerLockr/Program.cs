using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace IdentityServerLockr
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "IdentityServer";

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config)=> 
                {
                    config.AddJsonFile(
                    "appsettings.json", optional: false, reloadOnChange: true)                    
                    .AddEnvironmentVariables();
                })
                .UseStartup<Startup>();
        }
    }
}
