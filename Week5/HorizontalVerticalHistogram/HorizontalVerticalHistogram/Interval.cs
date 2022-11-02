using System.Text;

namespace HorizontalVerticalHistogram
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
