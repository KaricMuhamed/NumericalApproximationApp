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
    public class BisectionViewModel : INotifyPropertyChanged
    {
        private string _functionInput;
        private string _aInput;
        private string _bInput;
        private string _toleranceInput;
        private string _maxIterationsInput;
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

        public string ToleranceInput
        {
            get => _toleranceInput;
            set { _toleranceInput = value; OnPropertyChanged(); }
        }

        public string MaxIterationsInput
        {
            get => _maxIterationsInput;
            set { _maxIterationsInput = value; OnPropertyChanged(); }
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

        public ObservableCollection<BisectionIteration> Iterations { get; set; }
        public ICommand SolveCommand { get; }

        public BisectionViewModel()
        {
            Iterations = new ObservableCollection<BisectionIteration>();
            SolveCommand = new Command(OnSolve);

            FunctionInput = "x^2 - 4";
            AInput = "0";
            BInput = "5";
            ToleranceInput = "0.0001";
            MaxIterationsInput = "100";

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
                double tolerance = double.Parse(ToleranceInput, CultureInfo.InvariantCulture);
                int maxIterations = int.Parse(MaxIterationsInput);

                Func<double, double> function = ParserService.Parse(FunctionInput);

                var bisection = new Bisection
                {
                    A = a,
                    B = b,
                    Tolerance = tolerance,
                    MaxIterations = maxIterations,
                    Function = function
                };

                var results = bisection.Solve();

                foreach (var iteration in results)
                {
                    Iterations.Add(iteration);
                }

                var last = results.Last();
                ResultText = $"Nula funkcije ≈ {last.Midpoint:F6}";

                UpdateChart(function, a, b, last.Midpoint);
            }
            catch (Exception ex)
            {
                ResultText = $"Greška: {ex.Message}";
            }
        }

        private void UpdateChart(Func<double, double> function, double a, double b, double root)
        {
            var functionPoints = new List<ObservablePoint>();

            double margin = (b - a) * 0.3;
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

            var zeroLine = new List<ObservablePoint>
            {
                new ObservablePoint(plotA, 0),
                new ObservablePoint(plotB, 0)
            };

            var rootPoints = new List<ObservablePoint>
            {
                new ObservablePoint(root, 0)
            };

            ChartSeries = new ISeries[]
            {
                new LineSeries<ObservablePoint>
                {
                    Values = functionPoints,
                    Fill = null,
                    GeometrySize = 0,
                    Stroke = new SolidColorPaint(new SKColor(59, 130, 246)) { StrokeThickness = 3 },
                    Name = "f(x)",
                    LineSmoothness = 0.5,
                    AnimationsSpeed = TimeSpan.FromMilliseconds(800)
                },
                new LineSeries<ObservablePoint>
                {
                    Values = zeroLine,
                    Fill = null,
                    GeometrySize = 0,
                    Stroke = new SolidColorPaint(new SKColor(148, 163, 184)) { StrokeThickness = 1 },
                    Name = "y = 0",
                    AnimationsSpeed = TimeSpan.FromMilliseconds(500)
                },
                new ScatterSeries<ObservablePoint>
                {
                    Values = rootPoints,
                    GeometrySize = 16,
                    Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 3 },
                    Fill = new SolidColorPaint(new SKColor(16, 185, 129)),
                    Name = $"Nula ≈ {root:F4}",
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