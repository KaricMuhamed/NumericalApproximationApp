using NumericalApproximationApp.ViewModels;

namespace NumericalApproximationApp.Views
{
    public partial class DataAnalysisPage : ContentPage
    {
        public DataAnalysisPage()
        {
            InitializeComponent();
            BindingContext = new DataAnalysisViewModel();
        }
    }
}