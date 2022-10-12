using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSVParser
{
    public partial class Form3 : Form
    {
        public Form3(string varName, List<Interval> intervalList)
        {
            InitializeComponent();
            this.label1.Text += varName;
            this.richTextBox1.ScrollBars = RichTextBoxScrollBars.Both;
            this.richTextBox1.Enabled = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            List<string> counters = new List<string>();
            foreach(Interval interval in intervalList)
            {
                string colName;
                if(interval.value == null)
                {
                    colName = interval.min.ToString() + "-" + interval.max.ToString();
                }
                else
                {
                    colName = interval.value;
                }
                richTextBox1.Text += "\n" + colName + "\t\t" + interval.getCount();
            }
        }
    }
}
