namespace NumericalApproximationApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnBisectionTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//BisectionPage");
        }

        private async void OnNewtonRaphsonTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//NewtonRaphsonPage");
        }

        private async void OnTrapezoidalTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TrapezoidalPage");
        }

        private async void OnSimpsonTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SimpsonPage");
        }

        private async void OnDerivativeTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//NumericalDerivativePage");
        }

        private async void OnLagrangeTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LagrangePage");
        }

        private async void OnEulerTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//EulerPage");
        }

        private async void OnDataAnalysisTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//DataAnalysisPage");
        }
    }
}