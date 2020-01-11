using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebIncrementor.Models;

namespace WebIncrementor
{
    public class Startup
    {
        public static string ConnectionString { get; private set; }
        public static string DatabaseType { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            ConnectionString = Configuration["DatabaseConnection:Connection"];
            DatabaseType = Configuration["DatabaseConnection:Type"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<IncrementorDBContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 0;
            });

            services.AddDefaultIdentity<ApplicationUser>()
                .AddEntityFrameworkStores<IncrementorDBContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/v1/Error");
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseAuthentication();

            
            // Verify the create of the database
            using(var db = new IncrementorDBContext())
            {
                if (!db.Database.EnsureCreated())
                {
                    throw new InvalidOperationException("An error occured while creating the database. Please ensure that the Connection String and Database Type are valid.");
                }
            }
            
        }
    }
}
