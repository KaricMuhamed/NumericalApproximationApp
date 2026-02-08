using NumericalApproximationApp.Helpers;
using NumericalApproximationApp.Services;
using NumericalApproximationApp.ViewModels;

namespace NumericalApproximationApp.Views
{
    public partial class NumericalDerivativePage : ContentPage
    {
        private readonly OCRService _ocrService;

        public NumericalDerivativePage(OCRService ocrService)
        {
            InitializeComponent();
            _ocrService = ocrService;
            BindingContext = new NumericalDerivativeViewModel();
        }

        private async void OnCameraTapped(object sender, EventArgs e)
        {
            string result = await CameraHelper.ScanFunctionAsync(_ocrService, this);

            if (!string.IsNullOrEmpty(result))
            {
                var vm = BindingContext as NumericalDerivativeViewModel;
                if (vm != null)
                {
                    vm.FunctionInput = result;
                }
            }
        }
    }
}