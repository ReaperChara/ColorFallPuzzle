using ColorFallPuzzle.Models;
using SkiaSharp;
using System.Diagnostics;
using System.Linq;

namespace ColorFallPuzzle.Services;

public class GameManager
{
    public Models.Grid Grid { get; } = new Models.Grid();  // FIX: Tam qualify et
    public Piece? CurrentPiece { get; private set; }
    public int CurrentX { get; private set; } = Models.Grid.Width / 2 - 2;  // FIX: Tam qualify
    public int CurrentY { get; private set; } = 0;
    public int Score { get; private set; } = 0;
    private Stopwatch _timer = new Stopwatch();
    private const long DropIntervalMs = 1000;

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
        CurrentX = Models.Grid.Width / 2 - (CurrentPiece.Blocks.Max(b => b.X) - CurrentPiece.Blocks.Min(b => b.X)) / 2;
        CurrentY = 0;

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
            Grid.AddPiece(CurrentPiece!, CurrentX, CurrentY);
            Score += Grid.BurstMatches();
            SpawnNewPiece();
        }
    }

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
        DropPiece();
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

            if (nx < 0 || nx >= Models.Grid.Width || ny < 0 || ny >= Models.Grid.Height || Grid.Cells[nx, ny] != null)
                return false;
        }
        return true;
    }

    public void Draw(SKCanvas canvas, int width, int height)
    {
        float cellSize = Math.Min(width / (float)Models.Grid.Width, height / (float)Models.Grid.Height);
        float offsetX = (width - Models.Grid.Width * cellSize) / 2;
        float offsetY = (height - Models.Grid.Height * cellSize) / 2;

        for (int y = 0; y < Models.Grid.Height; y++)
        {
            for (int x = 0; x < Models.Grid.Width; x++)
            {
                if (Grid.Cells[x, y] != null)
                {
                    using var paint = new SKPaint { Color = Grid.Cells[x, y]!.Color };
                    canvas.DrawRect(offsetX + x * cellSize, offsetY + y * cellSize, cellSize, cellSize, paint);
                }
            }
        }

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
