using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Plugin.MauiMTAdmob;  // EKLE: CrossMTAdmob için

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

        // AdMob init
        CrossMTAdmob.Current.Init();

        return builder.Build();
    }
}
