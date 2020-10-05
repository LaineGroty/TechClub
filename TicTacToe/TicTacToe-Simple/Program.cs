using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe_Simple
{
    static class Program
    {
        static void Main()
        {
            List<List<char>> spaces = new List<List<char>>();
            // Create 3 rows with 3 columns
            for(int i = 0; i < 3; ++i)
                spaces.Add(new List<char>(new char[] { '-', '-', '-' }));

            int turn = 0; // Keep track of whose turn it is
            // Game loop
            while(spaces.BoardState() == 0)
            {
                // Say which player's turn it is
                Console.WriteLine("--- P" + ((turn % 2) + 1) + " ---");

                // Get user's position choice
                int x = 0, y = 0;
                do
                {
                    Console.WriteLine("Choose an empty space: ");
                    Console.Write("  X coordinate: ");
                    x = int.Parse(Console.ReadLine()) - 1;
                    Console.Write("  Y coordinate: ");
                    y = int.Parse(Console.ReadLine()) - 1;
                } while(spaces.GetSpace(x, y) != '-'); // Make the user enter positions until they enter one that is empty

                spaces[y][x] = turn % 2 == 0 ? 'X' : 'O'; // If it is player 1's turn, set X. Otherwise, set O.
                ++turn;

                // Print board
                foreach(List<char> row in spaces)
                {
                    foreach(char c in row)
                        Console.Write(c);
                    Console.WriteLine();
                }
                Console.WriteLine();
                // Alternatively:
                // foreach(List<char> row in spaces)
                //     Console.WriteLine(string.Join("", row));

                if(spaces.SelectMany(c => c).All(c => c != '-')) // Flatten spaces into a 1D IEnumerable and check if all elements are the empty space
                {
                    Console.WriteLine("Draw!");
                    return; // Escape Main(). This stops any more code in Main() from running and that's it.
                }
            }

            Console.WriteLine("Player " + spaces.BoardState() + " wins!");
            Console.ReadLine();
        }

        static int BoardState(this List<List<char>> spaces)
        {
            int winner = 0; // 0 = no winner

            // Check rows
            for(int y = 0; y < 3; ++y)
            {
                // Store first character
                char c = spaces[y][0];
                // If c is an empty space, go to the next iteration of the loop
                if(c == '-') continue;

                int contiguos = 0;
                for(int x = 0; x < 3; ++x)
                    if(c == spaces.GetSpace(x, y)) // If current space in row is the same as the first
                        ++contiguos; // Increment contiguos

                if(contiguos == 3)
                    return c == 'X' ? 1 : 2; // Return 1 if c is X, return 2 otherwise
            }

            // Check columns
            for(int x = 0; x < 3; ++x)
            {
                char c = spaces[0][x];
                if(c == '-') continue;

                int contiguos = 0;
                for(int y = 0; y < 3; ++y)
                    if(c == spaces.GetSpace(x, y))
                        ++contiguos;

                if(contiguos == 3)
                    return c == 'X' ? 1 : 2;
            }

            // Check diagonals
            // Note: It wouldn't be very difficult or even that bad to check the
            // diagonals individually, but they should be checked with for loops
            // so that the game can be easily expanded
            for(int x = 0; x < 3; ++x)
            {
                char c = spaces[0][x];
                // If c is an empty space, go to the next iteration of the loop
                if(c == '-') continue;

                char dir = '-'; // Direction of diagonal
                int contiguos = 1; // Contiguos characters that are the same
                for(int shift = 1; shift < 3; ++shift)
                {
                    // Check down-right
                    if((dir == 'r' || dir == '-') &&
                        c == spaces.GetSpace(x + shift, shift)) // If c is the same as the character down and to the right
                    {
                        ++contiguos; // Increment contiguos
                        if(dir == '-') // If the direction hasn't been set, set it to right
                            dir = 'r';
                        continue;
                    }
                    
                    // Check down-left
                    if((dir == 'l' || dir == '-') &&
                            c == spaces.GetSpace(x - shift, shift)) // If c is the same as the character down and to the left
                    {
                        ++contiguos; // Increment contiguos
                        if(dir == '-') // If the direction hasn't been set, set it to left
                            dir = 'l';
                        continue;
                    }
                }

                if(contiguos == 3)
                    return c == 'X' ? 1 : 2;
            }

            return winner;
        }

        static char GetSpace(this List<List<char>> spaces, int x, int y, int xShift = 0, int yShift = 0) // Sets default values for xShift and yShift
        {
            int xPos = x + xShift;
            int yPos = y + yShift;
            
            if(xPos < 3 && xPos >= 0 &&
               yPos < 3 && yPos >= 0) // Check that the given point is within the bounds of the board
                return spaces[yPos][xPos]; // Return the given point

            return '-'; // Return character for empty board space
        }
    }
}
