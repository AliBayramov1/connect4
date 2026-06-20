using Xunit;

namespace Connect4.Tests;

public class GameTests
{
    [Fact]
    public void IsValid_EmptyColumn_ReturnsTrue()
    {
        Game game = new Game();

        Assert.True(game.is_valid(0));
    }

    [Fact]
    public void IsValid_FullColumn_ReturnsFalse()
    {
        Game game = new Game();

        for (int row = 0; row < 6; row++)
            game.place_token(row, 0, 1);

        Assert.False(game.is_valid(0));
    }

    [Fact]
    public void GetClosestRow_EmptyColumn_ReturnsBottomRow()
    {
        Game game = new Game();

        Assert.Equal(5, game.get_closest_row(0));
    }

    [Fact]
    public void GetClosestRow_PartiallyFilledColumn_ReturnsNextFreeRow()
    {
        Game game = new Game();

        game.place_token(5, 0, 1);
        game.place_token(4, 0, 2);

        Assert.Equal(3, game.get_closest_row(0));
    }

    [Fact]
    public void WinningToken_HorizontalWin_ReturnsTrue()
    {
        Game game = new Game();

        for (int col = 0; col < 4; col++)
            game.place_token(5, col, 1);

        Assert.True(game.winning_token(5, 3, 1));
    }

    [Fact]
    public void WinningToken_VerticalWin_ReturnsTrue()
    {
        Game game = new Game();

        for (int row = 2; row <= 5; row++)
            game.place_token(row, 0, 1);

        Assert.True(game.winning_token(2, 0, 1));
    }

    [Fact]
    public void WinningToken_PositiveDiagonalWin_ReturnsTrue()
    {
        Game game = new Game();

        game.place_token(5, 0, 1);
        game.place_token(4, 1, 1);
        game.place_token(3, 2, 1);
        game.place_token(2, 3, 1);

        Assert.True(game.winning_token(2, 3, 1));
    }

    [Fact]
    public void WinningToken_NegativeDiagonalWin_ReturnsTrue()
    {
        Game game = new Game();

        game.place_token(2, 0, 1);
        game.place_token(3, 1, 1);
        game.place_token(4, 2, 1);
        game.place_token(5, 3, 1);

        Assert.True(game.winning_token(5, 3, 1));
    }

    [Fact]
    public void WinningToken_ThreeInRow_ReturnsFalse()
    {
        Game game = new Game();

        game.place_token(5, 0, 1);
        game.place_token(5, 1, 1);
        game.place_token(5, 2, 1);

        Assert.False(game.winning_token(5, 2, 1));
    }

    [Fact]
    public void IsFull_FullBoard_ReturnsTrue()
    {
        Game game = new Game();

        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                game.place_token(row, col, 1);
            }
        }

        Assert.True(game.is_full());
    }

    [Fact]
    public void IsFull_EmptyBoard_ReturnsFalse()
    {
        Game game = new Game();

        Assert.False(game.is_full());
    }

    [Fact]
    public void Eval_CenterControl_GivesPositiveScore()
    {
        Game game = new Game();

        game.place_token(5, 3, 2);

        Assert.True(game.eval() > 0);
    }

    [Fact]
    public void Minimax_FindsWinningMove()
    {
        Game game = new Game();

        game.place_token(5, 0, 2);
        game.place_token(5, 1, 2);
        game.place_token(5, 2, 2);

        game.turn = 2;

        (double score, int move) =
            game.minimax(
                0,
                double.NegativeInfinity,
                double.PositiveInfinity
            );

        Assert.Equal(3, move);
    }
}