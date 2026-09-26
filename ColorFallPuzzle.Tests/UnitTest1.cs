using ColorFallPuzzle.Models;
using SkiaSharp;

namespace ColorFallPuzzle.Tests;

public class GridTests
{
    [Fact]
    public void AddPiece_WhenAnyBlockOverlaps_DoesNotPartiallyWriteCells()
    {
        var grid = new Grid();
        grid.Cells[3, 3] = new Block(3, 3, SKColors.Blue);

        var piece = new Piece();
        piece.Blocks.Clear();
        piece.Blocks.Add(new Block(0, 0, SKColors.Red));
        piece.Blocks.Add(new Block(1, 0, SKColors.Green));

        var added = grid.AddPiece(piece, 2, 3);

        Assert.False(added);
        Assert.NotNull(grid.Cells[3, 3]);
        Assert.Null(grid.Cells[2, 3]);
    }

    [Fact]
    public void BurstMatches_RemovesOnlyMatchedGroup_AndDropsFloatingBlocks()
    {
        var grid = new Grid();
        grid.Cells[0, 20] = new Block(0, 20, SKColors.Red);
        grid.Cells[0, 21] = new Block(0, 21, SKColors.Red);
        grid.Cells[0, 22] = new Block(0, 22, SKColors.Red);
        grid.Cells[0, 10] = new Block(0, 10, SKColors.Green);
        grid.Cells[1, 23] = new Block(1, 23, SKColors.Blue);

        var score = grid.BurstMatches();

        Assert.Equal(30, score);
        Assert.NotNull(grid.Cells[0, 23]);
        Assert.Equal(SKColors.Green, grid.Cells[0, 23]!.Color);
        Assert.Null(grid.Cells[0, 20]);
        Assert.Null(grid.Cells[0, 21]);
        Assert.Null(grid.Cells[0, 22]);
        Assert.NotNull(grid.Cells[1, 23]);
        Assert.Equal(SKColors.Blue, grid.Cells[1, 23]!.Color);
    }
}
