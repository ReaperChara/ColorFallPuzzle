using Android.App;
using Android.OS;
using Android.Content.PM;  // EKLE: ConfigChanges için
using Plugin.MauiMTAdmob;

namespace ColorFallPuzzle;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnResume()
    {
        base.OnResume();
        CrossMauiMTAdmob.Current.OnResume();
    }
}
