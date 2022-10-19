using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSVParser
{
    
    public partial class Form3 : Form
    {
        private BackgroundWorker worker;
        public Form3(string var1Name, string var2name, List<BivariateInterval> intervalList)
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            this.label1.Text += "\n" + var1Name + "(rows) and " + var2name + "(columns)";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.view.AllowUserToAddRows = false;
            worker.DoWork += DoWork;
            worker.RunWorkerAsync(intervalList);
        }

        private void DoWork(object? sender, DoWorkEventArgs e)
        {
            List<BivariateInterval> intervalList = (List<BivariateInterval>)e.Argument;
            List<string> columns = intervalList.ElementAt(0).getInvervalDetails().Values.ElementAt(0).Select(tuple => tuple.Item1).ToList();
            foreach (string colName in columns)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                column.ReadOnly = true;
                column.DataPropertyName = colName;
                column.HeaderText = colName;
                column.Name = colName;
                column.ValueType = typeof(int);
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)(() => this.view.Columns.Add(column)));
                }
            }

            foreach(BivariateInterval interval in intervalList)
            {
                Dictionary<string, List<Tuple<string, int>>> intervalDetails = interval.getInvervalDetails();
                DataGridViewRow row = createGridViewRow(intervalDetails);
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)(() => this.view.Rows.Add(row)));
                }
            }
        }

        private DataGridViewRow createGridViewRow(Dictionary<string, List<Tuple<string, int>>> keyValuePairs)
        {
            DataGridViewRow Row = new DataGridViewRow();
            DataGridViewRowHeaderCell cell = new DataGridViewRowHeaderCell();
            object[] cellValues = keyValuePairs.Values.ElementAt(0).Select(tuple => tuple.Item2.ToString()).ToArray();
            Row.CreateCells(view, cellValues);
            cell.Value = keyValuePairs.Keys.ElementAt(0);
            Row.HeaderCell = cell;
            return Row;
            //cell.Value = intervalDetails.Keys.ElementAt(0);
        }


    }
}
