using System;
using System.Collections.Generic;

class Game
{
    const int ROWS = 6;
    const int COLS = 7;
    const int MAX_DEPTH = 7;

    public int[,] board;
    int turn;
    public bool game_over;

    public Game()
    {
        board = new int[ROWS, COLS];
        turn = 1;
        game_over = false;
    }

    public void print_board()
    {
        Console.WriteLine();

        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < COLS; j++)
            {
                if (board[i, j] == 0)
                    Console.Write(". ");
                else if (board[i, j] == 1)
                    Console.Write("X ");
                else
                    Console.Write("O ");
            }

            Console.WriteLine();
        }

        Console.WriteLine("0 1 2 3 4 5 6");
        Console.WriteLine();
    }

    public bool is_valid(int col)
    {
        return board[0, col] == 0;
    }

    public int get_closest_row(int col)
    {
        for (int i = ROWS - 1; i >= 0; i--)
        {
            if (board[i, col] == 0)
            {
                return i;
            }
        }

        return -1;
    }

    public void place_token(int row, int col, int player)
    {
        board[row, col] = player;
    }

    public int make_move(int col, int player)
    {
        int row = get_closest_row(col);

        if (row != -1)
        {
            board[row, col] = player;
        }

        return row;
    }

    public void unmove(int row, int col)
    {
        board[row, col] = 0;
    }

    private int Count(int row, int col, int dr, int dc, int player)
    {
        int count = 0;

        while (
            row >= 0 &&
            row < ROWS &&
            col >= 0 &&
            col < COLS &&
            board[row, col] == player
        )
        {
            count++;
            row += dr;
            col += dc;
        }

        return count;
    }

    public bool winning_token(int row, int col, int player)
    {
        if (Count(row, col, 0, 1, player) +
            Count(row, col, 0, -1, player) - 1 >= 4)
            return true;

        if (Count(row, col, 1, 0, player) +
            Count(row, col, -1, 0, player) - 1 >= 4)
            return true;

        if (Count(row, col, 1, 1, player) +
            Count(row, col, -1, -1, player) - 1 >= 4)
            return true;

        if (Count(row, col, -1, 1, player) +
            Count(row, col, 1, -1, player) - 1 >= 4)
            return true;

        return false;
    }

    public int winner()
    {
        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col < COLS; col++)
            {
                int player = board[row, col];

                if (player == 0)
                    continue;

                if (winning_token(row, col, player))
                    return player;
            }
        }

        return 0;
    }

    public bool is_full()
    {
        for (int col = 0; col < COLS; col++)
        {
            if (is_valid(col))
                return false;
        }

        return true;
    }

    public List<int> possible_moves()
    {
        List<int> moves = new List<int>();

        // center-first ordering (stronger minimax)
        int[] order = { 3, 2, 4, 1, 5, 0, 6 };

        foreach (int col in order)
        {
            if (is_valid(col))
            {
                moves.Add(col);
            }
        }

        return moves;
    }

    private int evaluate_window(int[] window)
    {
        int score = 0;

        int bot = 2;
        int human = 1;

        int botCount = 0;
        int humanCount = 0;
        int emptyCount = 0;

        foreach (int cell in window)
        {
            if (cell == bot)
                botCount++;
            else if (cell == human)
                humanCount++;
            else
                emptyCount++;
        }

        if (botCount == 4)
            score += 100000;
        else if (botCount == 3 && emptyCount == 1)
            score += 100;
        else if (botCount == 2 && emptyCount == 2)
            score += 10;

        if (humanCount == 3 && emptyCount == 1)
            score -= 120;

        if (humanCount == 2 && emptyCount == 2)
            score -= 8;

        return score;
    }

    public double eval()
    {
        int score = 0;

        for (int row = 0; row < ROWS; row++)
        {
            if (board[row, 3] == 2)
                score += 6;
        }

        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col < COLS - 3; col++)
            {
                int[] window =
                {
                    board[row, col],
                    board[row, col + 1],
                    board[row, col + 2],
                    board[row, col + 3]
                };

                score += evaluate_window(window);
            }
        }

        for (int row = 0; row < ROWS - 3; row++)
        {
            for (int col = 0; col < COLS; col++)
            {
                int[] window =
                {
                    board[row, col],
                    board[row + 1, col],
                    board[row + 2, col],
                    board[row + 3, col]
                };

                score += evaluate_window(window);
            }
        }

        for (int row = 0; row < ROWS - 3; row++)
        {
            for (int col = 0; col < COLS - 3; col++)
            {
                int[] window =
                {
                    board[row, col],
                    board[row + 1, col + 1],
                    board[row + 2, col + 2],
                    board[row + 3, col + 3]
                };

                score += evaluate_window(window);
            }
        }

        for (int row = 3; row < ROWS; row++)
        {
            for (int col = 0; col < COLS - 3; col++)
            {
                int[] window =
                {
                    board[row, col],
                    board[row - 1, col + 1],
                    board[row - 2, col + 2],
                    board[row - 3, col + 3]
                };

                score += evaluate_window(window);
            }
        }

        return score;
    }

    public (double, int) minimax(
        int depth,
        double alpha,
        double beta
    )
    {
        int w = winner();

        if (w == 2)
            return (1000000 - depth, -1);

        if (w == 1)
            return (-1000000 + depth, -1);

        if (is_full())
            return (0, -1);

        if (depth >= MAX_DEPTH)
            return (eval(), -1);

        bool maximizing = (turn == 2);

        int bestMove = -1;

        if (maximizing)
        {
            double bestValue = double.NegativeInfinity;

            foreach (int move in possible_moves())
            {
                int row = make_move(move, turn);

                int oldTurn = turn;
                turn = 1;

                (double score, _) =
                    minimax(depth + 1, alpha, beta);

                turn = oldTurn;
                unmove(row, move);

                if (score > bestValue)
                {
                    bestValue = score;
                    bestMove = move;
                }

                alpha = Math.Max(alpha, bestValue);

                if (beta <= alpha)
                    break;
            }

            return (bestValue, bestMove);
        }
        else
        {
            double bestValue = double.PositiveInfinity;

            foreach (int move in possible_moves())
            {
                int row = make_move(move, turn);

                int oldTurn = turn;
                turn = 2;

                (double score, _) =
                    minimax(depth + 1, alpha, beta);

                turn = oldTurn;
                unmove(row, move);

                if (score < bestValue)
                {
                    bestValue = score;
                    bestMove = move;
                }

                beta = Math.Min(beta, bestValue);

                if (beta <= alpha)
                    break;
            }

            return (bestValue, bestMove);
        }
    }

    public void move()
    {
        int choice;

        // HUMAN
        if (turn == 1)
        {
            while (true)
            {
                Console.WriteLine("Choose your move (0-6):");

                string ans = Console.ReadLine();

                if (!int.TryParse(ans, out choice))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                if (choice < 0 || choice > 6)
                {
                    Console.WriteLine(
                        "Number must be between 0 and 6."
                    );
                    continue;
                }

                if (!is_valid(choice))
                {
                    Console.WriteLine(
                        "This column is full."
                    );
                    continue;
                }

                break;
            }

            int r = get_closest_row(choice);

            place_token(r, choice, 1);

            if (winning_token(r, choice, 1))
            {
                print_board();
                Console.WriteLine("You won!");
                game_over = true;
                return;
            }

            if (is_full())
            {
                print_board();
                Console.WriteLine("Draw!");
                game_over = true;
                return;
            }

            turn = 2;
        }

        else
        {
            Console.WriteLine("Bot is thinking...");

            (double score, int botMove) =
                minimax(
                    0,
                    double.NegativeInfinity,
                    double.PositiveInfinity
                );

            int r = get_closest_row(botMove);

            place_token(r, botMove, 2);

            Console.WriteLine(
                $"Bot played column {botMove}"
            );

            if (winning_token(r, botMove, 2))
            {
                print_board();
                Console.WriteLine("Bot won!");
                game_over = true;
                return;
            }

            if (is_full())
            {
                print_board();
                Console.WriteLine("Draw!");
                game_over = true;
                return;
            }

            turn = 1;
        }
    }
}

class Project
{
    static void Main()
    {
        Game game = new Game();

        while (!game.game_over)
        {
            game.print_board();
            game.move();
        }

        game.print_board();
    }
}