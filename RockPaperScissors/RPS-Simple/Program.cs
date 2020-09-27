using System;

namespace RPS_Simple
{
    class Program
    {
        static void Main(string[] args)
        {
            Random r = new Random(); // Create random object

            string[] messages = { "Computer wins!", "It's a draw!", "Player wins!" };
            while(true)
            {
                Console.WriteLine("Select one: ");
                Console.WriteLine("(1) - Rock");
                Console.WriteLine("(2) - Paper");
                Console.WriteLine("(3) - Scissors");

                int userChoice;
                do userChoice = AskForInt("Choice: ") - 1; // Get the user's choice
                while(userChoice < 0 || userChoice >= 3); // Continue to ask the user for their choice until they enter a valid option
                // Simplified but unsafe option:
                // int userChoice = int.Parse(Console.ReadLine());

                int computerChoice = r.Next(3); // Choose a random number from 0-3 exclusive
                Console.WriteLine("The computer chose " +
                    computerChoice switch
                    {
                        0 => "rock",
                        1 => "paper",
                        2 => "scissors",
                        _ => "unknown option"
                    }); // C# 8.0+ syntax

                /* The above is the same as:
                 * string computerChoiceStr;
                 * switch(computerChoice)
                 * {
                 *     case 0: computerChoiceStr = "rock"; break;
                 *     case 1: computerChoiceStr = "paper"; break;
                 *     case 2: computerChoiceStr = "scissors"; break;
                 * }
                 * Console.WriteLine("The computer chose " + computerChoiceStr);
                 */

                /* If userChoice + 1 (wrapping around to prevent userChoice
                 * from being 3) is the computer's choice, the user chose the
                 * option that beats the computer
                 * 
                 *     (1 (rock) + 1) % 3 = 2 (paper)
                 *    (2 (paper) + 1) % 3 = 0 (scissors)
                 * (3 (scissors) + 1) % 3 = 1 (rock)
                 */
                int winner;
                if((userChoice + 1) % 3 == computerChoice)
                    winner = 1; // Player wins
                else if(userChoice == computerChoice)
                    winner = 0; // Draw
                else
                    winner = -1; // Computer wins

                Console.WriteLine(messages[winner + 1]); // Give win/lose/draw message
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Prompt the user for an integer value until a valid integer is given
        /// </summary>
        /// <param name="prompt">The string to prompt the user with</param>
        /// <returns></returns>
        public static int AskForInt(string prompt)
        {
            int n;

            do Console.Write(prompt); // Print prompt to console
            while(!int.TryParse(Console.ReadLine(), out n)); // Try to parse the user's input. If the input is valid, stop and return the input. Otherwise, continue to prompt the user.

            return n;
        }
    }
}
