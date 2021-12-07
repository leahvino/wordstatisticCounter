using DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;


namespace WordStatisticCounter
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
            string connectionString = Configuration.GetConnectionString("default");
            
            services.AddDbContext<wordStatisticCounterContext>(c => c.UseSqlServer(connectionString));
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.Use(async (context, next) =>
            {
                var contentType = context.Request.ContentType;


               
                if (contentType != null && contentType.Contains("x-www-form-urlencoded"))
                
                {
                    Dictionary<string, StringValues> formparameters = new Dictionary<string, StringValues>();

                    string s = String.Empty;
                    using (WebClient client = new WebClient())
                    {
                        s = client.DownloadString(context.Request.Form["Buffer"]);
                    }

                    formparameters.Add("Buffer", s?.ToString());// change to generic

                    context.Request.ContentType = "application/json";
                    context.Request.Form = new FormCollection(formparameters);             


                }
                await next();



            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
              endpoints.MapControllerRoute(
              name: "default",
              pattern: "api/{controller=Home}/{action=Index}/{id?}");              
            });



        }
    }
}
