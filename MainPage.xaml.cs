using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace ColorFallPuzzle;

public partial class MainPage : ContentPage
{
    private Services.GameManager _gameManager;

    public MainPage()
    {
        InitializeComponent();
        _gameManager = new Services.GameManager();  // Sonra oluşturacağımız class
        Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
        {
            _gameManager.Update();  // Oyun update
            GameCanvas.InvalidateSurface();  // Yeniden çiz
            return true;
        });
    }

    private void OnCanvasPaint(object sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Black);

        // Grid ve parçaları çiz (GameManager'dan al)
        _gameManager.Draw(canvas, e.Info.Width, e.Info.Height);

        // Skor güncelle
        ScoreLabel.Text = $"Score: {_gameManager.Score}";
    }
}
