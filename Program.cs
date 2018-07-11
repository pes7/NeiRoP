using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace NeiRoP
{
    class Program
    {
        Random rand = new Random();
        static void Main(string[] args)
        {
            Thread main = new Thread(() =>
            {
                var m = new Main();
                m.ShowDialog();
            });
            main.Start();
        }
    }
}
