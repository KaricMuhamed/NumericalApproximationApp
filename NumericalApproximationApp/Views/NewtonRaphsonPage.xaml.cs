using NumericalApproximationApp.Helpers;
using NumericalApproximationApp.Services;
using NumericalApproximationApp.ViewModels;

namespace NumericalApproximationApp.Views
{
    public partial class NewtonRaphsonPage : ContentPage
    {
        private readonly OCRService _ocrService;

        public NewtonRaphsonPage(OCRService ocrService)
        {
            InitializeComponent();
            _ocrService = ocrService;
            BindingContext = new NewtonRaphsonViewModel();
        }

        private async void OnCameraTapped(object sender, EventArgs e)
        {
            string result = await CameraHelper.ScanFunctionAsync(_ocrService, this);

            if (!string.IsNullOrEmpty(result))
            {
                var vm = BindingContext as NewtonRaphsonViewModel;
                if (vm != null)
                {
                    vm.FunctionInput = result;
                }
            }
        }
    }
}