using Microsoft.Win32;
using System.Collections.Generic;

namespace CoinToss
{
    public partial class Form1 : Form
    {
        private Bitmap b;
        private Graphics g;
        private Random r;
        private Pen penRelative, penAbsolute, penNormalized;
        private int trialsCount, sequencesCount;

        private const double failureProbability = 0.5;

        public Form1()
        {
            InitializeComponent();
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.r = new Random();
            this.numericUpDown1.Minimum = 2;
            this.numericUpDown1.Maximum = 1000000;
            this.numericUpDown1.Value = 2;
            this.numericUpDown2.Minimum = 2;
            this.numericUpDown2.Maximum = 1000000;
            this.numericUpDown2.Value = 2;
            this.trialsCount = 2;
            this.penRelative = new Pen(Color.OrangeRed, 2);
            this.penAbsolute = new Pen(Color.Blue, 2);
            this.penNormalized = new Pen(Color.Gray, 2);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.g = Graphics.FromImage(this.b);
            this.g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g.Clear(Color.White);
            this.pictureBox1.Image = b;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            this.sequencesCount = Convert.ToInt32(Math.Floor(((NumericUpDown)sender).Value));
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.trialsCount = Convert.ToInt32(Math.Floor(((NumericUpDown)sender).Value));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.g.Clear(Color.White);

            Point min = new Point(0, 0), max = new Point(trialsCount, trialsCount);


            Rectangle virtualWindow = new Rectangle(20, 20, this.b.Width - 40, this.b.Height - 40);

            g.DrawRectangle(Pens.Black, virtualWindow);

            /*Legend*/
            Rectangle Legend = new Rectangle(30, 30, 150, 110);
            g.DrawRectangle(Pens.Black, Legend);

            Rectangle legendText = new Rectangle(75, 35, 60, 18);
            g.DrawString("Legend:", new Font("Tahoma", 10), Brushes.Black, legendText);

            Rectangle firstItem = new Rectangle(40, 55, 15, 15);
            g.FillRectangle(Brushes.OrangeRed, firstItem);

            Rectangle firstText = new Rectangle(firstItem.Right + 3, firstItem.Top+2, 100, firstItem.Height+3);
            g.DrawString("Relative Frequency", new Font("Tahoma", 8), Brushes.Black, firstText);

            Rectangle secondItem = new Rectangle(40, firstItem.Bottom + 10, 15, 15);
            g.FillRectangle(Brushes.Blue, secondItem);

            Rectangle secondText = new Rectangle(secondItem.Right + 3, secondItem.Top + 2, 100, secondItem.Height + 3);
            g.DrawString("Absolute Frequency", new Font("Tahoma", 8), Brushes.Black, secondText);

            Rectangle thirdItem = new Rectangle(40, secondItem.Bottom + 10, 15, 15);
            g.FillRectangle(Brushes.Gray, thirdItem);

            Rectangle thirdText = new Rectangle(thirdItem.Right + 3, thirdItem.Top + 2, 150, thirdItem.Height + 3);
            g.DrawString("Normalized Frequency", new Font("Tahoma", 8), Brushes.Black, thirdText);

            Rectangle absoluteHistogram = new Rectangle(Legend.Right + 20, Legend.Top, 250, 200);
            g.DrawRectangle(Pens.Black, absoluteHistogram);

            Rectangle relativeHistogram = new Rectangle(absoluteHistogram.Right + 20, absoluteHistogram.Top, absoluteHistogram.Width, absoluteHistogram.Height);
            g.DrawRectangle(Pens.Black, relativeHistogram);

            Rectangle normalizedHistogram = new Rectangle(absoluteHistogram.Left, relativeHistogram.Bottom + 20, relativeHistogram.Width, relativeHistogram.Height);
            g.DrawRectangle(Pens.Black, normalizedHistogram);

            List<float> absoluteDistribution = new List<float>();
            List<float> relativeDistribution = new List<float>();
            List<float> normalizedDistribution = new List<float>();

            for (int i = 0; i < sequencesCount; i++)
            {
                List<PointF> absoluteFrequency = new List<PointF>();
                List<PointF> relativeFrequency = new List<PointF>();
                List<PointF> normalizedFrequency = new List<PointF>();
                int Y = 0;

                for (int X = 1; X <= trialsCount; ++X)
                {
                    double Uniform = r.NextDouble();
                    if (Uniform < failureProbability)
                    {
                        ++Y;
                    }
                    float ratioRelative = ((float)Y) / X;
                    float ratioNormalized = (float)(((double)Y) / Math.Sqrt(X));
                    absoluteFrequency.Add(fromRealToVirtual(new PointF(X, Y), min, max, virtualWindow));
                    relativeFrequency.Add(fromRealToVirtual(new PointF((float)X, ratioRelative), min, max, virtualWindow));
                    normalizedFrequency.Add(fromRealToVirtual(new PointF(X, ratioNormalized), min, max, virtualWindow));
                }
                absoluteDistribution.Add(absoluteFrequency.Last().Y);
                relativeDistribution.Add(relativeFrequency.Last().Y);
                normalizedDistribution.Add(normalizedFrequency.Last().Y);
                g.DrawLines(penAbsolute, absoluteFrequency.ToArray());
                g.DrawLines(penRelative, relativeFrequency.ToArray());
                g.DrawLines(penNormalized, normalizedFrequency.ToArray());
            }

            plotDistribution(buildIntervals(relativeDistribution), relativeHistogram, Brushes.OrangeRed);
            plotDistribution(buildIntervals(normalizedDistribution), normalizedHistogram, Brushes.Gray);
            plotDistribution(buildIntervals(absoluteDistribution), absoluteHistogram, Brushes.Blue);
            this.pictureBox1.Image = b;
        }

