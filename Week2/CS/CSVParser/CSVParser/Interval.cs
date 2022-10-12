using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace CSVParser
{
    public class Interval
    {
        public double? min, max;
        public string? value;
        private bool openedLeft, openedRight;
        private int valueCount;

        public Interval(double min, double max, bool openedLeft, bool openedRight)
        {
            this.min = min;
            this.max = max;
            this.openedLeft = openedLeft;
            this.openedRight = openedRight;
            this.valueCount = 0;
            this.value = null;
        }

        public Interval(string value)
        {
            this.value = value;
            this.min = this.max = null;
            this.openedLeft = this.openedRight = false;
        }

        public bool includes(double value)
        {
            if(min != null && max != null && this.value == null)
            {
                return openedLeft && openedRight ? value > min && value < max :
                (!openedLeft && !openedRight ? value >= min && value <= max :
                (!openedLeft && openedRight ? value >= min && value < max :
                 value > min && value <= max));
            }
            return false;
            
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
