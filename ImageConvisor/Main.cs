using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cudafy;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ImageConvisor
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        //double[,] _a = { {22, 15, 1 },
        //                 {42, 5, 38},
        //                 {28, 9, 4} };
        //double[,] _b = { { 0, 0, 1 },
        //                 { 0, 0, 0},
        //                 { 1, 0, 0} };
        //var _g1 = DenseMatrix.OfArray(_a);
        //var _g2 = DenseMatrix.OfArray(_b);
        //var max = _g1 * _g2;
        //Console.WriteLine(max.ToString());
        Matrix<double> red, green, blue;
        private void button1_Click(object sender, EventArgs e)
        {
            string _path = "example.png";
            red = parceImage((RGB)Enum.Parse(typeof(RGB),comboBox1.Text), _path);
            green = parceImage((RGB)Enum.Parse(typeof(RGB), comboBox2.Text), _path);
            blue = parceImage((RGB)Enum.Parse(typeof(RGB), comboBox3.Text), _path);
            var original = Bitmap.FromFile(_path);
            pictureBox1.Image = original;
            pictureBox2.Image = bitConvertSigmentary(RGB.Red, red.ToArray());
            pictureBox3.Image = bitConvertSigmentary(RGB.Green, green.ToArray());
            pictureBox4.Image = bitConvertSigmentary(RGB.Blue, blue.ToArray());
            pictureBox5.Image = bitConvertSigmentary(RGB.Red, red.ToArray(), green.ToArray(), blue.ToArray());
        }

        public static double[,] _mask;

        #region castrateMatrix Function
        private static double[,] castrateMatrix(Matrix<double> _matrix)
        {
            double[,] _mat = _matrix.ToArray();
            double[,] _newArr = new double[_matrix.RowCount - 2, _matrix.ColumnCount - 2];
            for(int i = 1, _ireal = 0; i < _matrix.RowCount - 1; i++, _ireal++)
            {
                for(int j = 1, _jreal = 0; j < _matrix.ColumnCount - 1; j++, _jreal++)
                {
                    _newArr[_ireal, _jreal] = getMaskedMatrix(getSubMatrix(_mat, i - 1, i + 1, j - 1, j + 1), _mask);
                }
            }
            return _newArr;
        }

        private static double[,] castrateMatrix(double[,] _matrix)
        {
            double[,] _newArr = new double[_matrix.GetLength(0) - 2, _matrix.GetLength(1) - 2];
            
            for (int i = 1, _ireal = 0; i < _matrix.GetLength(0) - 1; i++, _ireal++)
            {
                for (int j = 1, _jreal = 0; j < _matrix.GetLength(1) - 1; j++, _jreal++)
                {
                    _newArr[_ireal, _jreal] = getMaskedMatrix(getSubMatrix(_matrix, i - 1, i + 1, j - 1, j + 1), _mask);
                }
            }
            return _newArr;
        }
        #endregion


        private static double getMaskedMatrix(double [,] _a, double[,] _mask)
        {
            double value = 0;
            if (_a.GetLength(0) == _mask.GetLength(0) && _a.GetLength(1) == _mask.GetLength(1))
            {
                for (int i = 0; i < _a.GetLength(0); i++)
                {
                    for(int j = 0; j < _a.GetLength(1); j++)
                    {
                        value += _a[i,j] * _mask[i, j];
                    }
                }
            }
            else throw new Exception("_a and _mask need to be similar dimensions");
            return value;
        }

        private static double[,] getSubMatrix(double[,] _a, int i_start, int i_end, int j_start, int j_end)
        {
            double[,] _nex = new double[i_end - i_start + 1, j_end - j_start + 1];
            for(int i = i_start, i_sub = 0; i <= i_end; i++, i_sub++)
            {
                for(int j = j_start, j_sub = 0; j <= j_end; j++, j_sub++)
                {
                    _nex[i_sub, j_sub] = _a[i, j];
                }
            }
            return _nex;
        }

        public enum RGB {Red,Green,Blue};
        private static Matrix<double> parceImage(RGB rGB, string path)
        {
            var image = (Bitmap)Bitmap.FromFile(path);
            double[,] _array = new double[image.Height, image.Width];
            for(int i=0; i < image.Height; i++)
            {
                for(int j=0;j < image.Width; j++)
                {
                    _array[i, j] = getSubPixel(rGB, image, i, j);
                }
            }
            return DenseMatrix.OfArray(_array);
        }

        private static int getSubPixel(RGB rGB, Bitmap bit,int i ,int j)
        {
            switch (rGB)
            {
                case RGB.Red:
                    return bit.GetPixel(j, i).R;
                case RGB.Green:
                    return bit.GetPixel(j, i).G;
                case RGB.Blue:
                    return bit.GetPixel(j, i).B;
            }
            return 0;
        }

        private static Bitmap bitConvertSigmentary(RGB rGB,double[,] _arrayR, double[,] _arrayG = null, double[,] _arrayB = null)
        {
            Bitmap bit = new Bitmap(_arrayR.GetLength(1), _arrayR.GetLength(0));
            for (int i = 0; i < _arrayR.GetLength(0); i++)
            {
                for (int j = 0; j < _arrayR.GetLength(1); j++)
                {
                    if (_arrayG != null && _arrayB != null)
                        bit.SetPixel(j, i, Color.FromArgb((int)_arrayR[i, j] > 255 ? 255 : (int)_arrayR[i, j] < 0 ? 0 : (int)_arrayR[i, j],
                                                          (int)_arrayG[i, j] > 255 ? 255 : (int)_arrayG[i, j] < 0 ? 0 : (int)_arrayG[i, j],
                                                          (int)_arrayB[i, j] > 255 ? 255 : (int)_arrayB[i, j] < 0 ? 0 : (int)_arrayB[i, j]));
                    else
                        bit.SetPixel(j, i, getColorFromSigment(rGB, (int)_arrayR[i, j]));
                }
            }
            return bit;
        }

        private static Color getColorFromSigment(RGB rGB,int _sigm)
        {
            return rGB == RGB.Red ? Color.FromArgb(_sigm > 255 ? 255 : _sigm < 0 ? 0 : _sigm,0,0) : rGB == RGB.Green ? Color.FromArgb(0, _sigm > 255 ? 255 : _sigm < 0 ? 0 : _sigm, 0) : Color.FromArgb(0, 0, _sigm > 255 ? 255 : _sigm < 0 ? 0 : _sigm);
        }

        private void Genereate()
        {
            _mask = new double[,] { { int.Parse(textBox1.Text), int.Parse(textBox2.Text), int.Parse(textBox3.Text) },
                                    { int.Parse(textBox4.Text), int.Parse(textBox5.Text), int.Parse(textBox6.Text) },
                                    { int.Parse(textBox7.Text), int.Parse(textBox8.Text), int.Parse(textBox9.Text) } };
            int progon = trackBar1.Value;
            double[,] _r = red.ToArray(), _g = green.ToArray(), _b = blue.ToArray();
            for (int i = 0; i < progon; i++)
            {
                _r = castrateMatrix(_r);
                _g = castrateMatrix(_g);
                _b = castrateMatrix(_b);
            }
            pictureBox5.Image = bitConvertSigmentary(RGB.Red, _r, _g, _b);
            pictureBox5.Image.Save("exp.png");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Genereate();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = $"{trackBar1.Value} pts.";
        }
    }
}