        private PointF fromRealToVirtual(PointF XY, Point min, Point max, Rectangle r)
        {
            float newX = max.X - min.X == 0 ? 0 : (r.Left + r.Width * (XY.X - min.X) / (max.X - min.X));
            float newY = max.Y - min.Y == 0 ? 0 : (r.Top + r.Height - r.Height * (XY.Y - min.Y) / (max.Y - min.Y));
            return new PointF(newX, newY);
        }

        private List<Interval> buildIntervals(List<float> variable)
        {
            List<Interval> intervalList = new List<Interval>();
            double min = variable.Min(), max = variable.Max();
            double delta = (max - min) / 5;
            for(int i = 0; i < 4; ++i)
            {
                intervalList.Add(new Interval(min, min + delta, false, true));
                min += delta;
            }
            intervalList.Add(new Interval(min, max, false, false));
            foreach(float f in variable)
            {
                foreach(Interval interval in intervalList)
                {
                    if (interval.includes((double)f))
                    {
                        interval.incrementCount();
                        break;
                    }
                }
            }
            return intervalList;
        }

        private PointF fromRealToHisto(int count, int intervalN, Rectangle r)
        {
            float X = r.Left + r.Width * (intervalN / 10);
            double ratio = r.Height * ((double)count / sequencesCount);
            float Y = r.Top + r.Height - (float)ratio;
            return new PointF(X, Y);
        }

        private void plotDistribution(List<Interval> intervals, Rectangle rect, Brush b) {

            List<Rectangle> rects = new List<Rectangle>();

            Point min = new Point(0, 0), max = new Point(10, intervals.Select(interval=>interval.getCount()).Max());
            int i = 1;
            foreach(Interval interval in intervals)
            {
                PointF midPoint = fromRealToVirtual(new PointF(i, interval.getCount()), min, max, rect);
                RectangleF r = new RectangleF(midPoint.X - 28 + 20 * i + 10, midPoint.Y + 20, 20,rect.Bottom - midPoint.Y -20);
                ++i;
                g.FillRectangle(b, r);
            }
            
        }
    }
}