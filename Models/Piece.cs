using SkiaSharp;

namespace ColorFallPuzzle.Models;

public class Piece
{
    public List<Block> Blocks { get; private set; } = new();
    public int PivotX { get; private set; }
    public int PivotY { get; private set; }

    private static readonly SKColor[] Colors = new[]
    {
        SKColors.Red, SKColors.Blue, SKColors.Green,
        SKColors.Yellow, SKColors.Purple, SKColors.Orange
    };

    private static readonly Random Rand = new();

    // 12 Pentomino (F, I, L, N, P, T, U, V, W, X, Y, Z) – Tetris'ten tamamen farklı
    private static readonly int[][][] Pentominoes = new int[][][]
    {
        // F
        new int[][] {
            new[] {0,1,1},
            new[] {1,1,0},
            new[] {0,1,0}
        },
        // I
        new int[][] {
            new[] {1,1,1,1,1}
        },
        // L
        new int[][] {
            new[] {1,0,0},
            new[] {1,0,0},
            new[] {1,1,1}
        },
        // N
        new int[][] {
            new[] {0,1,1},
            new[] {1,1,0},
            new[] {0,0,1}
        },
        // P
        new int[][] {
            new[] {1,1},
            new[] {1,1},
            new[] {1,0}
        },
        // T
        new int[][] {
            new[] {1,1,1},
            new[] {0,1,0},
            new[] {0,1,0}
        },
        // U
        new int[][] {
            new[] {1,0,1},
            new[] {1,1,1}
        },
        // V
        new int[][] {
            new[] {1,0,0},
            new[] {1,0,0},
            new[] {1,1,1}
        },
        // W
        new int[][] {
            new[] {1,0,0},
            new[] {1,1,0},
            new[] {0,1,1}
        },
        // X
        new int[][] {
            new[] {0,1,0},
            new[] {1,1,1},
            new[] {0,1,0}
        },
        // Y
        new int[][] {
            new[] {0,1,0},
            new[] {1,1,1},
            new[] {0,1,0},
            new[] {0,1,0}
        },
        // Z
        new int[][] {
            new[] {1,1,0},
            new[] {0,1,0},
            new[] {0,1,1}
        }
    };

    public Piece()
    {
        GenerateRandomPiece();
    }

    private void GenerateRandomPiece()
    {
        int index = Rand.Next(Pentominoes.Length);
        var shape = Pentominoes[index];
        Blocks.Clear();

        int minX = int.MaxValue, minY = int.MaxValue;

        // Şekli bloklara çevir
        for (int y = 0; y < shape.Length; y++)
        {
            for (int x = 0; x < shape[y].Length; x++)
            {
                if (shape[y][x] == 1)
                {
                    var color = Colors[Rand.Next(Colors.Length)];
                    Blocks.Add(new Block(x, y, color));
                    minX = Math.Min(minX, x);
                    minY = Math.Min(minY, y);
                }
            }
        }

        // Normalize: sola-yukarıya kaydır
        foreach (var b in Blocks)
        {
            b.X -= minX;
            b.Y -= minY;
        }

        // Pivot (merkez) belirle – basit Round ile
        double avgX = Blocks.Average(b => b.X);
        PivotX = (int)Math.Round(avgX);

        double avgY = Blocks.Average(b => b.Y);
        PivotY = (int)Math.Round(avgY);
    }

    // Parçayı döndür (90 derece saat yönünde)
    public void Rotate()
    {
        for (int i = 0; i < Blocks.Count; i++)
        {
            var b = Blocks[i];
            int relX = b.X - PivotX;
            int relY = b.Y - PivotY;
            int newX = PivotX + relY;
            int newY = PivotY - relX;
            Blocks[i] = new Block(newX, newY, b.Color);
        }
    }

    // Kopya oluştur
    public Piece Clone()
    {
        var p = new Piece();
        p.Blocks = Blocks.Select(b => b.Clone()).ToList();
        p.PivotX = PivotX;
        p.PivotY = PivotY;
        return p;
    }
}
