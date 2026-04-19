using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Gms.Ads;

namespace ColorFallPuzzle;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        try 
        {
            // Manuel init, çökme riskini azaltır
            MobileAds.Initialize(this);
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Reklam Hatası: {ex.Message}");
        }
    }
}