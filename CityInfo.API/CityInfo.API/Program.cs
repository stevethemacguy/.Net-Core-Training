using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace CityInfo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Needed to deploy to heroku. See http://codersblock.com/blog/how-to-run-net-on-heroku/
            var config = new ConfigurationBuilder().AddCommandLine(args).Build();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(config)   //Needed to deploy to heroku. See http://codersblock.com/blog/how-to-run-net-on-heroku/
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
