using NumericalApproximationApp.ViewModels;

namespace NumericalApproximationApp.Views
{
    public partial class LagrangePage : ContentPage
    {
        public LagrangePage()
        {
            InitializeComponent();
            BindingContext = new LagrangeViewModel();
        }
    }
}