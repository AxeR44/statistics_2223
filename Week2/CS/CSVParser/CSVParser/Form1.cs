using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

namespace CSVParser
{
    public partial class Form1 : Form
    {

        private BackgroundWorker? asyncWorker;

        private bool fileOpened = false;
        private string filename = string.Empty;

        public Form1()
        {
            InitializeComponent();
            view.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.asyncWorker = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!fileOpened)
            {
                view.AllowUserToAddRows = false;
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.InitialDirectory = "C:\\";
                    dialog.Filter = "CSV Files (*.csv)|*.csv";
                    dialog.FilterIndex = 1;
                    dialog.RestoreDirectory = true;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        this.filename = dialog.FileName;
                        this.textBox1.Text = this.filename;

                        Stream fileStream = dialog.OpenFile();

                        this.asyncWorker = new BackgroundWorker();
                        this.asyncWorker.DoWork += DoWork;
                        this.asyncWorker.RunWorkerAsync(fileStream);
                    }
                }
            }
            else
            {
                this.view.DataSource = null;
                this.view.Rows.Clear();
                this.view.Columns.Clear();
                this.textBox1.Clear();
                this.button1.Text = "Browse";
                this.fileOpened = false;
                this.button2.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<string, Type> columns = new Dictionary<string, Type>();
            foreach (DataGridViewTextBoxColumn column in view.Columns)
            {
                columns.Add(column.Name, column.ValueType);
            }
            Form2 f2 = new Form2(columns);
            if (f2.ShowDialog() == DialogResult.OK)
            {
                view.ClearSelection();
                view.Columns[f2.SelectedColumn].Selected = true;

                List<Interval> intervalList = f2.typeSelected == typeof(double) ? getIntervalsDouble(f2.intervalNumber) : getIntervalsStrings();
                new Form3(f2.SelectedColumn, intervalList).Show();
            }
        }
        private List<Interval> getIntervalsDouble(uint intervalN)
        {
            List<double> valueList = new List<double>();
            foreach (DataGridViewCell cell in view.SelectedCells)
            {
                valueList.Add(Double.Parse((string)cell.Value));
            }
            double minValue = valueList.Min();
            double maxValue = valueList.Max();
            double delta = (maxValue - minValue) / intervalN;
            List<Interval> intervalList = new List<Interval>();
            for (int i = 0; i < intervalN - 1; i++)
            {
                intervalList.Add(new Interval(minValue, minValue + delta, false, true));
                minValue = minValue + delta;
            }
            intervalList.Add(new Interval(minValue, maxValue, false, false));
            foreach (double d in valueList)
            {
                foreach (Interval interval in intervalList)
                {
                    if (interval.includes(d))
                    {
                        interval.incrementCount();
                        break;
                    }
                }
            }
            return intervalList;
        }

        private List<Interval> getIntervalsStrings()
        {

            Dictionary<String, Interval> assocInterval = new Dictionary<string, Interval>();
            foreach (DataGridViewCell cell in view.SelectedCells)
            {
                string cellVal = (string)cell.Value;
                if (assocInterval.ContainsKey(cellVal))
                {
                    assocInterval[cellVal].incrementCount();
                }
                else
                {
                    Interval tmp = new Interval(cellVal);
                    tmp.incrementCount();
                    assocInterval.Add(cellVal, tmp);
                }
            }
            return assocInterval.Values.ToList();
        }

        private void DoWork(object? sender, DoWorkEventArgs e)
        {
            if(e.Argument == null)
            {
                throw new ArgumentNullException();
            }
            Stream fStream = (Stream)e.Argument;
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => this.button1.Enabled = false));
            }
            using (TextFieldParser parser = new TextFieldParser(fStream))
            {
                parser.CommentTokens = new string[] { "#" };
                parser.SetDelimiters(new string[] { "," });
                parser.HasFieldsEnclosedInQuotes = true;
                try
                {
                    string[]? headers = parser.ReadFields();

                    if (!parser.EndOfData)
                    {
                        string[]? firstRow = parser.ReadFields();
                        List<Type> columnsType = new List<Type>();
                        foreach (string str in firstRow)
                        {
                            double test;
                            if (Double.TryParse(str, out test))
                            {
                                columnsType.Add(typeof(double));
                            }
                            else
                            {
                                columnsType.Add(typeof(string));
                            }
                        }

                        foreach (String str in headers)
                        {
                            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                            column.SortMode = DataGridViewColumnSortMode.NotSortable;
                            column.ReadOnly = true;
                            column.DataPropertyName = str;
                            column.HeaderText = str;
                            column.Name = str;
                            column.ValueType = columnsType[0];
                            columnsType.RemoveAt(0);
                            if (InvokeRequired)
                            {
                                Invoke((MethodInvoker)(() => this.view.Columns.Add(column)));
                            }
                        }
                        if (InvokeRequired)
                        {
                            Invoke((MethodInvoker)(() => this.view.Rows.Add(firstRow)));
                        }
                        while (!parser.EndOfData)
                        {
                            if (InvokeRequired)
                            {
                                Invoke((MethodInvoker)(() => this.view.Rows.Add(parser.ReadFields())));
                            }
                        }
                        this.fileOpened = true;
                        if (InvokeRequired)
                        {
                            Invoke((MethodInvoker)(() =>
                            {
                                this.button1.Text = "Close File";
                                this.button2.Enabled = true;
                            }));
                        }
                    }
                }
                catch (MalformedLineException ex)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        this.view.DataSource = null;
                        this.view.Rows.Clear();
                        this.view.Columns.Clear();
                        this.textBox1.Clear();
                    }));
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK);
                }
                finally
                {
                    if (InvokeRequired)
                    {
                        Invoke((MethodInvoker)(() => this.button1.Enabled = true));
                    }
                    fStream.Close();
                }
            }
        }
    }
}