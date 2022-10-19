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
        public string? SelectedColumn = null, SelectedColumn2 = null;
        public UInt32 intervalNumber, intervalNumber2;
        public Type? typeSelected, typeSelected2;
        private Dictionary<string, Type> columns;

        public Form2(Dictionary<string, Type> columns)
        {
            this.columns = columns;
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach(string s in columns.Keys)
            {
                this.comboBox1.Items.Add(s);
            }
            this.comboBox1.SelectedIndex = 0;
            this.numericUpDown1.Minimum = 1;
            this.intervalNumber = 1; 
            this.numericUpDown2.Minimum = 1;
            this.intervalNumber2 = 1;
            updateCombo2ItemList();
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
            updateCombo2ItemList();
            
        }

        private void updateCombo2ItemList()
        {
            this.comboBox2.Items.Clear();
            foreach (string s in columns.Keys)
            {
                if (s == this.SelectedColumn)
                {
                    continue;
                }
                this.comboBox2.Items.Add(s);
            }
            this.comboBox2.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            this.intervalNumber2 = (UInt32)((NumericUpDown)sender).Value;
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedColumn2 = ((ComboBox)sender).SelectedItem.ToString();
            if ((this.typeSelected2 = columns[SelectedColumn2]) == typeof(string))
            {
                this.numericUpDown2.Enabled = false;
            }
            else
            {
                this.numericUpDown2.Enabled = true;
            }
        }
    }
}
