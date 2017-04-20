using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace CityInfo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();

            //Instead of using IIS, you can also use a direct URL. This can be used to connect to the API from your mac:
            //Optional. Use this VM's IP instead of localhost. For example: 192.168.173.201:5000/api/cities. 
            //WARNING: UseIISIntegration() will override UseUrls. If you want to test on your mac, you MUST remove the UseIISIntegration Line!
            //Also, when debugging, you should select CityInfo.API instead of IISExpress.
            //.UseUrls("http://192.168.173.201:5000/")
            ////.UseIISIntegration()
        }
    }
}
