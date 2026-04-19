using ColorFallPuzzle.Models;
using SkiaSharp;
using System.Diagnostics;
using System.Linq;

namespace ColorFallPuzzle.Services;

public class GameManager
{
    public Models.Grid Grid { get; } = new Models.Grid();
    public Piece? CurrentPiece { get; private set; }
    public int CurrentX { get; private set; }
    public int CurrentY { get; private set; }
    public int Score { get; private set; } = 0;
    
    private Stopwatch _timer = new Stopwatch();
    private const long DropIntervalMs = 1000;
    private readonly SKPaint _cachePaint = new SKPaint { IsAntialias = true };

    public GameManager()
    {
        ResetGame();
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
        int pieceWidth = CurrentPiece.Blocks.Max(b => b.X) - CurrentPiece.Blocks.Min(b => b.X) + 1;
        CurrentX = (Models.Grid.Width - pieceWidth) / 2;
        CurrentY = 0;

        if (!CanMove(0, 0))
        {
            OnGameOver();
            ResetGame();
        }
    }

    private void OnGameOver()
    {
        Debug.WriteLine("Game Over!");
    }

    private void DropPiece()
    {
        if (CanMove(0, 1))
        {
            CurrentY++;
        }
        else
        {
            if (CurrentPiece != null)
            {
                Grid.AddPiece(CurrentPiece, CurrentX, CurrentY);
                Score += Grid.BurstMatches();
                SpawnNewPiece();
            }
        }
    }

    // --- HATALI KISIMLAR BURADA TAMAMLANDI ---

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
        if (CurrentPiece == null) return;
        var temp = CurrentPiece.Clone();
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

    // ---------------------------------------

    private bool CanMove(int dx, int dy, Piece? piece = null)
    {
        piece ??= CurrentPiece;
        if (piece == null) return false;

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

        canvas.Clear(SKColors.Transparent);

        for (int y = 0; y < Models.Grid.Height; y++)
        {
            for (int x = 0; x < Models.Grid.Width; x++)
            {
                var cell = Grid.Cells[x, y];
                if (cell != null)
                {
                    _cachePaint.Color = cell.Color;
                    canvas.DrawRect(offsetX + x * cellSize, offsetY + y * cellSize, cellSize, cellSize, _cachePaint);
                }
            }
        }

        if (CurrentPiece != null)
        {
            foreach (var block in CurrentPiece.Blocks)
            {
                _cachePaint.Color = block.Color;
                canvas.DrawRect(offsetX + (block.X + CurrentX) * cellSize, offsetY + (block.Y + CurrentY) * cellSize, cellSize, cellSize, _cachePaint);
            }
        }
    }

    public void ResetGame()
    {
        Grid.Clear();
        Score = 0;
        SpawnNewPiece();
    }
}