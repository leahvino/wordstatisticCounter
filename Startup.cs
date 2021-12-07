using DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Model.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
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


            //app.Use(async (context, next) =>
            //{
            //    var contentType = context.Request.ContentType;
                               
            //    if (contentType != null && contentType.Contains("x-www-form-urlencoded"))
                
            //    {
            //        //var formData = await context.Request.Form
            //        //context.Request.Body = new FormUrlEncodedContent(Correct(formData));
            //        Dictionary<string, StringValues> formparameters = new Dictionary<string, StringValues>();

            //        string s = String.Empty;
            //        using (WebClient client = new WebClient())
            //        {
            //            s = client.DownloadString(context.Request.Form["Buffer"]);
            //        }

            //        formparameters.Add("Buffer", s?.ToString());// change to generic

            //        context.Request.ContentType = "application/json";
            //        var json = JsonConvert.SerializeObject(formparameters);
            //        //modified stream
            //        context.Request.Form = new FormCollection(formparameters);
            //        var requestData = Encoding.UTF8.GetBytes(json);
            //        var stream = new MemoryStream(requestData);
            //        context.Request.Body = stream;
            //        await next.Invoke();


            //    }
            //    await next();



            //});

            app.UseHttpsRedirection();

            app.UseMiddleware<CreateSession>();

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


    public class CreateSession
    {
        private readonly RequestDelegate _next;

        public CreateSession(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            var request = context.Request;
            if (request.Path.Value.Contains("SaveWordCounter"))
            {
                var contentType = context.Request.ContentType;

                if (contentType != null && contentType.Contains("x-www-form-urlencoded"))
                {
                    //get the request body and put it back for the downstream items to read
                    var stream = request.Body;// currently holds the original stream                    
                    var originalContent = await new StreamReader(stream).ReadToEndAsync();
                    var bufferUrlEncode = originalContent.Substring(originalContent.IndexOf("=") + 1);//.Replace("/", "%20"); ;
                    //var bufferUrl = new UriBuilder(bufferUrlEncode);
                    var bufferUrl = Uri.UnescapeDataString(bufferUrlEncode);

                    try
                    {
                        var bufferContent = string.Empty;
                        using (WebClient client = new WebClient())
                        {
                            bufferContent = client.DownloadString(bufferUrl);
                        }

                        //if (dataSource != null/* && dataSource.Take > 2000*/)
                        //{
                        //dataSource.Take = 2000;

                        WordcounterRequest stri = new WordcounterRequest
                        {
                            Buffer = bufferContent
                        };

                        var json = JsonConvert.SerializeObject(stri);
                        //replace request stream to downstream handlers

                        var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                            stream = await requestContent.ReadAsStreamAsync();//modified stream
                        context.Request.ContentType = "application/json";
                        //}
                    }
                    catch (Exception ex )
                    {
                        throw;
                    }                 

                    request.Body = stream;
                }
                await _next.Invoke(context);
            }
            else
            {
                await _next.Invoke(context);

            }



        }
    }
}
