using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxMullerTransform
{
    public class Interval
    {
        public double min, max;
        private bool openedLeft, openedRight;
        private int valueCountX, valueCountY;

        public Interval(double min, double max, bool openedLeft, bool openedRight)
        {
            this.min = min;
            this.max = max;
            this.openedLeft = openedLeft;
            this.openedRight = openedRight;
            this.valueCountX = this.valueCountY = 0;
        }

        public bool includes(double value)
        {
            return openedLeft && openedRight ? value > min && value < max :
            (!openedLeft && !openedRight ? value >= min && value <= max :
            (!openedLeft && openedRight ? value >= min && value < max :
            value > min && value <= max));
        }

        public void incrementCountX()
        {
            ++this.valueCountX;
        }
        public void incrementCountY()
        {
            ++this.valueCountY;
        }

        public Tuple<int, int> getCount()
        {
            return new Tuple<int, int>(this.valueCountX, this.valueCountY);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.openedLeft ? "(" : "[")
                .Append(this.min)
                .Append(",")
                .Append(this.max)
                .Append(this.openedRight ? ")" : "]");
            return builder.ToString();
        }
    }
}
