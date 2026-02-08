namespace NumericalApproximationApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("BisectionPage", typeof(Views.BisectionPage));
            Routing.RegisterRoute("NewtonRaphsonPage", typeof(Views.NewtonRaphsonPage));
            Routing.RegisterRoute("TrapezoidalPage", typeof(Views.TrapezoidalPage));
            Routing.RegisterRoute("SimpsonPage", typeof(Views.SimpsonPage));
            Routing.RegisterRoute("NumericalDerivativePage", typeof(Views.NumericalDerivativePage));
            Routing.RegisterRoute("LagrangePage", typeof(Views.LagrangePage));
            Routing.RegisterRoute("EulerPage", typeof(Views.EulerPage));
            Routing.RegisterRoute("DataAnalysisPage", typeof(Views.DataAnalysisPage));
        }
    }
}