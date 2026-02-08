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
    public class EulerViewModel : INotifyPropertyChanged
    {
        private string _functionInput;
        private string _x0Input;
        private string _y0Input;
        private string _xEndInput;
        private string _hInput;
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

        public string Y0Input
        {
            get => _y0Input;
            set { _y0Input = value; OnPropertyChanged(); }
        }

        public string XEndInput
        {
            get => _xEndInput;
            set { _xEndInput = value; OnPropertyChanged(); }
        }

        public string HInput
        {
            get => _hInput;
            set { _hInput = value; OnPropertyChanged(); }
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

        public ObservableCollection<EulerIteration> Iterations { get; set; }
        public ICommand SolveCommand { get; }

        public EulerViewModel()
        {
            Iterations = new ObservableCollection<EulerIteration>();
            SolveCommand = new Command(OnSolve);

            FunctionInput = "x + y";
            X0Input = "0";
            Y0Input = "1";
            XEndInput = "2";
            HInput = "0.2";

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
                    Name = "y",
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
                double y0 = double.Parse(Y0Input, CultureInfo.InvariantCulture);
                double xEnd = double.Parse(XEndInput, CultureInfo.InvariantCulture);
                double h = double.Parse(HInput, CultureInfo.InvariantCulture);

                Func<double, double, double> function = ParserService.ParseTwoVariables(FunctionInput);

                var euler = new EulerMethod
                {
                    X0 = x0,
                    Y0 = y0,
                    XEnd = xEnd,
                    H = h,
                    Function = function
                };

                var results = euler.Solve();

                foreach (var iteration in results)
                {
                    Iterations.Add(iteration);
                }

                var last = results.Last();
                ResultText = $"y({last.X:F2}) ≈ {last.Y:F6}";

                UpdateChart(results);
            }
            catch (Exception ex)
            {
                ResultText = $"Greška: {ex.Message}";
            }
        }

        private void UpdateChart(List<EulerIteration> results)
        {
            // Linija rješenja
            var solutionPoints = results
                .Select(r => new ObservablePoint(r.X, r.Y))
                .ToList();

            // Tačke na liniji
            var dotPoints = results
                .Select(r => new ObservablePoint(r.X, r.Y))
                .ToList();

            // Početna tačka
            var startPoint = new List<ObservablePoint>
            {
                new ObservablePoint(results.First().X, results.First().Y)
            };

            // Krajnja tačka
            var endPoint = new List<ObservablePoint>
            {
                new ObservablePoint(results.Last().X, results.Last().Y)
            };

            ChartSeries = new ISeries[]
            {
                new LineSeries<ObservablePoint>
                {
                    Values = solutionPoints,
                    Fill = null,
                    GeometrySize = 0,
                    Stroke = new SolidColorPaint(new SKColor(139, 92, 246)) { StrokeThickness = 3 },
                    Name = "y(x) — Euler",
                    LineSmoothness = 0,
                    AnimationsSpeed = TimeSpan.FromMilliseconds(800)
                },
                new ScatterSeries<ObservablePoint>
                {
                    Values = dotPoints,
                    GeometrySize = 8,
                    Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2 },
                    Fill = new SolidColorPaint(new SKColor(139, 92, 246)),
                    Name = "Koraci",
                    AnimationsSpeed = TimeSpan.FromMilliseconds(600)
                },
                new ScatterSeries<ObservablePoint>
                {
                    Values = startPoint,
                    GeometrySize = 16,
                    Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 3 },
                    Fill = new SolidColorPaint(new SKColor(6, 182, 212)),
                    Name = $"Početak ({results.First().X}, {results.First().Y})",
                    AnimationsSpeed = TimeSpan.FromMilliseconds(900)
                },
                new ScatterSeries<ObservablePoint>
                {
                    Values = endPoint,
                    GeometrySize = 16,
                    Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 3 },
                    Fill = new SolidColorPaint(new SKColor(16, 185, 129)),
                    Name = $"Kraj ({results.Last().X:F2}, {results.Last().Y:F2})",
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