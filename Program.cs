using System;
using System.Collections.Generic;

/// <summary>
/// Represents a Connect Four game with player vs bot
/// </summary>
class Game
{
    const int ROWS = 6;
    const int COLS = 7;
    const int MAX_DEPTH = 7;

    public int[,] board;
    int turn;
    public bool game_over;

    /// <summary>
    /// Initializes a new Connect Four game
    /// Sets up a 6x7 board, assigns player 1 as the starting player, and sets game over to false
    /// </summary>
    public Game()
    {
        board = new int[ROWS, COLS];
        turn = 1;
        game_over = false;
    }

    /// <summary>
    /// Displays the current biard state
    /// </summary>
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

    /// <summary>
    /// Checks if column is valid for a move
    /// A column is valid if the top row is empty
    /// </summary>
    /// <param name="col">Column index (0-6)</param>
    /// <returns>True if token can be placed, false otherwise</returns>
    public bool is_valid(int col)
    {
        return board[0, col] == 0;
    }

    /// <summary>
    /// Finds the lowest available row in a given column
    /// </summary>
    /// <param name="col">Column index</param>
    /// <returns>The row index where token will land, or -1 if full</returns>
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

    /// <summary>
    /// Places a token for a player at a specific position
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="col">Column index</param>
    /// <param name="player">Player ID (1 - player, 2 - bot)</param>
    public void place_token(int row, int col, int player)
    {
        board[row, col] = player;
    }

    /// <summary>
    /// Simulates move for bot without modifying game state
    /// </summary>
    /// <param name="col">Column indec</param>
    /// <param name="player">Player making the move</param>
    /// <returns>The row where the token was placed</returns>
    public int make_move(int col, int player)
    {
        int row = get_closest_row(col);

        if (row != -1)
        {
            board[row, col] = player;
        }

        return row;
    }

    /// <summary>
    /// Undoes a previously simulated move
    /// </summary>
    /// <param name="row">Row index of removed token</param>
    /// <param name="col">Column index</param>
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

    /// <summary>
    /// Checks if placing a token results in a win
    /// </summary>
    /// <param name="row">Row index of last move</param>
    /// <param name="col">Column index</param>
    /// <param name="player">Plaer ID</param>
    /// <returns>True if player has four in a row, false otherwise</returns>
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

    /// <summary>
    /// Determines winner of the game
    /// </summary>
    /// <returns>0 if no winner, 1 if player wins, 2 if bot wins</returns>
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

    /// <summary>
    /// Checks if the board is full
    /// </summary>
    /// <returns>True if there are no valid moves, false otherwise</returns>
    public bool is_full()
    {
        for (int col = 0; col < COLS; col++)
        {
            if (is_valid(col))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Generates a list of all valid moves
    /// </summary>
    /// <returns>Playable column indices</returns>
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

    /// <summary>
    /// Evaluates a set of four consecutive cells for the bot
    /// </summary>
    /// <param name="window">Array of 4 board positions</param>
    /// <returns>A score representing position strength</returns>
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

    /// <summary>
    /// Computes a score of the curret board state
    /// </summary>
    /// <returns>A score where positive value is better for bot</returns>
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

    /// <summary>
    /// Performs minimax search with alpha-beta pruning to determine the best move
    /// </summary>
    /// <param name="depth">Current recursion depth</param>
    /// <param name="alpha">Bets score for maximizer</param>
    /// <param name="beta">Best score for minimizer</param>
    /// <returns>Tuple containing: best evaluation score, best column move</returns>
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

    /// <summary>
    /// Executes a single turn in the game
    /// Handles player input , bot decision, board update, and win/draw detection
    /// </summary>
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

/// <summary>
/// Initializes the game and runs the main game loop until completion
/// </summary>
class Project
{
    /// <summary>
    /// Main method that starts the Connect Four game
    /// </summary>
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