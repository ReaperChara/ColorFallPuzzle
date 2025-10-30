namespace ColorFallPuzzle.Models;

public class Block
{
    public int X { get; set; }
    public int Y { get; set; }
    public SKColor Color { get; set; }

    public Block(int x, int y, SKColor color)
    {
        X = x;
        Y = y;
        Color = color;
    }

    // Kopya oluştur (hareket için)
    public Block Clone() => new Block(X, Y, Color);
}
