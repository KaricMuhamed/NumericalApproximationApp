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
    public class SimpsonViewModel : INotifyPropertyChanged
    {
        private string _functionInput;
        private string _aInput;
        private string _bInput;
        private string _nInput;
        private string _resultText;
        private ISeries[] _chartSeries;
        private LiveChartsCore.Kernel.Sketches.ICartesianAxis[] _xAxes;
        private LiveChartsCore.Kernel.Sketches.ICartesianAxis[] _yAxes;

        public string FunctionInput
        {
            get => _functionInput;
            set { _functionInput = value; OnPropertyChanged(); }
        }

        public string AInput
        {
            get => _aInput;
            set { _aInput = value; OnPropertyChanged(); }
        }

        public string BInput
        {
            get => _bInput;
            set { _bInput = value; OnPropertyChanged(); }
        }

        public string NInput
        {
            get => _nInput;
            set { _nInput = value; OnPropertyChanged(); }
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

        public ObservableCollection<SimpsonIteration> Iterations { get; set; }
        public ICommand SolveCommand { get; }

        public SimpsonViewModel()
        {
            Iterations = new ObservableCollection<SimpsonIteration>();
            SolveCommand = new Command(OnSolve);

            FunctionInput = "x^2";
            AInput = "0";
            BInput = "4";
            NInput = "10";

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

                double a = double.Parse(AInput, CultureInfo.InvariantCulture);
                double b = double.Parse(BInput, CultureInfo.InvariantCulture);
                int n = int.Parse(NInput);

                Func<double, double> function = ParserService.Parse(FunctionInput);

                var simpson = new SimpsonRule
                {
                    A = a,
                    B = b,
                    N = n,
                    Function = function
                };

                var (result, iterations) = simpson.Solve();

                foreach (var iteration in iterations)
                {
                    Iterations.Add(iteration);
                }

                ResultText = $"∫f(x)dx ≈ {result:F6}";

                UpdateChart(function, a, b, n);
            }
            catch (Exception ex)
            {
                ResultText = $"Greška: {ex.Message}";
            }
        }

        private void UpdateChart(Func<double, double> function, double a, double b, int n)
        {
            var functionPoints = new List<ObservablePoint>();

            double margin = (b - a) * 0.2;
            double plotA = a - margin;
            double plotB = b + margin;
            double step = (plotB - plotA) / 200;

            for (double x = plotA; x <= plotB; x += step)
            {
                try
                {
                    double y = function(x);
                    if (!double.IsNaN(y) && !double.IsInfinity(y) && Math.Abs(y) < 1000)
                        functionPoints.Add(new ObservablePoint(x, y));
                }
                catch { }
            }

            // Popunjena površina ispod krivulje
            var areaPoints = new List<ObservablePoint>();
            double hArea = (b - a) / 200;
            areaPoints.Add(new ObservablePoint(a, 0));
            for (double x = a; x <= b; x += hArea)
            {
                try
                {
                    double y = function(x);
                    if (!double.IsNaN(y) && !double.IsInfinity(y))
                        areaPoints.Add(new ObservablePoint(x, y));
                }
                catch { }
            }
            areaPoints.Add(new ObservablePoint(b, 0));

            // Tačke podjele
            var divisionPoints = new List<ObservablePoint>();
            double h = (b - a) / n;
            for (int i = 0; i <= n; i++)
            {
                double x = a + i * h;
                try
                {
                    divisionPoints.Add(new ObservablePoint(x, function(x)));
                }
                catch { }
            }

            ChartSeries = new ISeries[]
            {
                new LineSeries<ObservablePoint>
                {
                    Values = areaPoints,
                    Fill = new SolidColorPaint(new SKColor(245, 158, 11, 50)),
                    GeometrySize = 0,
                    Stroke = new SolidColorPaint(new SKColor(245, 158, 11, 80)) { StrokeThickness = 1 },
                    Name = "Površina",
                    LineSmoothness = 0.5,
                    AnimationsSpeed = TimeSpan.FromMilliseconds(600)
                },
                new LineSeries<ObservablePoint>
                {
                    Values = functionPoints,
                    Fill = null,
                    GeometrySize = 0,
                    Stroke = new SolidColorPaint(new SKColor(245, 158, 11)) { StrokeThickness = 3 },
                    Name = "f(x)",
                    LineSmoothness = 0.5,
                    AnimationsSpeed = TimeSpan.FromMilliseconds(800)
                },
                new ScatterSeries<ObservablePoint>
                {
                    Values = divisionPoints,
                    GeometrySize = 8,
                    Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2 },
                    Fill = new SolidColorPaint(new SKColor(6, 182, 212)),
                    Name = "Podjela",
                    AnimationsSpeed = TimeSpan.FromMilliseconds(700)
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