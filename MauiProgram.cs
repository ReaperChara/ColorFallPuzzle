using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Soenneker.Maui.Admob; // 🔹 AdMob paketi

namespace ColorFallPuzzle;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseAdMob() // 🔹 Soenneker paketi için doğru init
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder.Build();
    }
}
