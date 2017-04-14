using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using CityInfo.API.Services;
using Microsoft.Extensions.Configuration;
using CityInfo.API.Entities;

namespace CityInfo.API
{
    public class Startup
    {
        public static IConfigurationRoot Configuration;

        //Use a json settings file to configure our environment.
        public Startup(IHostingEnvironment env)
        {
            //The second addJsonFile is used for a production environment. If there's a conflict between the two files,
            //The last file added wins.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            //Add our custom service. Transient is for lightweight, stateless services.
            //When dubugging, use the LocalMailService. When in production, use the CloudMailService
#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif

            //Set up the SQL Server connection
            var connectionString = Startup.Configuration["connectionStrings:cityInfoDBConnectionString"];
            services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString));

            //Add the DB context so we can inject it into our classes
            services.AddDbContext<CityInfoContext>();

            //If you want to return UpperCase property names instead of camelCase. For new applications,
            //you'll probably want camelCase (the .Net Core default), but if you're working with Old MVC apps, then you may want uppercase.
            //services.AddMvc()
            //   .AddJsonOptions(o => {
            //       if (o.SerializerSettings.ContractResolver != null)
            //       {
            //           var castedResolver = o.SerializerSettings.ContractResolver
            //               as DefaultContractResolver;
            //               castedResolver.NamingStrategy = null;
            //       }
            //   });


            //If you want to return XML instead of JSON when the consuming app specifies an XMLL "content-type"
            //services.AddMvc()
            //    .AddMvcOptions(o => o.OutputFormatters.Add(
            //        new XmlDataContractSerializerOutputFormatter()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            //Long way to add the nLogger extension to loggerFactory
            //loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());

            //Short-cut. Now we're also logging out to a file.
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            // Show Error pages when the consuming browser gets an error (e.g. instead of a silent 404 error)
            //app.UseStatusCodePages();
            app.UseMvc();
            
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
