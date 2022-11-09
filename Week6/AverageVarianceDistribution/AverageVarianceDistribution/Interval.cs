using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace AverageVarianceDistribution
{
    public class Interval
    {
        public double min, max;
        private bool openedLeft, openedRight;
        private int valueCount;

        public Interval(double min, double max, bool openedLeft, bool openedRight)
        {
            this.min = min;
            this.max = max;
            this.openedLeft = openedLeft;
            this.openedRight = openedRight;
            this.valueCount = 0;
        }

        public bool includes(double value)
        {
            return openedLeft && openedRight ? value > min && value < max :
            (!openedLeft && !openedRight ? value >= min && value <= max :
            (!openedLeft && openedRight ? value >= min && value < max :
            value > min && value <= max));
        }

        public void incrementCount()
        {
            ++this.valueCount;
        }

        public int getCount()
        {
            return this.valueCount;
        }
    }
}
