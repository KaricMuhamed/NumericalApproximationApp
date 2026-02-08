namespace NumericalApproximationApp.Models
{
    public class NewtonRaphsonIteration
    {
        public int Step { get; set; }
        public double X { get; set; }
        public double FX { get; set; }
        public double FPrimeX { get; set; }
        public double Error { get; set; }
    }
}