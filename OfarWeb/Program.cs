﻿using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace OfarWeb
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hey there OfarWeb!");
            IWebHost wh = WebHost.CreateDefaultBuilder<Startup>(args).Build();
            wh.Run();
        }
    }
}
