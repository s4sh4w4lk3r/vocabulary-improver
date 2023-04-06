using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console
{
    class LearnWords
    {
        public static void Learn(Dictionary<string, string> dict)
        {
            for (int i = 0; i < dict.Count; i++)
            {

            }
            foreach (var item in dict)
            {
                Console.Write($"{item.Key} ---> ");
                Console.ReadLine();
                Console.WriteLine($"Correct answer ---> {item.Value}");
                Console.ReadKey(true);
            }
            Console.WriteLine("End of the dictionary.");
        }
    }
}
