using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace NeiRoP
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Matrix<double> mat1 = getFromPhoto("test1.png"),
                           mat2 = getFromPhoto("test2.png"),
                           mat3 = getFromPhoto("test3.png"),

                           notCorrect = getFromPhoto("example.png");

            var _mat1 = mat1.Transpose() * mat1;
            var _mat2 = mat2.Transpose() * mat2;
            var _mat3 = mat3.Transpose() * mat3;
            var sum = _mat1 + _mat2 + _mat3;
            for (int i = 0; i < 3; i++)
            {
                sum[i, i] = 0;
            } // не меняеться w[sum] * y[notCorrect.Transpose()]

            var test1 = sum * notCorrect.Transpose();
            //Console.WriteLine("Start: " + test1.ToString());

            var hock = notCorrect.Transpose();
            for (int i = 0; i < 1000; i++)
            {
                hock = sum * hock;
                hock = getNormalized(hock);
            }

            Bitmap bit = new Bitmap(Bitmap.FromFile("example.png"));
            for (int i = 0, j = 0; i < bit.Width * bit.Height; i++, j = j < bit.Height ? j++ : 0)
            {
                bit.SetPixel(j, i % bit.Height,hock[i,0] > 0 ? Color.Black : Color.White);
            }
            bit.Save("kek.png");
            pictureBox1.Image = bit;
            //Console.WriteLine("End: " + hock.ToString());

            Console.WriteLine($"test1: {getPercent(mat1, hock)}\ntest2: {getPercent(mat2, hock)}\ntest3: {getPercent(mat3, hock)}");
        }

        static public double getPercent(Matrix<double> original, Matrix<double> with)
        {
            double max = original.ColumnCount;
            double now = 0;
            for (int i = 0; i < max; i++)
            {
                now = original[0, i] == with[i, 0] ? now + 1 : now;
            }
            return now / max;
        }

        static public Matrix<double> getFromPhoto(string path)
        {
            var bit = (Bitmap)Bitmap.FromFile(path);
            double[,] arrey = new double[1, bit.Width * bit.Height];
            for (int i = 0, j = 0; i < bit.Width * bit.Height; i++, j = j < bit.Height ? j++ : 0)
            {
                arrey[0, i] = isItBlack(bit.GetPixel(j, i % bit.Height)) ? 1 : -1;
            }
            return DenseMatrix.OfArray(arrey);
        }

        static public bool isItBlack(Color d)
        {
            return d.G == Color.Black.G && d.B == Color.Black.B && d.R == Color.Black.R ? true : false;
        }

        static public Matrix<double> getNormalized(Matrix<double> mag)
        {
            for (int i = 0; i < mag.RowCount; i++)
            {
                //mag[i, 0] = sinaps(mag[i, 0]);
                mag[i, 0] = mag[i, 0] >= 0 ? 1 : -1;
            }
            return mag;
        }

        static public double sinaps(double x, bool deriv = false)
        {
            if (deriv == true)
                return x * (1 - x);
            else
                return 1 / (1 + Math.Exp(-x));
        }
    }
}
