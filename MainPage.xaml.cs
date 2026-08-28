using Microsoft.Maui.Dispatching;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
#if ANDROID && USE_ADMOB
using Soenneker.Maui.Admob;
using Soenneker.Maui.Admob.Enums;
#endif
using ColorFallPuzzle.Services;
using System.Diagnostics;

namespace ColorFallPuzzle;

public partial class MainPage : ContentPage
{
    private const string TestBannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
    private readonly GameManager _gameManager;
    private double _canvasWidth, _canvasHeight;
    private bool _isTimerRunning = false;
#if ANDROID && USE_ADMOB
    private bool _adInitialized = false;
#endif
    private bool _isPageActive = false;
    private int _lastRenderedScore = -1;
#if ANDROID && USE_ADMOB
    private BannerAd? _bannerAd;
#endif

    public MainPage()
    {
        InitializeComponent();

        _gameManager = new GameManager();

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        GameCanvas.GestureRecognizers.Add(tap);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _isPageActive = true;
#if ANDROID && USE_ADMOB
        InitializeBannerAd();
#endif
        StartGameLoop();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _isPageActive = false;
        _isTimerRunning = false;
#if ANDROID && USE_ADMOB
        RemoveBannerAd();
#endif
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

                GameCanvas.InvalidateSurface();

                return _isPageActive;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Game Loop Error: {ex}");
                _isTimerRunning = false;
                return false;
            }
        });
    }

#if ANDROID && USE_ADMOB
    private void InitializeBannerAd()
    {
        if (_adInitialized) return;

        try
        {
#if ANDROID
            var banner = new BannerAd
            {
                AdUnitId = TestBannerAdUnitId,
                Size = AdmobAdSize.Banner,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End
            };

            if (BannerHost != null)
            {
                BannerHost.Children.Add(banner);
                _bannerAd = banner;
                _adInitialized = true;
            }
#endif
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AdMob Init Error: {ex}");
        }
    }

    private void RemoveBannerAd()
    {
        if (_bannerAd == null)
            return;

        BannerHost?.Children.Remove(_bannerAd);
        _bannerAd = null;
        _adInitialized = false;
    }
#endif

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

            if (_lastRenderedScore != _gameManager.Score)
            {
                _lastRenderedScore = _gameManager.Score;
                ScoreLabel.Text = $"Score: {_lastRenderedScore}";
            }
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
            if (_canvasWidth <= 0 || _canvasHeight <= 0) return;

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