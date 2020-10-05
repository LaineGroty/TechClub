using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe_Scalable
{
    class Program
    {
        static void Main(string[] args)
        {
            char emptySpace = '-';
            char player1Space = 'X';
            char player2Space = 'O';

            Board board = new Board(4, 4);
            Console.WriteLine($"Match {board.ContiguosSpacesToWin} spaces to win!\n");
            Console.WriteLine(board); // ToString() is implicit

            int state = 0;
            Board.SpaceEnum turn = (Board.SpaceEnum)1;
            while(state == 0)
            {
                // Say which player's turn it is
                Console.WriteLine($"--- {turn} ---");

                // Get user's position choice
                int x = 0, y = 0;
                do
                {
                    Console.WriteLine("Choose an empty space: ");
                    Console.Write("  X coordinate: ");
                    x = int.Parse(Console.ReadLine()) - 1;
                    Console.Write("  Y coordinate: ");
                    y = int.Parse(Console.ReadLine()) - 1;
                } while(board.GetSpace(x, y) != emptySpace); // Make the user enter positions until they enter one that is empty

                // If it is player 1's turn, set X. Otherwise, set O.
                board.SetSpace(x, y, turn);
                turn = (Board.SpaceEnum)((int)turn % 2 + 1);

                // Print board
                Console.WriteLine("\n" + board);

                // Flatten spaces into a 1D IEnumerable and check if all elements aren't the empty space
                if(board.Spaces.SelectMany(c => c).All(c => c != emptySpace))
                {
                    Console.WriteLine("Draw!");
                    return; // Escape Main(). This stops any more code in Main() from running and that's it.
                }

                // Check to see if either player has won
                state = board.GameState();
            }

            Console.WriteLine($"Player {state} wins!");
        }
    }

    class Board
    {
        // Properties / Fields --------------------------------------
        private char DefaultSymbol = new char();
        public List<List<char>> Spaces { get; set; }
        public int Height // Allows easy access to height of board
        {
            get => Spaces.Count;
            set
            {
                if(value < Height)
                    Spaces = Spaces.Take(value).ToList();
                else
                {
                    List<char> defaultLine = new List<char>();
                    for(int i = 0; i < value; ++i)
                        defaultLine.Add(DefaultSymbol);
                    for(int y = Height; y < value; ++y)
                        Spaces.Add(new List<char>(value != 0 ? value : 1).Select(s => DefaultSymbol).ToList());
                }
            }
        }
        public int Width // Allows easy access to width of board
        {
            get => Spaces[0].Count;
            set
            {
                for(int y = 0; y < Height; ++y)
                    if(value < Width)
                        Spaces[y] = Spaces[y].Take(value).ToList();
                    else
                        for(int x = Spaces[y].Count; x < value; ++x)
                            Spaces[y].Add(DefaultSymbol);
            }
        }
        public int ContiguosSpacesToWin { get; set; } // Number of spaces in a row to win
        public char Player1Symbol { get; set; }
        public char Player2Symbol { get; set; }

        // Find a better name for this
        public enum SpaceEnum
        {
            Empty,
            Player1,
            Player2
        }

        // Constructors ---------------------------------------------
        public Board()
        {
            Player1Symbol = 'X';
            Player2Symbol = 'O';
            DefaultSymbol = '-';

            Spaces = new List<List<char>>();
            Height = 3;
            Width = 3;
            SetAll(DefaultSymbol);
            ContiguosSpacesToWin = (int)Math.Sqrt((Width + 1) * (Height + 1));
        }
        
        public Board(char defaultValue) : this()
        {
            DefaultSymbol = defaultValue;
            SetAll(defaultValue);
        }
        
        public Board(int height, int width) : this()
        {
            Height = height;
            Width = width;
        }

        public Board(char player1Symbol, char player2Symbol, int contiguosSpacesToWin) : this()
        {
            Player1Symbol = player1Symbol;
            Player2Symbol = player2Symbol;
            ContiguosSpacesToWin = contiguosSpacesToWin;
        }

        public Board(char defaultValue, int height, int width, int contiguosSpacesToWin, char player1Symbol, char player2Symbol) : this(defaultValue)
        {
            Height = height;
            Width = width;
            ContiguosSpacesToWin = contiguosSpacesToWin;
            Player1Symbol = player1Symbol;
            Player2Symbol = player2Symbol;
        }

        public Board(List<List<char>> spaces) : this()
            => Spaces = spaces;

        // Methods --------------------------------------------------
        public void SetAll(char space)
        {
            for(int x = 0; x < Width; ++x)
                for(int y = 0; y < Height; ++y)
                    Spaces[x][y] = space;
        }
        
        public void ResetAll()
            => SetAll(DefaultSymbol);

        public void SetSpace(int x, int y, SpaceEnum space)
            => this[x, y] = space switch // C# 8+ feature
               {
                   (SpaceEnum)1 => Player1Symbol,
                   (SpaceEnum)2 => Player2Symbol,
                              _ => DefaultSymbol // "_" represents the default case
               };

        public char GetSpace(int x, int y, int xShift = 0, int yShift = 0) // Sets default values for xShift and yShift
        {
            int xPos = x + xShift;
            int yPos = y + yShift;

            if(xPos < Width && xPos >= 0 &&
               yPos < Height && yPos >= 0) // Check that the given point is within the bounds of the board
                return Spaces[yPos][xPos]; // Return the given point

            return DefaultSymbol; // Return character for empty board space
        }

        // TODO: Check that GameState() works on boards larger than 3x3
        public int GameState()
        {
            // Check rows
            for(int y = 0; y < Height; ++y)
            {
                // Store first space
                char s = this[0, y];
                // If c is an empty space, go to the next iteration of the loop
                if(s == DefaultSymbol) continue;

                int contiguos = 0;
                for(int x = 0; x < Width; ++x)
                    if(s == GetSpace(x, y)) // If current space in row is the same as the first
                        ++contiguos; // Increment contiguos

                if(contiguos >= ContiguosSpacesToWin)
                    return s == Player1Symbol ? 1 : 2; // Return 1 if c is X, return 2 otherwise
            }

            // Check columns
            for(int x = 0; x < Width; ++x)
            {
                char s = this[x, 0];
                if(s == DefaultSymbol) continue;

                int contiguos = 0;
                for(int y = 0; y < Height; ++y)
                    if(s == GetSpace(x, y))
                        ++contiguos;

                if(contiguos >= ContiguosSpacesToWin)
                    return s == Player1Symbol ? 1 : 2;
            }

            // Check diagonals
            for(int x = 0; x < Width; ++x)
            {
                char s = this[x, 0];
                // If c is an empty space, go to the next iteration of the loop
                if(s == DefaultSymbol) continue;

                int[] contiguos = new int[2]; // Contiguos space that are the same
                bool[] continueInDir = new bool[] { true, true }; // Bools representing whether the search should continue in a direction
                for(int shift = 1; shift < Height; ++shift)
                {
                    // Check down-left
                    if(continueInDir[0] && s == GetSpace(x - shift, shift)) // If c is the same as the space down and to the left
                        ++contiguos[0]; // Increment contiguos's "left"
                    else continueInDir[0] = false; // Do not continue to search left

                    // Check down-right
                    if(continueInDir[1] && s == GetSpace(x + shift, shift)) // If c is the same as the space down and to the right
                        ++contiguos[1]; // Increment contiguos's "right"
                    else continueInDir[1] = false; // Do not continue to search right

                    // If no space was found on this iteration, break
                    if(!continueInDir[0] && !continueInDir[1])  break;
                }

                if(contiguos.Max() >= ContiguosSpacesToWin)
                    return s == Player1Symbol ? 1 : 2;
            }

            return 0; // Means no player wins
        }

        // ToString() exists for all classes. To define a new behavior for ToString(), the "override" keyword must be used.
        public override string ToString()
        {
            string boardStr = "";
            foreach(List<char> row in Spaces)
                boardStr += string.Join("", row) + "\n";
            return boardStr;
        }

        // Indexers -------------------------------------------------
        public char this[int xIndex, int yIndex]
        {
            get => Spaces[yIndex][xIndex];
            set => Spaces[yIndex][xIndex] = value;
        }
        
        public List<char> this[int yIndex]
        {
            get => Spaces[yIndex];
            set => Spaces[yIndex] = value;
        }
    }
}
