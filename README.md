# Connect 4 Game
This is c# implementation of connect 4 with a minimax bot.

### How to run the game on terminal
```
git clone https://github.com/AliBayramov1/connect4.git

cd connect4

dotnet run
```
## How to play
- You are Player 1 (X)
- Bot is Player 2 (O)
- Enter a column number (0-6) to drop your token
- First to connect 4 wins

## Bot details
### Minimax search
**Function: (double, int) minimax(int depth, double alpha, double beta)**
- Recursively simulates the decision tree up to a maximum depth
### Alpha-Beta Pruning
**Function: (double, int) minimax(int depth, double alpha, double beta)**
- To avoid examining every single branch in the tree, algorithm keeps track of two values: alpha, beta
- Significantly Improves performance
### Evaluation
**Function: List<int> possible_moves()**
- Creates a priority for center column while checking valid moves

**Function: private int evaluate_window(int[] window)**
- scans the board horizontally, vertically, and diagonally
- Each window is assigned a strategic weight


## Game Rules
### How the game works
The game is played on a 6x7 board. Players take turns dropping a token into one of the 7 columns. The token falls to the lowest available space in that column.

### Winning conditions
A player wins if they connect four tokens in a row: Horizontally, vertically, or diagonally

### Restrictions
- You can only choose columns from **0 to 6**
- You cannot place a token in a full column