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
    public partial class Form2 : Form
    {
        public string? SelectedColumn = null;
        public UInt32 intervalNumber;
        public Type? typeSelected;
        private Dictionary<string, Type> columns;

        public Form2(Dictionary<string, Type> columns)
        {
            this.columns = columns;
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach(string s in columns.Keys)
            {
                this.comboBox1.Items.Add(s);
            }
            this.comboBox1.SelectedIndex = 0;
            this.numericUpDown1.Minimum = 1;
            this.intervalNumber = 1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedColumn = ((ComboBox)sender).SelectedItem.ToString();
            if((this.typeSelected = columns.Values.ElementAt(((ComboBox)sender).SelectedIndex)) == typeof(string))
            {
                this.numericUpDown1.Enabled = false;
            }
            else
            {
                this.numericUpDown1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.intervalNumber = (UInt32)((NumericUpDown)sender).Value;
        }
    }
}
