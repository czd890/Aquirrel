using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
namespace Aquirrel.Logger.File.Test
{

    public class Program
    {
        static Startup startup;
        static IServiceProvider sp;
        internal static int C = 10;
        public static void Main(string[] args)
        {


            startup = new Startup(null);
            sp = startup.ConfigureServices(new ServiceCollection());
            Console.Read();

        }
    }
}
