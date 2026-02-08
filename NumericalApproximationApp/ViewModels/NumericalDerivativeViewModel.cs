using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using NumericalApproximationApp.Models;
using NumericalApproximationApp.Services;
using SkiaSharp;

namespace NumericalApproximationApp.ViewModels
{
    public class NumericalDerivativeViewModel : INotifyPropertyChanged
    {
        private string _functionInput;
        private string _xInput;
        private string _hInput;
        private string _refinementsInput;
        private string _resultText;
        private ISeries[] _chartSeries;
        private LiveChartsCore.Kernel.Sketches.ICartesianAxis[] _xAxes;
        private LiveChartsCore.Kernel.Sketches.ICartesianAxis[] _yAxes;

        public string FunctionInput
        {
            get => _functionInput;
            set { _functionInput = value; OnPropertyChanged(); }
        }

        public string XInput
        {
            get => _xInput;
            set { _xInput = value; OnPropertyChanged(); }
        }

        public string HInput
        {
            get => _hInput;
            set { _hInput = value; OnPropertyChanged(); }
        }

        public string RefinementsInput
        {
            get => _refinementsInput;
            set { _refinementsInput = value; OnPropertyChanged(); }
        }

        public string ResultText
        {
            get => _resultText;
            set { _resultText = value; OnPropertyChanged(); }
        }

        public ISeries[] ChartSeries
        {
            get => _chartSeries;
            set { _chartSeries = value; OnPropertyChanged(); }
        }

        public LiveChartsCore.Kernel.Sketches.ICartesianAxis[] XAxes
        {
            get => _xAxes;
            set { _xAxes = value; OnPropertyChanged(); }
        }

        public LiveChartsCore.Kernel.Sketches.ICartesianAxis[] YAxes
        {
            get => _yAxes;
            set { _yAxes = value; OnPropertyChanged(); }
        }

        public ObservableCollection<NumericalDerivativeResult> Iterations { get; set; }
        public ICommand SolveCommand { get; }

        public NumericalDerivativeViewModel()
        {
            Iterations = new ObservableCollection<NumericalDerivativeResult>();
            SolveCommand = new Command(OnSolve);

            FunctionInput = "x^3";
            XInput = "2";
            HInput = "0.1";
            RefinementsInput = "8";

            InitializeChart();
        }

        private void InitializeChart()
        {
            ChartSeries = Array.Empty<ISeries>();

            XAxes = new LiveChartsCore.Kernel.Sketches.ICartesianAxis[]
            {
                new Axis
                {
                    Name = "x",
                    NamePaint = new SolidColorPaint(SKColors.White),
                    LabelsPaint = new SolidColorPaint(new SKColor(148, 163, 184)),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(42, 58, 78))
                }
            };

            YAxes = new LiveChartsCore.Kernel.Sketches.ICartesianAxis[]
            {
                new Axis
                {
                    Name = "f(x)",
                    NamePaint = new SolidColorPaint(SKColors.White),
                    LabelsPaint = new SolidColorPaint(new SKColor(148, 163, 184)),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(42, 58, 78))
                }
            };
        }

        private void OnSolve()
        {
            try
            {
                Iterations.Clear();

                double x = double.Parse(XInput, CultureInfo.InvariantCulture);
                double h = double.Parse(HInput, CultureInfo.InvariantCulture);
                int refinements = int.Parse(RefinementsInput);

                Func<double, double> function = ParserService.Parse(FunctionInput);

                var derivative = new NumericalDerivative
                {
                    X = x,
                    InitialH = h,
                    Refinements = refinements,
                    Function = function
                };

                var results = derivative.Solve();

                foreach (var result in results)
                {
                    Iterations.Add(result);
                }

                var last = results.Last();
                ResultText = $"f'({x}) ≈ {last.Derivative:F6}";

                UpdateChart(function, x, last.Derivative);
            }
            catch (Exception ex)
            {
                ResultText = $"Greška: {ex.Message}";
            }
        }

        private void UpdateChart(Func<double, double> function, double x, double slope)
        {
            var functionPoints = new List<ObservablePoint>();
            double range = 4;
            double step = range / 100;

            for (double xi = x - range; xi <= x + range; xi += step)
            {
                try
                {
                    double y = function(xi);
                    if (!double.IsNaN(y) && !double.IsInfinity(y) && Math.Abs(y) < 1000)
                        functionPoints.Add(new ObservablePoint(xi, y));
                }
                catch { }
            }

            // Tangentna linija: y = f(x0) + f'(x0) * (x - x0)
            var tangentPoints = new List<ObservablePoint>();
            double fx = function(x);
            double tangentRange = 2;
            tangentPoints.Add(new ObservablePoint(x - tangentRange, fx + slope * (-tangentRange)));
            tangentPoints.Add(new ObservablePoint(x + tangentRange, fx + slope * (tangentRange)));

            // Tačka derivacije
            var derivPoint = new List<ObservablePoint>
            {
                new ObservablePoint(x, fx)
            };

            ChartSeries = new ISeries[]
            {
                new LineSeries<ObservablePoint>
                {
                    Values = functionPoints,
                    Fill = null,
                    GeometrySize = 0,
                    Stroke = new SolidColorPaint(new SKColor(239, 68, 68)) { StrokeThickness = 3 },
                    Name = "f(x)",
                    LineSmoothness = 0.5,
                    AnimationsSpeed = TimeSpan.FromMilliseconds(800)
                },
                new LineSeries<ObservablePoint>
                {
                    Values = tangentPoints,
                    Fill = null,
                    GeometrySize = 0,
                    Stroke = new SolidColorPaint(new SKColor(245, 158, 11)) { StrokeThickness = 2 },
                    Name = $"Tangenta (nagib = {slope:F2})",
                    LineSmoothness = 0,
                    AnimationsSpeed = TimeSpan.FromMilliseconds(600)
                },
                new ScatterSeries<ObservablePoint>
                {
                    Values = derivPoint,
                    GeometrySize = 14,
                    Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 3 },
                    Fill = new SolidColorPaint(new SKColor(16, 185, 129)),
                    Name = $"Tačka ({x}, {fx:F2})",
                    AnimationsSpeed = TimeSpan.FromMilliseconds(1000)
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}