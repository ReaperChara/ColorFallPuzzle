using Microsoft.Maui.Dispatching;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Soenneker.Maui.Admob;
using Soenneker.Maui.Admob.Enums;
using ColorFallPuzzle.Services;
using System.Diagnostics;

namespace ColorFallPuzzle;

public partial class MainPage : ContentPage
{
    private GameManager _gameManager;
    private double _canvasWidth, _canvasHeight;
    private bool _isTimerRunning = false;
    private bool _adInitialized = false;

    public MainPage()
    {
        InitializeComponent();

        _gameManager = new GameManager();

        this.Loaded += OnPageLoaded;

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        GameCanvas.GestureRecognizers.Add(tap);
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        InitializeBannerAd();
        StartGameLoop();
    }

    private void StartGameLoop()
    {
        if (_isTimerRunning) return;
        _isTimerRunning = true;

        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(33), () =>
        {
            try
            {
                _gameManager.Update();

                // Direkt UI thread flood yok
                GameCanvas.InvalidateSurface();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Game Loop Error: {ex}");
                return false;
            }
        });
    }

    private void InitializeBannerAd()
    {
        if (_adInitialized) return;

        try
        {
#if ANDROID
            var banner = new BannerAd
            {
                Size = AdmobAdSize.Banner,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End
            };

            if (BannerHost != null)
            {
                BannerHost.Children.Add(banner);
                _adInitialized = true;
            }
#endif
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AdMob Init Error: {ex}");
        }
    }

    private void OnCanvasPaint(object? sender, SKPaintSurfaceEventArgs e)
    {
        try
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.Black);

            _canvasWidth = e.Info.Width;
            _canvasHeight = e.Info.Height;

            if (_canvasWidth <= 0 || _canvasHeight <= 0)
                return;

            _gameManager.Draw(canvas, (int)_canvasWidth, (int)_canvasHeight);

            // UI thread spam yok (kritik fix)
            ScoreLabel.Text = $"Score: {_gameManager.Score}";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Render Error: {ex}");
        }
    }

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        try
        {
            var pos = e.GetPosition(GameCanvas);
            if (!pos.HasValue) return;

            double x = pos.Value.X;
            double y = pos.Value.Y;

            double thirdX = _canvasWidth / 3;
            double topY = _canvasHeight * 0.3;

            if (x < thirdX)
                _gameManager.MoveLeft();
            else if (x < 2 * thirdX)
                _gameManager.DropFast();
            else
                _gameManager.MoveRight();

            if (y < topY)
                _gameManager.Rotate();

            GameCanvas.InvalidateSurface();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Touch Error: {ex}");
        }
    }
}