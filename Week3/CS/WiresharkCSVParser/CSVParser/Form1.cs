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
                List<object> col1 = getCellsValue(f2.typeSelected);

                List<Interval> interval1List = f2.typeSelected == typeof(double) ? getIntervalsDouble(col1, f2.intervalNumber) : getIntervalsStrings(col1);
                


                /*Generating intervals for column 2*/
                view.ClearSelection();
                view.Columns[f2.SelectedColumn2].Selected = true;
                List<object> col2 = getCellsValue(f2.typeSelected2);

                List <Interval> interval2List = f2.typeSelected2 == typeof(double) ? getIntervalsDouble(col2, f2.intervalNumber2) : getIntervalsStrings(col2);

                List<BivariateInterval> bivariateIntervals = new List<BivariateInterval>();

                foreach(Interval i in interval1List)
                {
                    bivariateIntervals.Add(new BivariateInterval(
                        i,
                        interval2List.Select(interval => new Interval(
                            interval.min, 
                            interval.max, 
                            interval.value, 
                            interval.openedLeft, 
                            interval.openedRight, 
                            interval.valueCount
                            )).ToList()
                        )
                    );
                }
                for(int i = 0; i<col2.Count; ++i) {
                    foreach (BivariateInterval bivariateInterval in bivariateIntervals)
                    {
                        if(bivariateInterval.includes(col1.ElementAt(i), col2.ElementAt(i)))
                        {
                            break;
                        }
                    }
                }
                new Form3(f2.SelectedColumn, f2.SelectedColumn2, bivariateIntervals).Show();
            }
        }

        private List<object> getCellsValue(Type t)
        {
            List<object> cells = new List<object>();
            foreach(DataGridViewCell cell in view.SelectedCells)
            {
                cells.Add(convertToObj(cell.Value.ToString(), t));
            }
            return cells;
        }

        private object convertToObj(string s, Type t)
        {
            if(t == typeof(string))
            {
                return (object)s;
            }
            return (object)double.Parse(s);
        }

        private List<Interval> getIntervalsDouble(List<object> cellsValue, uint intervalN)
        {
            double minValue = (double)cellsValue.Min();
            double maxValue = (double)cellsValue.Max();
            double delta = (maxValue - minValue) / intervalN;
            List<Interval> intervalList = new List<Interval>();
            for (int i = 0; i < intervalN - 1; i++)
            {
                intervalList.Add(new Interval(minValue, minValue + delta, false, true));
                minValue = minValue + delta;
            }
            intervalList.Add(new Interval(minValue, maxValue, false, false));
            return intervalList;
        }

        private List<Interval> getIntervalsStrings(List<object> strings)
        {
            Dictionary<String, Interval> assocInterval = new Dictionary<string, Interval>();
            foreach (string s in strings)
            {
                if (!assocInterval.ContainsKey(s))
                {
                    assocInterval.Add(s, new Interval(s));
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