using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.MauiMTAdmob;  // DOĞRU using

namespace ColorFallPuzzle;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)  // Nullable Bundle? (warning fix)
    {
        base.OnCreate(savedInstanceState);
        CrossMauiMTAdmob.Current.Init(this);  // DOĞRU init
    }
}
