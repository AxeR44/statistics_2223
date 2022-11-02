namespace HorizontalVerticalHistogram
{
    public partial class Form1 : Form
    {

        Bitmap b;
        Graphics g;
        Rectangle virtualWindow, VirtualLeft, VirtualRight;
        private int trialsCount, sequencesCount;
        private const double failureProbability = 0.5;

        public Form1()
        {
            InitializeComponent();
            this.b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            this.g = Graphics.FromImage(b);
            this.trialsCount = 50;
            this.sequencesCount = 50;
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            Random r = new Random();
            Point min = new Point(0, 0), max = new Point(trialsCount, trialsCount);
            this.virtualWindow = new Rectangle(20, 20, this.b.Width - 40, this.b.Height - 40);
            this.VirtualLeft = new Rectangle(virtualWindow.Left, virtualWindow.Top, (virtualWindow.Width / 2) - 5, virtualWindow.Height);
            this.VirtualRight = new Rectangle(virtualWindow.Left + (virtualWindow.Width / 2) + 5, virtualWindow.Top , (virtualWindow.Width / 2) - 5, virtualWindow.Height);
            this.g.DrawRectangle(Pens.Black, virtualWindow);
            this.g.DrawRectangle(Pens.Black, VirtualLeft);
            this.g.DrawRectangle(Pens.Black, VirtualRight);

            List<float> absoluteDistribution = new List<float>();

            for (int i = 0; i < sequencesCount; i++)
            {
                List<PointF> absoluteFrequency = new List<PointF>();

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

                }
                absoluteDistribution.Add(absoluteFrequency.Last().Y);
            }
            List<Interval> absoluteIntervals = buildIntervals(absoluteDistribution);
            plotDistribution(absoluteIntervals, VirtualLeft, Brushes.Blue, false);
            plotDistribution(absoluteIntervals, VirtualRight, Brushes.Blue, true);
            this.pictureBox1.Image = b;
        }



        private PointF fromRealToVirtual(PointF XY, Point min, Point max, Rectangle r)
        {
            float newX = max.X - min.X == 0 ? 0 : (r.Left + r.Width * (XY.X - min.X) / (max.X - min.X));
            float newY = max.Y - min.Y == 0 ? 0 : (r.Top + r.Height - r.Height * (XY.Y - min.Y) / (max.Y - min.Y));
            return new PointF(newX, newY);
        }

        private PointF fromRealToVirtualVertical(PointF XY, Point min, Point max, Rectangle r)
        {
            float newX = max.X - min.X == 0 ? 0 : (r.Left + r.Width * (XY.X - min.X) / (max.X - min.X));
            float newY = max.Y - min.Y == 0 ? 0 : (r.Top + r.Height  * (XY.Y - min.Y) / (max.Y - min.Y));
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

        private void plotDistribution(List<Interval> intervals, Rectangle rect, Brush b, bool isVertical)
        {

            List<Rectangle> rects = new List<Rectangle>();

            Point min = new Point(0, 0), max = isVertical? new Point(intervals.Select(interval => interval.getCount()).Max(), 10) : new Point(10, intervals.Select(interval => interval.getCount()).Max());
            int i = 1;
            foreach (Interval interval in intervals)
            {
                RectangleF r;
                if (!isVertical)
                {
                    PointF midPoint = fromRealToVirtual(new PointF(i, interval.getCount()), min, max, rect);
                    r = new RectangleF(midPoint.X - 28 + 20 * i + 10, midPoint.Y + 20, 20, rect.Bottom - midPoint.Y - 20);
                }
                else
                {
                    PointF midPoint = fromRealToVirtualVertical(new PointF(0, i), min, max, rect);
                    PointF rectMax = fromRealToVirtualVertical(new PointF(interval.getCount(), 0), min, max, rect);
                    r = new RectangleF(midPoint.X, midPoint.Y - 28 + 20 * i + 10, rectMax.X - rect.Left - 20, 20);
                }
                ++i;
                g.FillRectangle(b, r);
            }

        }
    }
}