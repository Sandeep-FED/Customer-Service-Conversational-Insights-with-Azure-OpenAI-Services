﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CognitiveSearch.UI.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace CognitiveSearch.UI
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddMicrosoftIdentityWebAppAuthentication(Configuration);
            services.AddMvc(option =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                option.Filters.Add(new AuthorizeFilter(policy));
            }).AddMicrosoftIdentityUI();

            var apiConfig = new ApiConfig
            {
                Url = "/api"
            };
            services.AddSingleton(apiConfig);

            var orgConfig = new OrganizationConfig
            {
                Name = Configuration["OrganizationName"],
            };
            services.AddSingleton(orgConfig);

            var appConfig = new AppConfig
            {
                ApiConfig = apiConfig,
                Organization = orgConfig,
                Customizable = Configuration["Customizable"].Equals("true", StringComparison.InvariantCultureIgnoreCase),
                UploadDisclaimer = Configuration["UploadDisclaimer"] != null ? Configuration["UploadDisclaimer"] : ""
            };
            services.AddSingleton(appConfig);

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddMvc(options => options.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseMvcWithDefaultRoute();
        }
    }
}
