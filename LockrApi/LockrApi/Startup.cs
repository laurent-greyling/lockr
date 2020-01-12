using IdentityServer4.AccessTokenValidation;
using LockrApi.Database;
using LockrApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LockrApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var clientId = Configuration.GetValue<string>("ClientId");
            var authority = Configuration.GetValue<string>("Authority");


            services.AddScoped<IDomainQueries, DomainQueries>();
            services.AddScoped<IDatabaseContext, DatabaseContext>();
            services.AddScoped<IVerifyDomain, VerifyDomain>();

            services.AddMvcCore()
                .AddAuthorization();
            services.AddControllers();
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = authority;
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "LockrApi";
                });

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
            //    options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
            //}).AddApiKeySupport(options => { });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
