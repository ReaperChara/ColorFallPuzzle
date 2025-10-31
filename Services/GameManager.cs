using ColorFallPuzzle.Models;
using SkiaSharp;
using System.Diagnostics;
using System.Linq;  // EKLE: Max/Min için

namespace ColorFallPuzzle.Services;

public class GameManager
{
    public Grid Grid { get; } = new Grid();
    public Piece? CurrentPiece { get; private set; }
    public int CurrentX { get; private set; } = Grid.Width / 2 - 2;  // Başlangıç X (merkez)
    public int CurrentY { get; private set; } = 0;  // Başlangıç Y (üst)
    public int Score { get; private set; } = 0;
    private Stopwatch _timer = new Stopwatch();
    private const long DropIntervalMs = 1000;  // 1 sn'de bir düş

    public GameManager()
    {
        SpawnNewPiece();
        _timer.Start();
    }

    public void Update()
    {
        if (_timer.ElapsedMilliseconds >= DropIntervalMs)
        {
            DropPiece();
            _timer.Restart();
        }
    }

    private void SpawnNewPiece()
    {
        CurrentPiece = new Piece();
        CurrentX = Grid.Width / 2 - (CurrentPiece.Blocks.Max(b => b.X) - CurrentPiece.Blocks.Min(b => b.X)) / 2;
        CurrentY = 0;

        // Eğer çarpışma varsa oyun biter (şimdilik basit, sonra game over ekle)
        if (!CanMove(0, 0))
        {
            ResetGame();
        }
    }

    private void DropPiece()
    {
        if (CanMove(0, 1))
        {
            CurrentY++;
        }
        else
        {
            // Yerleştir
            Grid.AddPiece(CurrentPiece!, CurrentX, CurrentY);
            // Patlat
            Score += Grid.BurstMatches();
            // Yeni parça
            SpawnNewPiece();
        }
    }

    // Kullanıcı hareketi (sol/sağ/dön/aşağı) - Dokunma event'leri sonra ekleriz
    public void MoveLeft()
    {
        if (CanMove(-1, 0)) CurrentX--;
    }

    public void MoveRight()
    {
        if (CanMove(1, 0)) CurrentX++;
    }

    public void Rotate()
    {
        var temp = CurrentPiece!.Clone();
        temp.Rotate();
        if (CanMove(0, 0, temp))
        {
            CurrentPiece = temp;
        }
    }

    public void DropFast()
    {
        while (CanMove(0, 1))
        {
            CurrentY++;
        }
        DropPiece();  // Hemen yerleştir
    }

    private bool CanMove(int dx, int dy, Piece? piece = null)
    {
        if (piece == null)
        {
            piece = CurrentPiece!;
        }

        foreach (var block in piece.Blocks)
        {
            int nx = block.X + CurrentX + dx;
            int ny = block.Y + CurrentY + dy;

            if (nx < 0 || nx >= Grid.Width || ny < 0 || ny >= Grid.Height || Grid.Cells[nx, ny] != null)
                return false;
        }
        return true;
    }

    public void Draw(SKCanvas canvas, int width, int height)
    {
        float cellSize = Math.Min(width / (float)Grid.Width, height / (float)Grid.Height);
        float offsetX = (width - Grid.Width * cellSize) / 2;
        float offsetY = (height - Grid.Height * cellSize) / 2;

        // Grid hücreleri çiz
        for (int y = 0; y < Grid.Height; y++)
        {
            for (int x = 0; x < Grid.Width; x++)
            {
                if (Grid.Cells[x, y] != null)
                {
                    using var paint = new SKPaint { Color = Grid.Cells[x, y]!.Color };
                    canvas.DrawRect(offsetX + x * cellSize, offsetY + y * cellSize, cellSize, cellSize, paint);
                }
            }
        }

        // Mevcut parça çiz
        if (CurrentPiece != null)
        {
            foreach (var block in CurrentPiece.Blocks)
            {
                int x = block.X + CurrentX;
                int y = block.Y + CurrentY;
                using var paint = new SKPaint { Color = block.Color };
                canvas.DrawRect(offsetX + x * cellSize, offsetY + y * cellSize, cellSize, cellSize, paint);
            }
        }
    }

    private void ResetGame()
    {
        Grid.Clear();
        Score = 0;
        SpawnNewPiece();
    }
}
