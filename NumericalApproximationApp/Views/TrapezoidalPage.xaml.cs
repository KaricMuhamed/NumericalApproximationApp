using NumericalApproximationApp.Helpers;
using NumericalApproximationApp.Services;
using NumericalApproximationApp.ViewModels;

namespace NumericalApproximationApp.Views
{
    public partial class TrapezoidalPage : ContentPage
    {
        private readonly OCRService _ocrService;

        public TrapezoidalPage(OCRService ocrService)
        {
            InitializeComponent();
            _ocrService = ocrService;
            BindingContext = new TrapezoidalViewModel();
        }

        private async void OnCameraTapped(object sender, EventArgs e)
        {
            string result = await CameraHelper.ScanFunctionAsync(_ocrService, this);

            if (!string.IsNullOrEmpty(result))
            {
                var vm = BindingContext as TrapezoidalViewModel;
                if (vm != null)
                {
                    vm.FunctionInput = result;
                }
            }
        }
    }
}