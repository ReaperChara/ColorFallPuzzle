using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
#if ANDROID && USE_ADMOB
using Soenneker.Maui.Admob.Registrars;
#endif

namespace ColorFallPuzzle;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if ANDROID && USE_ADMOB
        builder.AddAdMobService();
#endif

        return builder.Build();
    }
}
