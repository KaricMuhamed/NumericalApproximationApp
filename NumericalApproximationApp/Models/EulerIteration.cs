namespace NumericalApproximationApp.Models
{
    public class EulerIteration
    {
        public int Step { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Slope { get; set; }
        public double DeltaY { get; set; }
    }
}