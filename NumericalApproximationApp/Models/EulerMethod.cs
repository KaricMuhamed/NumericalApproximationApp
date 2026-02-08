namespace NumericalApproximationApp.Models
{
    public class EulerMethod
    {
        public double X0 { get; set; }
        public double Y0 { get; set; }
        public double XEnd { get; set; }
        public double H { get; set; }
        public Func<double, double, double> Function { get; set; }

        public List<EulerIteration> Solve()
        {
            var iterations = new List<EulerIteration>();
            double x = X0;
            double y = Y0;
            int step = 0;

            iterations.Add(new EulerIteration
            {
                Step = step,
                X = x,
                Y = y,
                Slope = Function(x, y),
                DeltaY = 0
            });

            while (x < XEnd - H / 2.0)
            {
                double slope = Function(x, y);
                double deltaY = H * slope;
                y = y + deltaY;
                x = x + H;
                step++;

                iterations.Add(new EulerIteration
                {
                    Step = step,
                    X = x,
                    Y = y,
                    Slope = slope,
                    DeltaY = deltaY
                });
            }

            return iterations;
        }
    }
}