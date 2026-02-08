namespace NumericalApproximationApp.Models
{
    public class LagrangeInterpolation
    {
        public List<double> XPoints { get; set; }
        public List<double> YPoints { get; set; }

        public double Evaluate(double x)
        {
            int n = XPoints.Count;
            double result = 0;

            for (int i = 0; i < n; i++)
            {
                double li = 1;
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        li *= (x - XPoints[j]) / (XPoints[i] - XPoints[j]);
                    }
                }
                result += YPoints[i] * li;
            }

            return result;
        }

        public List<(double X, double Y)> GenerateCurve(double from, double to, int points = 200)
        {
            var curve = new List<(double X, double Y)>();
            double step = (to - from) / points;

            for (double x = from; x <= to; x += step)
            {
                try
                {
                    double y = Evaluate(x);
                    if (!double.IsNaN(y) && !double.IsInfinity(y) && Math.Abs(y) < 10000)
                        curve.Add((x, y));
                }
                catch { }
            }

            return curve;
        }
    }
}