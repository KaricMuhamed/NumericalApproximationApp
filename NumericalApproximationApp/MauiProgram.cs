using SkiaSharp.Views.Maui.Controls.Hosting;
using Plugin.Maui.OCR;
using NumericalApproximationApp.Services;

namespace NumericalApproximationApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .UseOcr()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IOcrService>(OcrPlugin.Default);
            builder.Services.AddSingleton<OCRService>();
            builder.Services.AddTransient<Views.BisectionPage>();
            builder.Services.AddTransient<Views.NewtonRaphsonPage>();
            builder.Services.AddTransient<Views.TrapezoidalPage>();
            builder.Services.AddTransient<Views.SimpsonPage>();
            builder.Services.AddTransient<Views.NumericalDerivativePage>();
            builder.Services.AddTransient<Views.LagrangePage>();
            builder.Services.AddTransient<Views.EulerPage>();

            return builder.Build();
        }
    }
}