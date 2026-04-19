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

    public MainPage()
    {
        InitializeComponent();
        
        // GameManager'ı oluştur
        _gameManager = new GameManager();

        // Sayfa tamamen yüklendiğinde reklamı ve oyun döngüsünü başlat
        this.Loaded += (s, e) => 
        {
            InitializeBannerAd();
            StartGameLoop();
        };

        // Tıklama olayını bağla
        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        GameCanvas.GestureRecognizers.Add(tap);
    }

    private void StartGameLoop()
    {
        if (_isTimerRunning) return;
        _isTimerRunning = true;

        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), () =>
        {
            try 
            {
                _gameManager.Update();
                
                // UI güncellemelerini ana iş parçacığında güvenli yap
                MainThread.BeginInvokeOnMainThread(() => {
                    GameCanvas.InvalidateSurface();
                });
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Oyun Döngüsü Hatası: {ex.Message}");
                return false; 
            }
        });
    }

    private void InitializeBannerAd()
    {
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
            }
#endif
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AdMob Yükleme Hatası: {ex.Message}");
        }
    }

    private void OnCanvasPaint(object? sender, SKPaintSurfaceEventArgs e)
    {
        try 
        {
            _canvasWidth = e.Info.Width;
            _canvasHeight = e.Info.Height;

            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.Black);

            // Eğer genişlik/yükseklik henüz hesaplanmadıysa çizim yapma (Çökmeyi engeller)
            if (_canvasWidth <= 0 || _canvasHeight <= 0) return;

            _gameManager.Draw(canvas, (int)_canvasWidth, (int)_canvasHeight);

            // Skoru güvenli bir şekilde güncelle
            MainThread.BeginInvokeOnMainThread(() => {
                ScoreLabel.Text = $"Score: {_gameManager.Score}";
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Çizim Hatası: {ex.Message}");
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

            // Kontroller
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
            Debug.WriteLine($"Dokunma Hatası: {ex.Message}");
        }
    }
}