using Microsoft.Maui.Dispatching;  // EKLE: Dispatcher için
using SkiaSharp;  // EKLE: SKColors için
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
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

        // Dispatcher ile timer (obsolete fix)
        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), () =>
        {
            _gameManager.Update();
            GameCanvas.InvalidateSurface();
            return true;
        });

        // Dokunma
        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        GameCanvas.GestureRecognizers.Add(tap);
    }

    private void OnCanvasPaint(object sender, SKPaintSurfaceEventArgs e)  // XAML ile eşleşsin
    {
        _canvasWidth = e.Info.Width;
        _canvasHeight = e.Info.Height;

        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Black);
        _gameManager.Draw(canvas, (int)_canvasWidth, (int)_canvasHeight);

        ScoreLabel.Text = $"Score: {_gameManager.Score}";
    }

    private void OnTapped(object? sender, TappedEventArgs e)  // Nullable sender (warning fix)
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
}
