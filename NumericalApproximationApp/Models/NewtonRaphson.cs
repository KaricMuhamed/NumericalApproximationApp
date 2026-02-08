namespace NumericalApproximationApp.Models
{
    public class NewtonRaphson
    {
        public double X0 { get; set; }
        public double Tolerance { get; set; }
        public int MaxIterations { get; set; }
        public Func<double, double> Function { get; set; }
        public Func<double, double> Derivative { get; set; }

        public List<NewtonRaphsonIteration> Solve()
        {
            var iterations = new List<NewtonRaphsonIteration>();
            double x = X0;

            for (int i = 1; i <= MaxIterations; i++)
            {
                double fx = Function(x);
                double fpx = Derivative(x);

                if (Math.Abs(fpx) < 1e-12)
                    throw new Exception("Derivacija je nula. Metoda ne može nastaviti.");

                double xNew = x - fx / fpx;
                double error = Math.Abs(xNew - x);

                iterations.Add(new NewtonRaphsonIteration
                {
                    Step = i,
                    X = x,
                    FX = fx,
                    FPrimeX = fpx,
                    Error = error
                });

                if (error < Tolerance || Math.Abs(fx) < Tolerance)
                    break;

                x = xNew;
            }

            return iterations;
        }
    }
}