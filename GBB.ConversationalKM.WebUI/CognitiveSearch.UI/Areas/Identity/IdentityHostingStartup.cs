using System;
using CognitiveSearch.UI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(CognitiveSearch.UI.Areas.Identity.IdentityHostingStartup))]
namespace CognitiveSearch.UI.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<CognitiveSearchUIContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("CognitiveSearchUIContextConnection")));

                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<CognitiveSearchUIContext>();
            });
        }
    }
}