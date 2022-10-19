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
        public bool openedLeft, openedRight;
        public int valueCount;

        public Interval(double min, double max, bool openedLeft, bool openedRight)
        {
            this.min = min;
            this.max = max;
            this.openedLeft = openedLeft;
            this.openedRight = openedRight;
            this.valueCount = 0;
            this.value = null;
        }

        public Interval(double? min, double? max, string? value, bool openedLeft, bool openedRight, int valueCount)
        {
            this.min = min;
            this.max = max;
            this.value = value;
            this.openedLeft = openedLeft;
            this.openedRight = openedRight;
            this.valueCount = valueCount;
        }

        public Interval(string value)
        {
            this.value = value;
            this.min = this.max = null;
            this.openedLeft = this.openedRight = false;
        }

        public bool includes(object o)
        {
            if(o.GetType() == typeof(double))
            {
                return this.includes((double)o);
            }
            return this.includes((string)o);
        }

        private bool includes(double value)
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

        private bool includes(string value)
        {
            return (this.min == null && this.max == null) && this.value == value;
        }

        public void incrementCount()
        {
            ++this.valueCount;
        }

        public int getCount()
        {
            return this.valueCount;
        }

        public string getName()
        {
            if(this.min == null && this.max == null && this.value != null)
            {
                //intervalName is value
                return this.value;
            }
            return this.min.ToString() + "-" + this.max.ToString();
        }
    }
}
