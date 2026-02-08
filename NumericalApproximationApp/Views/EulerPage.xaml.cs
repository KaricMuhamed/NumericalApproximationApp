using NumericalApproximationApp.Helpers;
using NumericalApproximationApp.Services;
using NumericalApproximationApp.ViewModels;

namespace NumericalApproximationApp.Views
{
    public partial class EulerPage : ContentPage
    {
        private readonly OCRService _ocrService;

        public EulerPage(OCRService ocrService)
        {
            InitializeComponent();
            _ocrService = ocrService;
            BindingContext = new EulerViewModel();
        }

        private async void OnCameraTapped(object sender, EventArgs e)
        {
            string result = await CameraHelper.ScanFunctionAsync(_ocrService, this);

            if (!string.IsNullOrEmpty(result))
            {
                var vm = BindingContext as EulerViewModel;
                if (vm != null)
                {
                    vm.FunctionInput = result;
                }
            }
        }
    }
}