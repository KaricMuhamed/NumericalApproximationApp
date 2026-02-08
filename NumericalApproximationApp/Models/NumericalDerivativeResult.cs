namespace NumericalApproximationApp.Models
{
    public class NumericalDerivativeResult
    {
        public int Step { get; set; }
        public double H { get; set; }
        public double Derivative { get; set; }
        public double Error { get; set; }
    }
}