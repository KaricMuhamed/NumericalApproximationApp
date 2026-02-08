namespace NumericalApproximationApp.Models
{
    public class NumericalDerivative
    {
        public double X { get; set; }
        public double InitialH { get; set; }
        public int Refinements { get; set; }
        public Func<double, double> Function { get; set; }

        public List<NumericalDerivativeResult> Solve()
        {
            var results = new List<NumericalDerivativeResult>();
            double h = InitialH;
            double previousDerivative = double.NaN;

            for (int i = 1; i <= Refinements; i++)
            {
                double derivative = (Function(X + h) - Function(X - h)) / (2 * h);
                double error = double.IsNaN(previousDerivative) ? 0 : Math.Abs(derivative - previousDerivative);

                results.Add(new NumericalDerivativeResult
                {
                    Step = i,
                    H = h,
                    Derivative = derivative,
                    Error = error
                });

                previousDerivative = derivative;
                h /= 10.0;
            }

            return results;
        }
    }
}