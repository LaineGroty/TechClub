using System;
using System.Linq;

namespace PigLatin
{
    static class Program
    {
        static void Main(string[] args)
        {
            // If an argument has been passed, run the pig latin method on the first one
            if(args.Length > 0)
                Console.WriteLine(args[0].PigLatin());
            // Otherwise, ask the user to enter text to convert until the program is closed
            else
                while(true)
                {
                    Console.Write("Enter text: ");
                    string text = Console.ReadLine();
                    Console.WriteLine(text.PigLatin());
                }
        }

        public static string PigLatin(this string str)
        {
            string vowels = "aeiouAEIOU"; // Define vowels
            string suffix = "ay"; // Define a suffix. Doing this makes it easier to apply changes to the program.

            string @out = ""; // Output string. The "@" disambiguates the variable from the keyword
            // Loop through all words in the string, splitting at every space and removing empty splits
            foreach(string word in str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                // If the first letter is a vowel, just add the word
                if(vowels.Contains(word[0]))
                    @out += word;
                // Otherwise, add the word with its first consonant group moved to the end
                else
                    @out += new string(word.SkipWhile(c => !vowels.Contains(c)).ToArray()) +
                            new string(word.TakeWhile(c => !vowels.Contains(c)).ToArray());
                // Add the suffix and a space
                @out += suffix + " ";
            }

            // Return @out without any leading or trailing whitespace
            return @out.Trim();
        }
    }
}
