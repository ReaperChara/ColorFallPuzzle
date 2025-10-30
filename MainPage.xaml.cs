using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using ColorFallPuzzle.Services;

namespace ColorFallPuzzle;

public partial class MainPage : ContentPage
{
    private Services.GameManager _gameManager;
    private double _canvasWidth, _canvasHeight;

    public MainPage()
    {
        InitializeComponent();
        _gameManager = new Services.GameManager();

        // Timer: 60 FPS update
        Device.StartTimer(TimeSpan.FromMilliseconds(16), () =>
        {
            _gameManager.Update();
            GameCanvas.InvalidateSurface();
            return true;
        });

        // Dokunma kontrolleri
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnCanvasTapped;
        GameCanvas.GestureRecognizers.Add(tapGesture);
    }

    private void OnCanvasPaint(object sender, SKPaintSurfaceEventArgs e)
    {
        _canvasWidth = e.Info.Width;
        _canvasHeight = e.Info.Height;

        SKCanvas canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Black);

        _gameManager.Draw(canvas, (int)_canvasWidth, (int)_canvasHeight);

        ScoreLabel.Text = $"Score: {_gameManager.Score}";
    }

    private void OnCanvasTapped(object sender, TappedEventArgs e)
    {
        var pos = e.GetPosition(GameCanvas);
        if (pos == null) return;

        double x = pos.Value.X;
        double third = _canvasWidth / 3;

        if (x < third)
            _gameManager.MoveLeft();
        else if (x < 2 * third)
            _gameManager.DropFast();
        else
            _gameManager.MoveRight();

        // Yukarı dokunma = döndür (Y koordinatı küçükse)
        if (pos.Value.Y < _canvasHeight * 0.3)
            _gameManager.Rotate();

        GameCanvas.InvalidateSurface();
    }
}
