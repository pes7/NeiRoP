using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Cudafy;
using System.Diagnostics;

namespace ImageConvisor
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread main = new Thread(() =>
            {
                var _main = new Main();
                _main.ShowDialog();
            });
            main.Start();

            //test();
        }

        [Cudafy]
        public static void test()
        {
            long middle = 0;
            for (int i = 0; i < 100; i++)
            {
                long f = Stopwatch.GetTimestamp();
                cudefyTest();
                long s = Stopwatch.GetTimestamp() - f;
                if (i > 0)
                {
                    Console.WriteLine($"{s} secs.");
                    middle += s;
                }
            }
            Console.WriteLine($"middle: {middle/100}");
        }

        [Cudafy]
        public static void  cudefyTest()
        {
            double i = 0;
            for(int j = 0; j < 100000; j++)
            {
                i += Math.PI * j;
            }
        }
    }
}
