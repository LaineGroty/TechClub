using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors
{
    class Program
    {
        static void Main(string[] args)
        {
            Random r = new Random(); // Create random object
            Type rpsType = typeof(rps); // Store the type of rps for use in enum methods
            int count = Enum.GetValues(rpsType).Length; // Get the number of rock paper scissors elements

            string[] messages = { "Computer wins!", "It's a draw!", "Player wins!" };

            while(true)
            {
                Console.WriteLine("Select one: ");
                ListOptions(rpsType, count);

                int userChoice;
                do userChoice = AskForInt("Choice: "); // Get the user's choice
                while(userChoice < 0 || userChoice >= count); // Continue to ask the user for their choice until they enter a valid option

                int computerChoice = r.Next(count) + 1;

                int winner = Winner(userChoice, computerChoice, count);
                Console.WriteLine(messages[winner + 1]);
                Console.Write("Press any key to continue");
                Console.ReadKey();

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Print names of items in an enum
        /// </summary>
        /// <param name="enumType">The type of the enum to be listed</param>
        /// <param name="count">The number of elements to list. Must be less than or equal to the enum's maximum value</param>
        public static void ListOptions(Type enumType, int count, string prefix = "")
        {
            for(int i = 0; i < count; ++i)
                Console.WriteLine(prefix + $"({i + 1}) - {Enum.GetName(enumType, i)}"); // Write the current option
            
            // Same as Console.WriteLine("(" + (i + 1) + ") - " + Enum.GetName(rpsType, i));

            // ++i and i++ are the same in this context, but ++i is my preference
            // More info: https://stackoverflow.com/questions/3346450/what-is-the-difference-between-i-and-i
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

        /// <summary>
        /// Determine the winner of a RPS matchup
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="count"></param>
        /// <returns>-1 when a loses, 0 when a = b, 1 when a wins</returns>
        public static int Winner(int a, int b, int count)
        {
            /* In a given RPS game, any given object should be able to beat
             * half of the objects that are not itself. The object loses to
             * the remaining half. The number of objects that a given object
             * can beat is (n-1)/2, where n is the total number of objects.
             * For n = 3, this can be visualized as follows:
             *      __      __      __
             *      12312  12312  12312
             * 
             * The number at the leftmost point on the line represents the
             * given object. The number immediately to its right is the given
             * object's first bound. The number at the rightmost point on
             * the line represents the object's second bound. From the
             * object's bounds, the range in which the given object wins can
             * be determined. If the object is within the given object's
             * range, the given object wins. Otherwise, the other object wins.
             * 
             * However, when n % 2 = 0, a problem occurs. Since n = 4 and
             * the number of objects the given object can beat is equivelant
             * to (n-1)/2, the number of objects that can be beaten is 1 1/2.
             * An object cannot be half-beaten. This visualizes as follows:
             *      ___      ___      ___      ___  
             *      123412  123412  123412  123412
             *      
             * The line for 1 shows that 1 beats 2-3. The line for 3 shows
             * that it beats 4-1. The two lines contradict each other; 1
             * cannot silmutaneously win and lose to 3. The same thing
             * happens with the lines for 2 and 4: 2 beats 3-4 and 4 beats
             * 1-2. This contradiction occurs for all multiples of 2.
             * 
             * It took me three hours to figure this out.
             */

            if(a == b) return 0;

            int c = count / 2;
            Func<int, int> g = x =>
                x > count ? x - count : x; // Wrap x around so that it isn't greater than count

            int[] bounds = new int[] { g(a), g(a + c) };
            int lowerBound = bounds.Min();
            int upperBound = bounds.Max();

            if(lowerBound < b && b <= upperBound)
                return 1;
            return -1;
        }

        enum rps
        {
            Rock,
            Paper,
            Scissors,
            Water,
            Earth,
            Fire,
            Air
        }
    }
}
