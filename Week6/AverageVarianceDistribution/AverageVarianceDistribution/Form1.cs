using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace AverageVarianceDistribution
{
    public partial class Form1 : Form
    {
        private List<double> heights;
        BackgroundWorker CSVDelegate;
        Rectangle LeftVirtual, RightVirtual;
        private Bitmap b;
        private Graphics g;

        public Form1()
        {
            InitializeComponent();
            this.heights = new List<double>();
            this.CSVDelegate = new BackgroundWorker();
            CSVDelegate.DoWork += DoWork;
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.g = Graphics.FromImage(this.b);
            this.g.Clear(Color.White);
            this.LeftVirtual = new Rectangle(20, 20, (this.b.Width/2) - 40, this.b.Height - 40);
            this.RightVirtual = new Rectangle((this.b.Width / 2) + 40, 20, (this.b.Width / 2) - 60, this.b.Height - 40);
            g.DrawRectangle(Pens.Black, LeftVirtual);
            g.DrawRectangle(Pens.Black, RightVirtual);
            this.pictureBox1.Image = b;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void Init()
        {
            String currentPath = AppDomain.CurrentDomain.BaseDirectory;
            String CSVPath = String.Format("{0}Resources\\hw_25000.csv", Path.GetFullPath(Path.Combine(currentPath, @"..\..\")));
            CSVDelegate.RunWorkerAsync(CSVPath);
        }

        private void DoWork(object? sender, DoWorkEventArgs e)
        {
            if(e.Argument == null)
            {
                throw new ArgumentNullException();
            }
            FileStream stream = File.OpenRead((String)e.Argument);
            using (TextFieldParser parser = new TextFieldParser(stream)) {
                parser.CommentTokens = new string[] { "#" };
                parser.SetDelimiters(new string[] { "," });
                parser.HasFieldsEnclosedInQuotes = true;
                parser.ReadFields();    //discard headers
                while (!parser.EndOfData)
                {
                    heights.Add(Math.Round(Double.Parse(parser.ReadFields()[1]) * 2.54, 2));
                    
                }
            }

            double mean = heights.Mean();
            double variance = heights.Variance();


            List<Tuple<double, double>> meanVariance = new List<Tuple<double, double>>();

            heights.Shuffle();

            double maxMean = 0, minMean = double.MaxValue, maxVariance = 0, minVariance = double.MaxValue;

            for(int i = 0, count = 0; i < 1000; ++i)
            {
                List<double> sublist = heights.GetRange(count, 10);
                double m = sublist.Mean(), v = sublist.Variance();
                if(m > maxMean)
                {
                    maxMean = m;
                }
                if(m < minMean)
                {
                    minMean = m;
                }
                if(v > maxVariance)
                {
                    maxVariance = v;
                }
                if(v < minVariance)
                {
                    minVariance = v;
                }
                meanVariance.Add(new Tuple<double, double>(m, v));
                count += 10;
            }

            Tuple<List<Interval>, List<Interval>> Distributions = GetIntervals(meanVariance, maxMean, minMean, maxVariance, minVariance);

            PlotDistribution(Distributions);

            List<double> means = meanVariance.Select((tuple) => tuple.Item1).ToList();
            List<double> variances = meanVariance.Select((tuple) => tuple.Item2).ToList();

            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => {
                    this.richTextBox1.Text = "Original Population:\n" +
                        "Mean: " + mean +
                        "\nVariance: " + variance +
                        "\n\nMean Distribution" +
                        "\nMean: " + means.Mean() +
                        "\nVariance: " + means.Variance() +
                        "\n\nVariance Distribution" +
                        "\nMean: " + variances.Mean() +
                        "\nVariance: " + variances.Variance();
                }));
            }
        }

        private Tuple<List<Interval>, List<Interval>> GetIntervals(List<Tuple<double, double>> values,
            double maxMean, double minMean, double maxVariance, double minVariance)
        {
            List<Interval> MeanIntervals = new List<Interval>();
            List<Interval> VarianceIntervals = new List<Interval>();

            int nIntervals = 5;

            double deltaMean = (maxMean - minMean) / nIntervals, deltaVariance = (maxVariance - minVariance) / nIntervals;

            for(int i = 0; i < nIntervals-1; ++i)
            {
                MeanIntervals.Add(new Interval(minMean, minMean + deltaMean, false, true));
                VarianceIntervals.Add(new Interval(minVariance, minVariance + deltaVariance, false, true));
                minMean += deltaMean;
                minVariance += deltaVariance;
            }
            MeanIntervals.Add(new Interval(minMean, maxMean, false, false));
            VarianceIntervals.Add(new Interval(minVariance, maxVariance, false, false));

            foreach(Tuple<double, double> v in values)
            {
                foreach(Interval interval in MeanIntervals)
                {
                    if (interval.includes(v.Item1))
                    {
                        interval.incrementCount();
                        break;
                    }
                }

                foreach(Interval interval in VarianceIntervals)
                {
                    if (interval.includes(v.Item2))
                    {
                        interval.incrementCount();
                        break;
                    }
                }
            }
            return new Tuple<List<Interval>, List<Interval>>(MeanIntervals, VarianceIntervals);
        }

        private PointF FromRealToVirtual(PointF XY, Point min, Point max, Rectangle r)
        {
            float newX = max.X - min.X == 0 ? 0 : (r.Left + r.Width * (XY.X - min.X) / (max.X - min.X));
            float newY = max.Y - min.Y == 0 ? 0 : (r.Top + r.Height - r.Height * (XY.Y - min.Y) / (max.Y - min.Y));
            return new PointF(newX, newY);
        }

        private void PlotDistribution(Tuple<List<Interval>, List<Interval>> intervals)
        {
            List<Rectangle> rects = new List<Rectangle>();

            List<Interval> means = intervals.Item1, variances = intervals.Item2;

            Point min = new Point(0, 0), maxM = new Point(6, means.Select(interval => interval.getCount()).Max()),
                maxV = new Point(6, variances.Select(interval => interval.getCount()).Max());

            for (int i = 0; i < 5; ++i)
            {
                PointF midPointM = FromRealToVirtual(new PointF(i, means[i].getCount()), min, maxM, LeftVirtual),
                    midPointV = FromRealToVirtual(new PointF(i, variances[i].getCount()), min, maxV, RightVirtual);
                RectangleF rM = new RectangleF(midPointM.X + 60, midPointM.Y, 20, LeftVirtual.Bottom - midPointM.Y),
                    rV = new RectangleF(midPointV.X + 60, midPointV.Y, 20, RightVirtual.Bottom - midPointV.Y);
                g.FillRectangle(Brushes.BlueViolet, rM);
                g.FillRectangle(Brushes.OrangeRed, rV);
            }
            if (InvokeRequired)
            {
                Invoke(((MethodInvoker)(() => this.pictureBox1.Image = b)));
            }
        }

    }
    
    /*
     *  List<double> extension for shuffling, element swapping, and Mean/Variance Calculation
     */
    public static class StatListExtension
    {
        public static double Mean(this List<double> values)
        {
            return values.Count == 0 ? 0 : values.Mean(0, values.Count);
        }

        public static double Mean(this List<double> values, int start, int end)
        {
            double retVal = 0;
            for(int i = start; i < end; ++i)
            {
                retVal += values[i];
            }
            return retVal / (end-start);
        }

        public static double Variance(this List<double> values)
        {
            return values.Variance(values.Mean(), 0, values.Count);
        }


        public static double Variance(this List<double> values, double mean, int start, int end)
        {
            double retVal = 0;
            for (int i = start; i < end; ++i)
            {
                retVal += Math.Pow((values[i] - mean), 2);
            }

            return retVal / (end - start);
        }

        public static void Shuffle(this List<double> values)
        {
            Random rnd = new Random();
            for(int i = values.Count; i > 0; i--)
            {
                values.Swap(0, rnd.Next(0, i));
            }
        }

        public static void Swap(this List<double> list, int i, int j)
        {
            (list[j], list[i]) = (list[i], list[j]);
        }
    }
}