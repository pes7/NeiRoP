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
using MathNet.Numerics.LinearAlgebra.Single;

namespace ImageConvisor
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        //float[,] _a = { {22, 15, 1 },
        //                 {42, 5, 38},
        //                 {28, 9, 4} };
        //float[,] _b = { { 0, 0, 1 },
        //                 { 0, 0, 0},
        //                 { 1, 0, 0} };
        //var _g1 = DenseMatrix.OfArray(_a);
        //var _g2 = DenseMatrix.OfArray(_b);
        //var max = _g1 * _g2;
        //Console.WriteLine(max.ToString());
        Matrix<float> red, green, blue;
        static string _path = "ex1.exp";
        private void button1_Click(object sender, EventArgs e)
        {
            Inicialize();
            button2.Enabled = true;
            if (th != null)
                th.Abort();
        }

        public void Inicialize()
        {
            bool th1 = false, th2 = false, th3 = false;
            RGB d1 = (RGB)Enum.Parse(typeof(RGB), comboBox1.Text), d2 = (RGB)Enum.Parse(typeof(RGB), comboBox2.Text), d3 = (RGB)Enum.Parse(typeof(RGB), comboBox3.Text);
            Task.Run(async () =>
            {
                red = await parceImage(d1, _path);
                th1 = true;
            });
            Task.Run(async () =>
            {
                green = await parceImage(d2, _path);
                th2 = true;
            });
            Task.Run(async () =>
            {
                blue = await parceImage(d3, _path);
                th3 = true;
            });
            do
            {

            } while ((th1 == false || th2 == false) || (th1 == false || th3 == false));
            th1 = false;
            th2 = false;
            th3 = false;

            var original = Bitmap.FromFile(_path);
            pictureBox1.Image = original;
            pictureBox2.Image = bitConvertSigmentary(RGB.Red, red.ToArray());
            pictureBox3.Image = bitConvertSigmentary(RGB.Green, green.ToArray());
            pictureBox4.Image = bitConvertSigmentary(RGB.Blue, blue.ToArray());
            pictureBox5.Image = bitConvertSigmentary(RGB.Red, red.ToArray(), green.ToArray(), blue.ToArray());
        }

        public static float[,] _mask;

        #region castrateMatrix Function
        private static float[,] castrateMatrix(Matrix<float> _matrix)
        {
            float[,] _mat = _matrix.ToArray();
            float[,] _newArr = new float[_matrix.RowCount - 2, _matrix.ColumnCount - 2];
            for(int i = 1, _ireal = 0; i < _matrix.RowCount - 1; i++, _ireal++)
            {
                for(int j = 1, _jreal = 0; j < _matrix.ColumnCount - 1; j++, _jreal++)
                {
                    _newArr[_ireal, _jreal] = getMaskedMatrix(getSubMatrix(_mat, i - 1, i + 1, j - 1, j + 1), _mask);
                }
            }
            return _newArr;
        }

        private static async Task<float[,]> castrateMatrix(float[,] _matrix)
        {
            float[,] _newArr = new float[_matrix.GetLength(0) - 2, _matrix.GetLength(1) - 2];
            
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

        //_a and _mask need to be similar dimensions
        private static float getMaskedMatrix(float [,] _a, float[,] _mask)
        {
            float value = 0;
            //if (_a.GetLength(0) == _mask.GetLength(0) && _a.GetLength(1) == _mask.GetLength(1))
            //{
                for (int i = 0; i < _a.GetLength(0); i++)
                {
                    for(int j = 0; j < _a.GetLength(1); j++)
                    {
                        value += _a[i,j] * _mask[i, j];
                    }
                }
            //}
            return value;
        }

        private static float[,] getSubMatrix(float[,] _a, int i_start, int i_end, int j_start, int j_end)
        {
            float[,] _nex = new float[i_end - i_start + 1, j_end - j_start + 1];
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
        private static async Task<Matrix<float>> parceImage(RGB rGB, string path)
        {
            var image = (Bitmap)Bitmap.FromFile(path);
            float[,] _array = new float[image.Height, image.Width];
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

        private static Bitmap bitConvertSigmentary(RGB rGB,float[,] _arrayR, float[,] _arrayG = null, float[,] _arrayB = null)
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
        
        private static float[,] _r, _g, _b;
        [Cudafy]
        private void Genereate()
        {
            _mask = new float[,] { { float.Parse(textBox1.Text), float.Parse(textBox2.Text), float.Parse(textBox3.Text) },
                                    { float.Parse(textBox4.Text), float.Parse(textBox5.Text), float.Parse(textBox6.Text) },
                                    { float.Parse(textBox7.Text), float.Parse(textBox8.Text), float.Parse(textBox9.Text) } };
            int progon = trackBar1.Value;
            _r = red.ToArray();
            _g = green.ToArray();
            _b = blue.ToArray();

            bool th1 = false, th2 = false, th3 = false;
            for (int i = 0; i < progon; i++)
            {
                //7.6
                //7.6
                //7.0
                //5.62
                //5.54
                //3.27
                Task.Run(async () =>
                {
                    _r = await castrateMatrix(_r);
                    th1 = true;
                });
                Task.Run(async () =>
                {
                    _g = await castrateMatrix(_g);
                    th2 = true;
                });
                Task.Run(async () =>
                {
                    _b = await castrateMatrix(_b);
                    th3 = true;
                });
                do
                {
                    
                } while ((th1 == false || th2 == false) || (th1 == false || th3 == false));
                
                th1 = false;
                th2 = false;
                th3 = false;
            }
            pictureBox5.Image = bitConvertSigmentary(RGB.Red, _r, _g, _b);
            pictureBox5.Image.Save("exp.png");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (trackBar1.Value < trackBar1.Maximum)
                trackBar1.Value++;
            label1.Text = $"{trackBar1.Value} pts.";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (trackBar1.Value > 1)
                trackBar1.Value--;
            label1.Text = $"{trackBar1.Value} pts.";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            setTextToMatrix(new float[,] { { 0,0,0},{ 0,1,0},{ 0,0,0} });
        }

        private void setTextToMatrix(float[,] fl)
        {
            textBox1.Text = fl[0,0].ToString(); textBox2.Text = fl[0, 1].ToString(); textBox3.Text = fl[0, 2].ToString();
            textBox4.Text = fl[1, 0].ToString(); textBox5.Text = fl[1, 1].ToString(); textBox6.Text = fl[1, 2].ToString();
            textBox7.Text = fl[2, 0].ToString(); ; textBox8.Text = fl[2, 1].ToString(); ; textBox9.Text = fl[1, 2].ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var fl = new float[,] { { float.Parse(textBox1.Text), float.Parse(textBox2.Text), float.Parse(textBox3.Text) },
                                    { float.Parse(textBox4.Text), float.Parse(textBox5.Text), float.Parse(textBox6.Text) },
                                    { float.Parse(textBox7.Text), float.Parse(textBox8.Text), float.Parse(textBox9.Text) } };
            th = new Thread(() =>
            {
                Clipboard.SetText(floatArrayToString(fl));
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            Thread.Sleep(20);
            th.Abort();
        }

        private static string floatArrayToString(float[,] fl)
        {
            return $"{fl[0,0]} {fl[0, 1]} {fl[0, 2]}{System.Environment.NewLine}{fl[1, 0]} {fl[1, 1]} {fl[1, 2]}{System.Environment.NewLine}{fl[2, 0]} {fl[2, 1]} {fl[2, 2]}";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            float[,] _math = new float[3, 3];
            th = new Thread(() =>
            {
                var text = Clipboard.GetText();
                if(text.Length > 0)
                {
                    try
                    {
                        text = text.Replace('\n', ' ');
                        text = text.Replace("\r", string.Empty);
                        var separete = text.Split(' ');
                        int g = 0;
                        for(int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                _math[i,j] = float.Parse(separete[g]);
                                g++;
                            }
                        }
                    }
                    catch { }
                }
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            Thread.Sleep(50);
            th.Abort();
            setTextToMatrix(_math);
        }

        static Thread th;
        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dr = DialogResult.None;
            th = new Thread(() =>
            {
                openFileDialog1.Filter = "jpg and png files (*.exp)|*.exp";
                dr = openFileDialog1.ShowDialog();
                while (dr == DialogResult.OK)
                {
                    if (openFileDialog1.FileName != null)
                    {
                        _path = openFileDialog1.FileName;
                    }
                }
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
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
