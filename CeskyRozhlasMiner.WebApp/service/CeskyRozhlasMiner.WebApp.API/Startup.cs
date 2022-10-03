using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.DSX.ProjectTemplate.Command;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace Microsoft.DSX.ProjectTemplate.API
{
    /// <summary>
    /// Class that initializes our API.
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">Configuration of the web API.</param>
        /// <param name="environment">Hosting environment.</param>
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        /// <summary>
        /// This method gets called by the runtime and is used to add services to the DI container.
        /// </summary>
        /// <param name="services">Collection of services to be provided by DI.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbConnections(_configuration, _environment)
                .AddAutoMapperProfiles()
                .AddServices()
                .AddMediatR(typeof(HandlerBase))
                .AddCors()
                .AddSwaggerDocument()
                .AddCookiePolicy(options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.Secure = CookieSecurePolicy.Always;
                });

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, (options) =>
                    {
                        options.Cookie.Name = ".CeskyRozhlasMiner.Cookie";
                        options.Cookie.HttpOnly = true;
                        options.Cookie.IsEssential = true;
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                        
                    });

            services.AddHttpContextAccessor()
                .AddControllers();

        }

        /// <summary>
        /// This method gets called by the runtime and is used to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder.</param>
        public virtual void Configure(IApplicationBuilder app)
        {
            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app
                .UseExceptionHandling()
                .UseOpenApi()
                .UseSwaggerUi3()
                .UseCookiePolicy()
                .UseRouting()
                .UseCors("CorsPolicy")
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "../../client";
                spa.Options.DevServerPort = 3000;

                if (_environment.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer($"https://localhost:{spa.Options.DevServerPort}");
                    // spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
