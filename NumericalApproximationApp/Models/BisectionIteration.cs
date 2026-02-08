using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalApproximationApp.Models
{
    public class BisectionIteration
    {
        public int Step { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double Midpoint { get; set; }
        public double FMidpoint { get; set; }
        public double Error { get; set; }
    }
}
