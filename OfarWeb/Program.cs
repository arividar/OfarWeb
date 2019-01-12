using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace OfarWeb
{
    class Program
    {
        static void Main(string[] args) 
        {
            Console.WriteLine("Hey there OfarWeb!");
            IWebHostBuilder whb1 = WebHost.CreateDefaultBuilder(args);
            IWebHostBuilder whb2 = whb1.UseStartup<Startup>();
            IWebHost wh = whb2.Build();
            wh.Run();
        }
    }
}
