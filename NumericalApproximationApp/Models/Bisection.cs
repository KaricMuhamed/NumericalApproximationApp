using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalApproximationApp.Models
{
    public class Bisection
    {
        public double A { get; set; }
        public double B { get; set; }
        public double Tolerance { get; set; }
        public int MaxIterations { get; set; }
        public Func<double, double> Function { get; set; }

        public List<BisectionIteration> Solve()
        {
            var iterations = new List<BisectionIteration>();

            double a = A;
            double b = B;

            if (Function(a) * Function(b) >= 0)
                throw new ArgumentException("f(a) i f(b) moraju biti različitog predznaka.");

            for (int i = 1; i <= MaxIterations; i++)
            {
                double midpoint = (a + b) / 2.0;
                double fMid = Function(midpoint);
                double error = (b - a) / 2.0;

                iterations.Add(new BisectionIteration
                {
                    Step = i,
                    A = a,
                    B = b,
                    Midpoint = midpoint,
                    FMidpoint = fMid,
                    Error = error
                });

                if (Math.Abs(fMid) < Tolerance || error < Tolerance)
                    break;

                if (Function(a) * fMid < 0)
                    b = midpoint;
                else
                    a = midpoint;
            }

            return iterations;
        }
    }
}
