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
    public class DataAnalysisViewModel : INotifyPropertyChanged
    {
        private string _xDataInput;
        private string _yDataInput;
        private string _xUnitInput;
        private string _yUnitInput;
        private string _integralResult;
        private string _maxRateResult;
        private string _minRateResult;
        private string _averageResult;
        private string _interpolateXInput;
        private string _interpolateResult;
        private ISeries[] _mainChartSeries;
        private ISeries[] _rateChartSeries;
        private LiveChartsCore.Kernel.Sketches.ICartesianAxis[] _mainXAxes;
        private LiveChartsCore.Kernel.Sketches.ICartesianAxis[] _mainYAxes;
        private LiveChartsCore.Kernel.Sketches.ICartesianAxis[] _rateXAxes;
        private LiveChartsCore.Kernel.Sketches.ICartesianAxis[] _rateYAxes;
        private bool _hasResults;

        public string XDataInput
        {
            get => _xDataInput;
            set { _xDataInput = value; OnPropertyChanged(); }
        }

        public string YDataInput
        {
            get => _yDataInput;
            set { _yDataInput = value; OnPropertyChanged(); }
        }

        public string XUnitInput
        {
            get => _xUnitInput;
            set { _xUnitInput = value; OnPropertyChanged(); }
        }

        public string YUnitInput
        {
            get => _yUnitInput;
            set { _yUnitInput = value; OnPropertyChanged(); }
        }

        public string IntegralResult
        {
            get => _integralResult;
            set { _integralResult = value; OnPropertyChanged(); }
        }

        public string MaxRateResult
        {
            get => _maxRateResult;
            set { _maxRateResult = value; OnPropertyChanged(); }
        }

        public string MinRateResult
        {
            get => _minRateResult;
            set { _minRateResult = value; OnPropertyChanged(); }
        }

        public string AverageResult
        {
            get => _averageResult;
            set { _averageResult = value; OnPropertyChanged(); }
        }

        public string InterpolateXInput
        {
            get => _interpolateXInput;
            set { _interpolateXInput = value; OnPropertyChanged(); }
        }

        public string InterpolateResult
        {
            get => _interpolateResult;
            set { _interpolateResult = value; OnPropertyChanged(); }
        }

        public bool HasResults
        {
            get => _hasResults;
            set { _hasResults = value; OnPropertyChanged(); }
        }

        public ISeries[] MainChartSeries
        {
            get => _mainChartSeries;
            set { _mainChartSeries = value; OnPropertyChanged(); }
        }

        public ISeries[] RateChartSeries
        {
            get => _rateChartSeries;
            set { _rateChartSeries = value; OnPropertyChanged(); }
        }

        public LiveChartsCore.Kernel.Sketches.ICartesianAxis[] MainXAxes
        {
            get => _mainXAxes;
            set { _mainXAxes = value; OnPropertyChanged(); }
        }

        public LiveChartsCore.Kernel.Sketches.ICartesianAxis[] MainYAxes
        {
            get => _mainYAxes;
            set { _mainYAxes = value; OnPropertyChanged(); }
        }

        public LiveChartsCore.Kernel.Sketches.ICartesianAxis[] RateXAxes
        {
            get => _rateXAxes;
            set { _rateXAxes = value; OnPropertyChanged(); }
        }

        public LiveChartsCore.Kernel.Sketches.ICartesianAxis[] RateYAxes
        {
            get => _rateYAxes;
            set { _rateYAxes = value; OnPropertyChanged(); }
        }

        public ICommand AnalyzeCommand { get; }
        public ICommand InterpolateCommand { get; }

        private List<double> _xValues;
        private List<double> _yValues;

        public DataAnalysisViewModel()
        {
            AnalyzeCommand = new Command(OnAnalyze);
            InterpolateCommand = new Command(OnInterpolate);
            HasResults = false;

            // Primjer: temperatura pacijenta tokom dana (sati vs °C)
            XDataInput = "0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24";
            YDataInput = "36.6, 36.8, 37.2, 37.8, 38.5, 39.1, 38.7, 38.2, 37.6, 37.1, 36.9, 36.7, 36.6";
            XUnitInput = "sati";
            YUnitInput = "°C";
            InterpolateXInput = "7";

            InitializeCharts();
        }

        private void InitializeCharts()
        {
            MainChartSeries = Array.Empty<ISeries>();
            RateChartSeries = Array.Empty<ISeries>();

            MainXAxes = CreateAxes("x");
            MainYAxes = CreateAxes("y");
            RateXAxes = CreateAxes("x");
            RateYAxes = CreateAxes("Promjena");
        }

        private LiveChartsCore.Kernel.Sketches.ICartesianAxis[] CreateAxes(string name)
        {
            return new LiveChartsCore.Kernel.Sketches.ICartesianAxis[]
            {
                new Axis
                {
                    Name = name,
                    NamePaint = new SolidColorPaint(SKColors.White),
                    LabelsPaint = new SolidColorPaint(new SKColor(148, 163, 184)),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(42, 58, 78))
                }
            };
        }

        private void OnAnalyze()
        {
            try
            {
                _xValues = XDataInput.Split(',')
                    .Select(s => double.Parse(s.Trim(), CultureInfo.InvariantCulture))
                    .ToList();

                _yValues = YDataInput.Split(',')
                    .Select(s => double.Parse(s.Trim(), CultureInfo.InvariantCulture))
                    .ToList();

                if (_xValues.Count != _yValues.Count)
                    throw new ArgumentException("Broj x i y vrijednosti mora biti isti.");

                if (_xValues.Count < 2)
                    throw new ArgumentException("Potrebne su najmanje 2 tačke.");

                string xUnit = string.IsNullOrEmpty(XUnitInput) ? "" : $" {XUnitInput}";
                string yUnit = string.IsNullOrEmpty(YUnitInput) ? "" : $" {YUnitInput}";

                // 1. INTEGRAL (trapezno pravilo) — ukupna akumulacija
                double integral = 0;
                for (int i = 0; i < _xValues.Count - 1; i++)
                {
                    double h = _xValues[i + 1] - _xValues[i];
                    integral += h * (_yValues[i] + _yValues[i + 1]) / 2.0;
                }
                IntegralResult = $"{integral:F2} {YUnitInput}·{XUnitInput}";

                // 2. DERIVACIJA (brzina promjene) u svakoj tački
                var rates = new List<double>();
                for (int i = 0; i < _xValues.Count; i++)
                {
                    double rate;
                    if (i == 0)
                        rate = (_yValues[1] - _yValues[0]) / (_xValues[1] - _xValues[0]);
                    else if (i == _xValues.Count - 1)
                        rate = (_yValues[i] - _yValues[i - 1]) / (_xValues[i] - _xValues[i - 1]);
                    else
                        rate = (_yValues[i + 1] - _yValues[i - 1]) / (_xValues[i + 1] - _xValues[i - 1]);
                    rates.Add(rate);
                }

                int maxIdx = rates.IndexOf(rates.Max());
                int minIdx = rates.IndexOf(rates.Min());

                MaxRateResult = $"+{rates.Max():F4} {yUnit}/{xUnit} (u tački x={_xValues[maxIdx]})";
                MinRateResult = $"{rates.Min():F4} {yUnit}/{xUnit} (u tački x={_xValues[minIdx]})";

                // 3. PROSJEK
                double average = _yValues.Average();
                AverageResult = $"{average:F4}{yUnit}";

                // Ažuriraj ose
                MainXAxes = CreateAxes(XUnitInput);
                MainYAxes = CreateAxes(YUnitInput);
                RateXAxes = CreateAxes(XUnitInput);
                RateYAxes = CreateAxes($"Δ{YUnitInput}/Δ{XUnitInput}");

                UpdateMainChart();
                UpdateRateChart(rates);

                HasResults = true;

                // Auto interpolacija
                OnInterpolate();
            }
            catch (Exception ex)
            {
                IntegralResult = $"Greška: {ex.Message}";
                HasResults = false;
            }
        }

        private void OnInterpolate()
        {
            try
            {
                if (_xValues == null || _yValues == null) return;

                double xTarget = double.Parse(InterpolateXInput, CultureInfo.InvariantCulture);

                // Lagrange interpolacija
                double result = 0;
                int n = _xValues.Count;
                for (int i = 0; i < n; i++)
                {
                    double li = 1;
                    for (int j = 0; j < n; j++)
                    {
                        if (i != j)
                            li *= (xTarget - _xValues[j]) / (_xValues[i] - _xValues[j]);
                    }
                    result += _yValues[i] * li;
                }

                string yUnit = string.IsNullOrEmpty(YUnitInput) ? "" : $" {YUnitInput}";
                InterpolateResult = $"Vrijednost u x={xTarget} ≈ {result:F4}{yUnit}";
            }
            catch (Exception ex)
            {
                InterpolateResult = $"Greška: {ex.Message}";
            }
        }

        private void UpdateMainChart()
        {
            var dataPoints = new List<ObservablePoint>();
            for (int i = 0; i < _xValues.Count; i++)
            {
                dataPoints.Add(new ObservablePoint(_xValues[i], _yValues[i]));
            }

            // Popunjena površina (integral vizuelno)
            var areaPoints = new List<ObservablePoint>();
            areaPoints.Add(new ObservablePoint(_xValues.First(), 0));
            for (int i = 0; i < _xValues.Count; i++)
            {
                areaPoints.Add(new ObservablePoint(_xValues[i], _yValues[i]));
            }
            areaPoints.Add(new ObservablePoint(_xValues.Last(), 0));

            MainChartSeries = new ISeries[]
            {
        new LineSeries<ObservablePoint>
        {
            Values = areaPoints,
            Fill = new SolidColorPaint(new SKColor(59, 130, 246, 30)),
            GeometrySize = 0,
            Stroke = new SolidColorPaint(SKColors.Transparent),
            Name = "Integral",
            LineSmoothness = 0,
            AnimationsSpeed = TimeSpan.FromMilliseconds(500)
        },
        new LineSeries<ObservablePoint>
        {
            Values = dataPoints,
            Fill = null,
            GeometrySize = 0,
            Stroke = new SolidColorPaint(new SKColor(59, 130, 246)) { StrokeThickness = 2 },
            Name = "Podaci",
            LineSmoothness = 0.3,
            AnimationsSpeed = TimeSpan.FromMilliseconds(700)
        },
        new ScatterSeries<ObservablePoint>
        {
            Values = dataPoints,
            GeometrySize = 12,
            Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2 },
            Fill = new SolidColorPaint(new SKColor(6, 182, 212)),
            Name = "Mjerenja",
            AnimationsSpeed = TimeSpan.FromMilliseconds(800)
        }
            };
        }

        private void UpdateRateChart(List<double> rates)
        {
            var ratePoints = new List<ObservablePoint>();
            for (int i = 0; i < _xValues.Count; i++)
            {
                ratePoints.Add(new ObservablePoint(_xValues[i], rates[i]));
            }

            var zeroLine = new List<ObservablePoint>
            {
                new ObservablePoint(_xValues.First(), 0),
                new ObservablePoint(_xValues.Last(), 0)
            };

            RateChartSeries = new ISeries[]
            {
                new LineSeries<ObservablePoint>
                {
                    Values = zeroLine,
                    Fill = null,
                    GeometrySize = 0,
                    Stroke = new SolidColorPaint(new SKColor(148, 163, 184)) { StrokeThickness = 1 },
                    AnimationsSpeed = TimeSpan.FromMilliseconds(400)
                },
                new LineSeries<ObservablePoint>
                {
                    Values = ratePoints,
                    Fill = null,
                    GeometrySize = 8,
                    GeometryStroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2 },
                    GeometryFill = new SolidColorPaint(new SKColor(245, 158, 11)),
                    Stroke = new SolidColorPaint(new SKColor(245, 158, 11)) { StrokeThickness = 2 },
                    Name = "Brzina promjene",
                    LineSmoothness = 0.3,
                    AnimationsSpeed = TimeSpan.FromMilliseconds(800)
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