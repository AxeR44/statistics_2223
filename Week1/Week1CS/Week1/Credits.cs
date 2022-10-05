using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Week1{
    public partial class Credits : Form{
        public Credits(){
            InitializeComponent();
            label1.Text = "This application was written by" +
                "\nAlessandro Albini (AxeR)" +
                "\n(ID 2032013)" +
                "\n Github: https://github.com/AxeR44/statistics_2223" +
                "\n Website: https://axer44.github.io/statistics_2223/";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            string pwd = Directory.GetCurrentDirectory();
            Bitmap bmp = new Bitmap(".\\assets\\sapienza.png");
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = bmp;
        }
    }
}
