using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVParser
{

    public class BivariateInterval
    {
        private Interval referenceInterval;
        private List<Interval> intervals;

        public BivariateInterval(Interval referenceInterval, List<Interval> intervals)
        {
            this.referenceInterval = referenceInterval;
            this.intervals = intervals;
        }

        public bool includes(object prop1, object prop2)
        {
            if (referenceInterval.includes(prop1))
            {
                foreach (Interval interval in intervals)
                {
                    if (interval.includes(prop2))
                    {
                        interval.incrementCount();
                        return true;
                    }
                }
            }
            return false;
        }

        public Dictionary<string, List<Tuple<string, int>>> getInvervalDetails()
        {
            List<Tuple<string, int>> pairs = new List<Tuple<string, int>>();
            for (int i = 0; i < intervals.Count; ++i)
            {
                Interval current = intervals.ElementAt(i);
                pairs.Add(new Tuple<string, int>(current.getName(), current.getCount()));
            }
            Dictionary<string, List<Tuple<string, int>>> keyValuePairs = new Dictionary<string, List<Tuple<string, int>>>();
            keyValuePairs.Add(this.referenceInterval.getName(), pairs);
            return keyValuePairs;
        }
    }
}
