using AngouriMath;

namespace NumericalApproximationApp.Services
{
    public static class ParserService
    {
        public static Func<double, double> Parse(string expression)
        {
            Entity expr = expression;
            Entity.Variable x = "x";

            return (double value) =>
            {
                var result = expr.Substitute(x, value).EvalNumerical();
                return (double)result;
            };
        }

        public static Func<double, double> NumericalDerivative(Func<double, double> function, double h = 1e-8)
        {
            return (double x) =>
            {
                return (function(x + h) - function(x - h)) / (2 * h);
            };
        }

        public static Func<double, double, double> ParseTwoVariables(string expression)
        {
            Entity expr = expression;
            Entity.Variable x = "x";
            Entity.Variable y = "y";

            return (double xVal, double yVal) =>
            {
                var result = expr.Substitute(x, xVal).Substitute(y, yVal).EvalNumerical();
                return (double)result;
            };
        }
    }
}