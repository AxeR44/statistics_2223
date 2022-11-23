using System.Security.Principal;

namespace OtherDistributions
{
    public partial class Form1 : Form
    {
        private int nSamples;
        private Bitmap b;
        private Graphics g;
        private Rectangle NormalDistribution, CauchyDistribution, ChiSquaredDistribution, TStudentDistribution, FFisherDistribution;
        RandomPolar PolarRNG;
        private int nIntervals = 30;
        private const int W = 400, H = 300;

        public Form1()
        {
            InitializeComponent();
            this.numericUpDown1.Minimum = 100;
            this.numericUpDown1.Maximum = 100000;
            this.numericUpDown1.Value = this.numericUpDown1.Minimum;
            this.nSamples = (int)this.numericUpDown1.Value;
            b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            g = Graphics.FromImage(b);
            this.NormalDistribution = new Rectangle(80, 20, W, H);
            this.CauchyDistribution = new Rectangle(this.NormalDistribution.Right + 10, this.NormalDistribution.Top, W, H);
            this.ChiSquaredDistribution = new Rectangle(this.CauchyDistribution.Right + 10, this.CauchyDistribution.Top, W, H);
            this.TStudentDistribution = new Rectangle(this.NormalDistribution.Right - 200, this.NormalDistribution.Bottom + 50, W, H);
            this.FFisherDistribution = new Rectangle(this.TStudentDistribution.Right + 10, this.TStudentDistribution.Top, W, H);
            this.PolarRNG = new RandomPolar();
            resetGraphics();
            pictureBox1.Image = b;

        }

        private void resetGraphics()
        {
            g.Clear(Color.White);
            g.DrawRectangle(Pens.Black, this.NormalDistribution);
            g.DrawRectangle(Pens.Black, this.CauchyDistribution);
            g.DrawRectangle(Pens.Black, this.ChiSquaredDistribution);
            g.DrawRectangle(Pens.Black, this.TStudentDistribution);
            g.DrawRectangle(Pens.Black, this.FFisherDistribution);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.nSamples = (int)((NumericUpDown)sender).Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            resetGraphics();
            List<double> Cauchy = new List<double>(),
                TStudent = new List<double>(), 
                FFisher = new List<double>(), 
                Normal = new List<double>(),
                ChiSquared = new List<double>();
            for (int i = 0; i < nSamples; ++i)
            {
                Tuple<double, double> generatedPoint = PolarRNG.NextPolar();
                Normal.Add(generatedPoint.Item1);
                ChiSquared.Add(Math.Pow(generatedPoint.Item1, 2));
                Cauchy.Add(generatedPoint.Item1 / generatedPoint.Item2);
                FFisher.Add(Math.Pow(generatedPoint.Item1, 2) / Math.Pow(generatedPoint.Item2, 2));
                TStudent.Add(generatedPoint.Item1 / Math.Pow(generatedPoint.Item2, 2));
            }

            double CAverage = Cauchy.Average();
            Cauchy = Cauchy.Where((value) => (value >= CAverage - 50 && value <= CAverage + 50)).ToList();

            TStudent = TStudent.Where((value) => (value >= - 50 && value <= 50)).ToList();
            FFisher = FFisher.Where((value) => (value <= 50)).ToList();

            plotDistribution(Normal, this.NormalDistribution);
            plotDistribution(ChiSquared, this.ChiSquaredDistribution);
            plotDistribution(Cauchy, this.CauchyDistribution);
            plotDistribution(FFisher, this.FFisherDistribution);
            plotDistribution(TStudent, this.TStudentDistribution);
            pictureBox1.Image = b;
        }

        private void plotDistribution(List<double> XY, Rectangle R)
        {
            double min = XY.Min(), max = XY.Max();
            double delta = (max-min) / nIntervals;
            List<Interval> intervalList = new List<Interval>();
            for (int i = 0; i < nIntervals - 1; ++i)
            {
                intervalList.Add(new Interval(min, min + delta, false, true));
                min += delta;
            }
            intervalList.Add(new Interval(min, max, false, false));

            foreach (double t in XY)
            {
                foreach (Interval i in intervalList)
                {
                    if (i.includes(t))
                    {
                        i.incrementCount();
                        break;
                    }
                }
            }

            List<RectangleF> rectangles = new List<RectangleF>();
            Point minHisto = new Point(0, 0);
            Point maxHisto = new Point(nIntervals, intervalList.Select((interval) => interval.getCount()).Max());

            for (int i = 0; i < nIntervals; ++i)
            {
                int counter = intervalList[i].getCount();
                Tuple<double, double> mappedX = fromRealToVirtual(new Tuple<double, double>(i, counter), minHisto, maxHisto, R);
                rectangles.Add(new RectangleF((float)mappedX.Item1, (float)mappedX.Item2, 10, R.Bottom - (float)mappedX.Item2));
            }
            g.FillRectangles(Brushes.BlueViolet, rectangles.ToArray());
        }



        private Tuple<double, double> fromRealToVirtual(Tuple<double, double> XY, Point min, Point max, Rectangle r)
        {
            double newX = max.X - min.X == 0 ? 0 : (r.Left + r.Width * (XY.Item1 - min.X) / (max.X - min.X));
            double newY = max.Y - min.Y == 0 ? 0 : (r.Top + r.Height - r.Height * (XY.Item2 - min.Y) / (max.Y - min.Y));
            return new Tuple<double, double>(newX, newY);
        }
    }
}