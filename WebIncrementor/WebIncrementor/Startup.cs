using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            
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
