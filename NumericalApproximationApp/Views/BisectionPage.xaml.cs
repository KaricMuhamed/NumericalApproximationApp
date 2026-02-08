using NumericalApproximationApp.Helpers;
using NumericalApproximationApp.Services;
using NumericalApproximationApp.ViewModels;

namespace NumericalApproximationApp.Views
{
    public partial class BisectionPage : ContentPage
    {
        private readonly OCRService _ocrService;

        public BisectionPage(OCRService ocrService)
        {
            InitializeComponent();
            _ocrService = ocrService;
            BindingContext = new BisectionViewModel();
        }

        private async void OnCameraTapped(object sender, EventArgs e)
        {
            string result = await CameraHelper.ScanFunctionAsync(_ocrService, this);

            if (!string.IsNullOrEmpty(result))
            {
                var vm = BindingContext as BisectionViewModel;
                if (vm != null)
                {
                    vm.FunctionInput = result;
                }
            }
        }
    }
}