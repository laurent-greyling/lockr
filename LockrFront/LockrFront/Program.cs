using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LockrFront
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((context, config)=> 
                    {
                        var root = config.Build();
                        config.AddAzureKeyVault(
                            $"https://{root["KeyVault"]}.vault.azure.net/",
                            root["ClientId"],
                            root["ClientSecret"]
                            );
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
