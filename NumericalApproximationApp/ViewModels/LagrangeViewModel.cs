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
using SkiaSharp;

namespace NumericalApproximationApp.ViewModels
{
    public class LagrangeViewModel : INotifyPropertyChanged
    {
        private string _xPointsInput;
        private string _yPointsInput;
        private string _evaluateXInput;
        private string _resultText;
        private ISeries[] _chartSeries;
        private LiveChartsCore.Kernel.Sketches.ICartesianAxis[] _xAxes;
        private LiveChartsCore.Kernel.Sketches.ICartesianAxis[] _yAxes;

        public string XPointsInput
        {
            get => _xPointsInput;
            set { _xPointsInput = value; OnPropertyChanged(); }
        }

        public string YPointsInput
        {
            get => _yPointsInput;
            set { _yPointsInput = value; OnPropertyChanged(); }
        }

        public string EvaluateXInput
        {
            get => _evaluateXInput;
            set { _evaluateXInput = value; OnPropertyChanged(); }
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

        public ObservableCollection<LagrangePoint> Points { get; set; }
        public ICommand SolveCommand { get; }

        public LagrangeViewModel()
        {
            Points = new ObservableCollection<LagrangePoint>();
            SolveCommand = new Command(OnSolve);

            XPointsInput = "1, 2, 4, 5";
            YPointsInput = "1, 3, 2, 4";
            EvaluateXInput = "3";

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
                    Name = "P(x)",
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
                Points.Clear();

                var xValues = XPointsInput.Split(',')
                    .Select(s => double.Parse(s.Trim(), CultureInfo.InvariantCulture))
                    .ToList();

                var yValues = YPointsInput.Split(',')
                    .Select(s => double.Parse(s.Trim(), CultureInfo.InvariantCulture))
                    .ToList();

                if (xValues.Count != yValues.Count)
                    throw new ArgumentException("Broj x i y vrijednosti mora biti isti.");

                if (xValues.Count < 2)
                    throw new ArgumentException("Potrebne su najmanje 2 tačke.");

                double evalX = double.Parse(EvaluateXInput, CultureInfo.InvariantCulture);

                // Popuni tabelu tačaka
                for (int i = 0; i < xValues.Count; i++)
                {
                    Points.Add(new LagrangePoint
                    {
                        Index = i,
                        X = xValues[i],
                        Y = yValues[i]
                    });
                }

                var lagrange = new LagrangeInterpolation
                {
                    XPoints = xValues,
                    YPoints = yValues
                };

                double result = lagrange.Evaluate(evalX);
                ResultText = $"P({evalX}) ≈ {result:F6}";

                UpdateChart(lagrange, xValues, yValues, evalX, result);
            }
            catch (Exception ex)
            {
                ResultText = $"Greška: {ex.Message}";
            }
        }

        private void UpdateChart(LagrangeInterpolation lagrange,
            List<double> xValues, List<double> yValues,
            double evalX, double evalY)
        {
            double minX = xValues.Min();
            double maxX = xValues.Max();
            double margin = (maxX - minX) * 0.3;

            var curve = lagrange.GenerateCurve(minX - margin, maxX + margin);
            var curvePoints = curve.Select(p => new ObservablePoint(p.X, p.Y)).ToList();

            // Originalne tačke
            var originalPoints = new List<ObservablePoint>();
            for (int i = 0; i < xValues.Count; i++)
            {
                originalPoints.Add(new ObservablePoint(xValues[i], yValues[i]));
            }

            // Evaluirana tačka
            var evalPoints = new List<ObservablePoint>
            {
                new ObservablePoint(evalX, evalY)
            };

            ChartSeries = new ISeries[]
            {
                new LineSeries<ObservablePoint>
                {
                    Values = curvePoints,
                    Fill = null,
                    GeometrySize = 0,
                    Stroke = new SolidColorPaint(new SKColor(59, 130, 246)) { StrokeThickness = 3 },
                    Name = "P(x)",
                    LineSmoothness = 0.5,
                    AnimationsSpeed = TimeSpan.FromMilliseconds(800)
                },
                new ScatterSeries<ObservablePoint>
                {
                    Values = originalPoints,
                    GeometrySize = 14,
                    Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2 },
                    Fill = new SolidColorPaint(new SKColor(6, 182, 212)),
                    Name = "Tačke",
                    AnimationsSpeed = TimeSpan.FromMilliseconds(700)
                },
                new ScatterSeries<ObservablePoint>
                {
                    Values = evalPoints,
                    GeometrySize = 16,
                    Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 3 },
                    Fill = new SolidColorPaint(new SKColor(16, 185, 129)),
                    Name = $"P({evalX}) = {evalY:F2}",
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