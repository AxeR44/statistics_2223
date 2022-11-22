using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherDistributions
{
    internal class RandomPolar
    {
        private Random RNG;
        private const double PI2 = Math.PI * 2;

        public RandomPolar()
        {
            this.RNG = new Random();
        }

        /*On a goniometric circumference*/
        public Tuple<double, double> NextPolar()
        {
            double radius = Math.Sqrt(-2*Math.Log(this.RNG.NextDouble()));
            double angle = this.RNG.NextDouble() * PI2;

            return new Tuple<double, double>(radius * Math.Cos(angle), radius * Math.Sin(angle));
        }
    }
}
