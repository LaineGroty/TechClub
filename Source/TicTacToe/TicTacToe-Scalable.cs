using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe_Scalable
{
    class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board();
            // Check if user has given size arguments
            if(args.Length >= 2)
                // Try to set board size from args
                try
                {
                    board.Width = int.Parse(args[0]);
                    board.Height = int.Parse(args[1]);
                }
                catch
                {
                    throw new ArgumentException("Invalid arguments");
                }

            Console.WriteLine($"Match {board.ContiguosSpacesToWin} spaces to win!\n");
            Console.WriteLine(board); // ToString() is implicit

            // This is called a local function
            static SpaceEnum nextTurn(SpaceEnum space) => (SpaceEnum)((int)space % 2 + 1);

            SpaceEnum turn = (SpaceEnum)1;
            while(board.GameState() == 0)
            {
                // Say which player's turn it is
                Console.WriteLine($"--- {turn} ---");

                // Get user's position choice
                int[] pos = board.AskSpace();

                // If it is player 1's turn, set X. Otherwise, set O.
                board.SetSpace(pos[0], pos[1], turn);
                turn = nextTurn(turn);

                // Print board
                Console.WriteLine("\n" + board);

                // Flatten spaces into a 1D IEnumerable and check if all elements aren't the empty space
                if(board.Spaces.SelectMany(c => c).All(c => c != board.DefaultSymbol))
                {
                    Console.WriteLine("Draw!");
                    return; // Escape Main(). This stops any more code in Main() from running and that's it.
                }
            }

            Console.WriteLine($"{nextTurn(turn)} wins!");
        }
    }

    class Board
    {
        // Properties / Fields ----------------------------------------------------------
        private char defaultSymbol;
        public char DefaultSymbol { get => defaultSymbol; }
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

        // Constructors -----------------------------------------------------------------
        // IMPORTANT NOTE: Misuse of regions can make code much more difficult to read. Only use regions to improve code legibility.
        #region Constructors
        public Board()
        {
            Player1Symbol = 'X';
            Player2Symbol = 'O';
            defaultSymbol = '-';

            Spaces = new List<List<char>>();
            Height = 3;
            Width = 3;
            SetAll(defaultSymbol);
            ContiguosSpacesToWin = (int)Math.Sqrt((Width + 1) * (Height + 1));
        }
        
        public Board(char defaultSymbol) : this()
        {
            this.defaultSymbol = defaultSymbol;
            SetAll(defaultSymbol);
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
        #endregion

        // Methods ----------------------------------------------------------------------
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

        public int[] AskSpace()
        {
            int x, y;
            do
            {
                Console.WriteLine("Choose an empty space: ");
                Console.Write("  X coordinate: ");
                x = int.Parse(Console.ReadLine()) - 1;
                Console.Write("  Y coordinate: ");
                y = int.Parse(Console.ReadLine()) - 1;
            } while(GetSpace(x, y) != DefaultSymbol); // Make the user enter positions until they enter one that is empty

            return new int[] { x, y };
        }

        // TODO: Check that GameState() works on boards larger than 4x4
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
                for(int shift = 0; shift < Height; ++shift)
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
                    if(!continueInDir[0] && !continueInDir[1]) break;
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

        // Indexers ---------------------------------------------------------------------
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

    // Find a better name for this
    public enum SpaceEnum
    {
        Empty,
        Player1,
        Player2
    }
}
