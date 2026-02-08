namespace NumericalApproximationApp.Models
{
    public class TrapezoidalRule
    {
        public double A { get; set; }
        public double B { get; set; }
        public int N { get; set; }
        public Func<double, double> Function { get; set; }

        public (double Result, List<TrapezoidalIteration> Iterations) Solve()
        {
            var iterations = new List<TrapezoidalIteration>();
            double h = (B - A) / N;
            double sum = 0;

            for (int i = 0; i <= N; i++)
            {
                double x = A + i * h;
                double fx = Function(x);
                double weight = (i == 0 || i == N) ? 1 : 2;
                double contribution = weight * fx * h / 2.0;
                sum += contribution;

                iterations.Add(new TrapezoidalIteration
                {
                    Step = i,
                    X = x,
                    FX = fx,
                    Weight = weight,
                    Contribution = contribution
                });
            }

            return (sum, iterations);
        }
    }
}