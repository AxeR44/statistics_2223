using System.Reflection;

namespace InterArrivalTimes
{
    public partial class Form1 : Form
    {
        private Bitmap b;
        private Graphics g;
        private Random r;
        private Pen penRelative, penAbsolute, penNormalized;
        private int trialsCount, sequencesCount;
        private int lambda;

        private double failureProbability;

        public Form1()
        {
            InitializeComponent();
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.r = new Random();
            this.trialsCount = 2;
            this.numericUpDown1.Minimum = 2;
            this.numericUpDown1.Maximum = 1000000;
            this.numericUpDown1.Value = 2;
            this.numericUpDown2.Minimum = 2;
            this.numericUpDown2.Maximum = 1000000;
            this.numericUpDown2.Value = 2;
            this.numericUpDown3.Value = 1;
            this.numericUpDown2.Minimum = 1;
            this.numericUpDown3.Maximum = this.trialsCount-1;
            this.lambda = 1;
            this.failureProbability = (double)lambda / (double)trialsCount;
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
            this.numericUpDown3.Maximum = this.trialsCount - 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.g.Clear(Color.White);

            Point min = new Point(0, 0), max = new Point(trialsCount, trialsCount);


            Rectangle virtualWindow = new Rectangle(20, 20, this.b.Width - 400, this.b.Height - 40);

            Rectangle vW2 = new Rectangle( virtualWindow.Right, virtualWindow.Top, this.b.Width - virtualWindow.Width - 40, this.b.Height - 40);

            g.DrawRectangle(Pens.Black, virtualWindow);
            g.DrawRectangle(Pens.Black, vW2);

            /*Legend*/
            Rectangle Legend = new Rectangle(30, 30, 150, 110);
            g.DrawRectangle(Pens.Black, Legend);

            Rectangle legendText = new Rectangle(75, 35, 60, 18);
            g.DrawString("Legend:", new Font("Tahoma", 10), Brushes.Black, legendText);

            Rectangle firstItem = new Rectangle(40, 55, 15, 15);
            g.FillRectangle(Brushes.OrangeRed, firstItem);

            Rectangle firstText = new Rectangle(firstItem.Right + 3, firstItem.Top + 2, 100, firstItem.Height + 3);
            g.DrawString("Relative Frequency", new Font("Tahoma", 8), Brushes.Black, firstText);

            Rectangle secondItem = new Rectangle(40, firstItem.Bottom + 10, 15, 15);
            g.FillRectangle(Brushes.Blue, secondItem);

            Rectangle secondText = new Rectangle(secondItem.Right + 3, secondItem.Top + 2, 100, secondItem.Height + 3);
            g.DrawString("Absolute Frequency", new Font("Tahoma", 8), Brushes.Black, secondText);

            Rectangle thirdItem = new Rectangle(40, secondItem.Bottom + 10, 15, 15);
            g.FillRectangle(Brushes.Gray, thirdItem);

            Rectangle thirdText = new Rectangle(thirdItem.Right + 3, thirdItem.Top + 2, 150, thirdItem.Height + 3);
            g.DrawString("Normalized Frequency", new Font("Tahoma", 8), Brushes.Black, thirdText);

            List<float> absoluteDistribution = new List<float>();
            List<float> relativeDistribution = new List<float>();
            List<float> normalizedDistribution = new List<float>();
            Dictionary<int, int> interArrivalTimes = new Dictionary<int, int>();

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
                if(!interArrivalTimes.ContainsKey(Y))
                {
                    interArrivalTimes.Add(Y, trialsCount - Y);
                }
                else
                {
                    interArrivalTimes[Y] += (trialsCount - Y);
                }
                
                absoluteDistribution.Add(absoluteFrequency.Last().Y);
                relativeDistribution.Add(relativeFrequency.Last().Y);
                normalizedDistribution.Add(normalizedFrequency.Last().Y);
                g.DrawLines(penAbsolute, absoluteFrequency.ToArray());
                g.DrawLines(penRelative, relativeFrequency.ToArray());
                g.DrawLines(penNormalized, normalizedFrequency.ToArray());
            }

            drawHistogram(interArrivalTimes, vW2);
            
            this.pictureBox1.Image = b;
        }

        void drawHistogram(Dictionary<int, int> values, Rectangle virtualWindow)
        {
            Pen p = new Pen(Color.Orange, 4);
            int maxK = values.Keys.Max();
            int maxV = values.Values.Max();
            Point maxPoint = new Point(maxV, trialsCount), minPoint = new Point(0, 0);

            foreach(KeyValuePair<int, int> kvp in values)
            {
                PointF mapped = fromRealToVirtual(new PointF(kvp.Value, kvp.Key), minPoint, maxPoint, virtualWindow);
                g.DrawLine(p, 
                    new PointF(virtualWindow.Left, mapped.Y),
                    mapped);
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            this.lambda = Convert.ToInt32(Math.Floor(((NumericUpDown)sender).Value));
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
            for (int i = 0; i < 4; ++i)
            {
                intervalList.Add(new Interval(min, min + delta, false, true));
                min += delta;
            }
            intervalList.Add(new Interval(min, max, false, false));
            foreach (float f in variable)
            {
                foreach (Interval interval in intervalList)
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
    }
}