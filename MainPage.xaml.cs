using Microsoft.Maui.Controls;
using Plugin.MauiMTAdmob;  // eklendi
using SkiaSharp.Views.Maui.Controls;
using SkiaSharp.Views.Maui;
using ColorFallPuzzle.Services;

namespace ColorFallPuzzle;

public partial class MainPage : ContentPage
{
    private GameManager _gameManager;
    private double _canvasWidth, _canvasHeight;

    public MainPage()
    {
        InitializeComponent();
        _gameManager = new GameManager();

        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), () =>
        {
            _gameManager.Update();
            GameCanvas.InvalidateSurface();
            return true;
        });

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        GameCanvas.GestureRecognizers.Add(tap);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Banner reklamı programatik ekliyoruz
        var banner = new MauiMTAdmobView
        {
            AdUnitId = "ca-app-pub-3940256099942544/6300978111", // test reklam ID’si
            AdSize = AdSizeType.Banner
        };

        // sayfada Grid içinde altta tanımlı satıra ekleme
        if (this.Content is Layout layout)
        {
            layout.Children.Add(banner);
        }

        banner.LoadAd();
    }

    private void OnTapped(object? sender, TappedEventArgs e)
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

    private void OnCanvasPaint(object? sender, SKPaintSurfaceEventArgs e)
    {
        _canvasWidth = e.Info.Width;
        _canvasHeight = e.Info.Height;

        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Black);
        _gameManager.Draw(canvas, (int)_canvasWidth, (int)_canvasHeight);

        ScoreLabel.Text = $"Score: {_gameManager.Score}";
    }
}
