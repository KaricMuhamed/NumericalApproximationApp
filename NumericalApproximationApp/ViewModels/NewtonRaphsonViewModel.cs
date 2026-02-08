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
    public class NewtonRaphsonViewModel : INotifyPropertyChanged
    {
        private string _functionInput;
        private string _x0Input;
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

        public string X0Input
        {
            get => _x0Input;
            set { _x0Input = value; OnPropertyChanged(); }
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

        public ObservableCollection<NewtonRaphsonIteration> Iterations { get; set; }
        public ICommand SolveCommand { get; }

        public NewtonRaphsonViewModel()
        {
            Iterations = new ObservableCollection<NewtonRaphsonIteration>();
            SolveCommand = new Command(OnSolve);

            FunctionInput = "x^2 - 4";
            X0Input = "5";
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

                double x0 = double.Parse(X0Input, CultureInfo.InvariantCulture);
                double tolerance = double.Parse(ToleranceInput, CultureInfo.InvariantCulture);
                int maxIterations = int.Parse(MaxIterationsInput);

                Func<double, double> function = ParserService.Parse(FunctionInput);
                Func<double, double> derivative = ParserService.NumericalDerivative(function);

                var newton = new NewtonRaphson
                {
                    X0 = x0,
                    Tolerance = tolerance,
                    MaxIterations = maxIterations,
                    Function = function,
                    Derivative = derivative
                };

                var results = newton.Solve();

                foreach (var iteration in results)
                {
                    Iterations.Add(iteration);
                }

                var last = results.Last();
                double root = last.X - last.FX / last.FPrimeX;
                ResultText = $"Nula funkcije ≈ {root:F6}";

                UpdateChart(function, x0, root);
            }
            catch (Exception ex)
            {
                ResultText = $"Greška: {ex.Message}";
            }
        }

        private void UpdateChart(Func<double, double> function, double x0, double root)
        {
            var functionPoints = new List<ObservablePoint>();

            double min = Math.Min(x0, root) - 2;
            double max = Math.Max(x0, root) + 2;
            double step = (max - min) / 200;

            for (double x = min; x <= max; x += step)
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
                new ObservablePoint(min, 0),
                new ObservablePoint(max, 0)
            };

            var rootPoints = new List<ObservablePoint>
            {
                new ObservablePoint(root, 0)
            };

            // Tačke iteracija
            var iterationPoints = new List<ObservablePoint>();
            foreach (var iter in Iterations)
            {
                iterationPoints.Add(new ObservablePoint(iter.X, iter.FX));
            }

            ChartSeries = new ISeries[]
            {
                new LineSeries<ObservablePoint>
                {
                    Values = functionPoints,
                    Fill = null,
                    GeometrySize = 0,
                    Stroke = new SolidColorPaint(new SKColor(139, 92, 246)) { StrokeThickness = 3 },
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
                    Values = iterationPoints,
                    GeometrySize = 10,
                    Stroke = new SolidColorPaint(new SKColor(245, 158, 11)) { StrokeThickness = 2 },
                    Fill = new SolidColorPaint(new SKColor(245, 158, 11)),
                    Name = "Iteracije",
                    AnimationsSpeed = TimeSpan.FromMilliseconds(600)
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