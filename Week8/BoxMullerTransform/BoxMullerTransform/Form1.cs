using System.Security.Principal;

namespace BoxMullerTransform
{
    public partial class Form1 : Form
    {
        private int nSamples;
        private Bitmap b;
        private Graphics g;
        private Rectangle SamplesWindow, XDistribution, YDistribution;
        RandomPolar PolarRNG;
        Point min = new Point(-1, -1), max = new Point(1,1);
        private int nIntervals = 30;

        public Form1()
        {
            InitializeComponent();
            this.numericUpDown1.Minimum = 100;
            this.numericUpDown1.Maximum = 100000;
            this.numericUpDown1.Value = this.numericUpDown1.Minimum;
            this.nSamples = (int)this.numericUpDown1.Value;
            b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            g = Graphics.FromImage(b);
            this.SamplesWindow = new Rectangle(400, 200, this.pictureBox1.Width - 820, this.pictureBox1.Height - 220);
            this.XDistribution = new Rectangle(SamplesWindow.Left, 0, SamplesWindow.Width, 200);
            this.YDistribution = new Rectangle(SamplesWindow.Right, SamplesWindow.Top, 200, SamplesWindow.Height);
            this.PolarRNG = new RandomPolar();
            resetGraphics();
            pictureBox1.Image = b;
            
        }

        private void resetGraphics()
        {
            g.Clear(Color.White);
            g.DrawRectangle(Pens.Black, this.SamplesWindow);
            g.DrawRectangle(Pens.Black, this.XDistribution);
            g.DrawRectangle(Pens.Black, this.YDistribution);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.nSamples = (int)((NumericUpDown)sender).Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            resetGraphics();
            List<Tuple<double, double>> pointList = new List<Tuple<double, double>>();
            for(int i = 0; i < nSamples; ++i)
            {
                pointList.Add(PolarRNG.NextPolar());
            }

            foreach(Tuple<double, double> t in pointList)
            {
                Tuple<double, double> mapped = fromRealToVirtual(t, min, max, SamplesWindow);
                RectangleF r = new RectangleF((float)mapped.Item1 - 1, (float)mapped.Item2 - 1, 2, 2);
                g.FillEllipse(Brushes.Black, r);
            }
            plotDistribution(pointList);
            pictureBox1.Image = b;
        }

        private void plotDistribution(List<Tuple<double, double>> XY)
        {
            double delta = 2.0 / nIntervals;
            double min = -1, max = 1;
            List<Interval> intervalList = new List<Interval>();
            for(int i = 0; i < nIntervals - 1; ++i)
            {
                intervalList.Add(new Interval(min, min + delta, false, true));
                min += delta;
            }
            intervalList.Add(new Interval(min, max, false, false));

            foreach(Tuple<double, double> t in XY)
            {
                bool foundX = false, foundY = false;
                foreach(Interval i in intervalList)
                {
                    if (i.includes(t.Item1))
                    {
                        foundX = true;
                        i.incrementCountX();
                    }
                    if (i.includes(t.Item2))
                    {
                        foundY = true;
                        i.incrementCountY();
                    }
                    if(foundX && foundY)
                    {
                        break;
                    }
                }
            }
            List<RectangleF> rectangles = new List<RectangleF>();
            Point minHisto = new Point(0, 0);
            List<Tuple<int, int>> TupleList = intervalList.Select((interval) => interval.getCount()).ToList();
            Point maxHistoX = new Point(nIntervals, TupleList.Select((tuple) => tuple.Item1).Max());
            Point maxHistoY = new Point(TupleList.Select((tuple) => tuple.Item2).Max(), nIntervals);
            for(int i = 0; i < nIntervals; ++i)
            {
                Tuple<int, int> counters = TupleList[i];
                Tuple<double, double> mappedX = fromRealToVirtual(new Tuple<double, double>(i, counters.Item1), minHisto, maxHistoX, XDistribution),
                    mappedY = fromRealToVirtualVertical(new Tuple<double, double>(counters.Item2, i), minHisto, maxHistoY, YDistribution);
                rectangles.Add(new RectangleF((float)mappedX.Item1, (float)mappedX.Item2, 20, XDistribution.Bottom - (float)mappedX.Item2));
                rectangles.Add(new RectangleF(YDistribution.Left, (float)mappedY.Item2, (float)mappedY.Item1 - YDistribution.Left - 20, 20));
            }
            g.FillRectangles(Brushes.BlueViolet, rectangles.ToArray());
        }



        private Tuple<double, double> fromRealToVirtual(Tuple<double, double> XY, Point min, Point max, Rectangle r)
        {
            double newX = max.X - min.X == 0 ? 0 : (r.Left + r.Width * (XY.Item1 - min.X) / (max.X - min.X));
            double newY = max.Y - min.Y == 0 ? 0 : (r.Top + r.Height - r.Height * (XY.Item2 - min.Y) / (max.Y - min.Y));
            return new Tuple<double, double>(newX, newY);
        }

        private Tuple<double, double> fromRealToVirtualVertical(Tuple<double, double> XY, Point min, Point max, Rectangle r)
        {
            double newX = max.X - min.X == 0 ? 0 : (r.Left + r.Width * (XY.Item1 - min.X) / (max.X - min.X));
            double newY = max.Y - min.Y == 0 ? 0 : (r.Top + r.Height * (XY.Item2 - min.Y) / (max.Y - min.Y));
            return new Tuple<double, double>(newX, newY);
        }
    }
}