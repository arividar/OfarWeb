using System;
using Microsoft.AspNetCore;

namespace OfarWeb
{
    class Program
    {
        static void Main(string[] args) 
        {
             Console.WriteLine("Hey there OfarWeb!");
             WebHost.CreateDefaultBuilder(args);
        }
    }
}
