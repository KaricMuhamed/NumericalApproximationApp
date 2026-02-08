namespace NumericalApproximationApp.Models
{
    public class SimpsonRule
    {
        public double A { get; set; }
        public double B { get; set; }
        public int N { get; set; }
        public Func<double, double> Function { get; set; }

        public (double Result, List<SimpsonIteration> Iterations) Solve()
        {
            if (N % 2 != 0)
                throw new ArgumentException("n mora biti paran broj.");

            var iterations = new List<SimpsonIteration>();
            double h = (B - A) / N;
            double sum = 0;

            for (int i = 0; i <= N; i++)
            {
                double x = A + i * h;
                double fx = Function(x);
                double weight;

                if (i == 0 || i == N)
                    weight = 1;
                else if (i % 2 != 0)
                    weight = 4;
                else
                    weight = 2;

                double contribution = weight * fx * h / 3.0;
                sum += contribution;

                iterations.Add(new SimpsonIteration
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