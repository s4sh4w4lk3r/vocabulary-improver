using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console;

partial class Improver
{
    VIDictionary vIDictionary;
    public Improver(VIDictionary vIDictionary)
    {
        this.vIDictionary = vIDictionary;
    }
    public void Start()
    {
        if (vIDictionary.DictList.Count == 0)
        {
            System.Console.WriteLine("Bad path or empty file.");
            Console.ReadKey();
            return;
        }
        Console.WriteLine($"Welcome! dictionary size is {vIDictionary.DictList.Count} words.");
        foreach (var item in vIDictionary.DictList)
        {
            System.Console.WriteLine();
            Console.Write($"{item.Key} ---> ");
            Console.ReadLine();
            Console.WriteLine($"Correct answer ---> {item.Value}\nDid you answer correctly? y/n");
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Y:
                vIDictionary.IncreaseRating(item.Key);
                break;

                case ConsoleKey.N:
                vIDictionary.ReduceRating(item.Key);
                break;

                default: Console.WriteLine("Skipped");
                break;
            }
        }
        Console.WriteLine("End of the dictionary.");
        Console.ReadKey();
    }
}