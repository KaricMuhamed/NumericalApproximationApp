namespace NumericalApproximationApp.Models
{
    public class TrapezoidalIteration
    {
        public int Step { get; set; }
        public double X { get; set; }
        public double FX { get; set; }
        public double Weight { get; set; }
        public double Contribution { get; set; }
    }
}