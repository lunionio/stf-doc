using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Doc.Service.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Doc.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {

            MainAsync().Wait();

        }
        static async Task MainAsync()
        {
            var url = await AuxNotStatic.GetInfoMotorAux("wpDocumento", 1);
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls(url.Url)
                //.UseUrls("http://localhost:5000")
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
