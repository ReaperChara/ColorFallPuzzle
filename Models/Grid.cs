using SkiaSharp;

namespace ColorFallPuzzle.Models;

public class Grid
{
    public const int Width = 12;
    public const int Height = 24;
    public Block?[,] Cells { get; private set; } = new Block?[Width, Height];

    // Blok ekle (çakışma varsa false)
    public bool AddPiece(Piece piece, int offsetX, int offsetY)
    {
        foreach (var block in piece.Blocks)
        {
            int x = block.X + offsetX;
            int y = block.Y + offsetY;

            if (x < 0 || x >= Width || y < 0 || y >= Height || Cells[x, y] != null)
                return false;
        }

        // Ekle
        foreach (var block in piece.Blocks)
        {
            int x = block.X + offsetX;
            int y = block.Y + offsetY;
            Cells[x, y] = block.Clone();
        }

        return true;
    }

    // Renk patlatma (3+ aynı renk yan yana/dikey)
    public int BurstMatches()
    {
        bool[,] visited = new bool[Width, Height];
        int score = 0;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Cells[x, y] == null || visited[x, y]) continue;

                var color = Cells[x, y]!.Color;
                var matches = FloodFill(x, y, color, visited);

                if (matches.Count >= 3)
                {
                    foreach (var (mx, my) in matches)
                    {
                        Cells[mx, my] = null;
                    }
                    score += matches.Count * 10;
                }
            }
        }

        DropFloatingBlocks();
        return score;
    }

    private List<(int x, int y)> FloodFill(int startX, int startY, SKColor color, bool[,] visited)
    {
        var matches = new List<(int, int)>();
        var queue = new Queue<(int, int)>();
        queue.Enqueue((startX, startY));
        visited[startX, startY] = true;

        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { 1, 0, -1, 0 };

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();
            matches.Add((x, y));

            for (int d = 0; d < 4; d++)
            {
                int nx = x + dx[d];
                int ny = y + dy[d];

                if (nx >= 0 && nx < Width && ny >= 0 && ny < Height &&
                    !visited[nx, ny] && Cells[nx, ny]?.Color.Equals(color) == true)
                {
                    visited[nx, ny] = true;
                    queue.Enqueue((nx, ny));
                }
            }
        }

        return matches;
    }

    private void DropFloatingBlocks()
    {
        for (int x = 0; x < Width; x++)
        {
            int writeY = Height - 1;
            for (int y = Height - 1; y >= 0; y--)
            {
                if (Cells[x, y] != null)
                {
                    if (y != writeY)
                    {
                        Cells[x, writeY] = Cells[x, y];
                        Cells[x, y] = null;
                    }
                    writeY--;
                }
            }
        }
    }

    public void Clear()
    {
        Cells = new Block?[Width, Height];
    }
}
