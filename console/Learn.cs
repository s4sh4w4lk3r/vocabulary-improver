using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console;

class Learn
{
    public static void Start(Dictionary<string, string> dict)
    {
        if (dict.Count == 0)
        {
            System.Console.WriteLine("Bad path or empty file.");
            Console.ReadKey();
            return;
        }
        Console.WriteLine($"Welcome! dictionary size is {dict.Count} words.");
        foreach (var item in dict)
        {
            Console.Write($"{item.Key} ---> ");
            Console.ReadLine();
            Console.WriteLine($"Correct answer ---> {item.Value}\n");
        }
        Console.WriteLine("End of the dictionary.");
        Console.ReadKey();
    }
}