using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console;


partial class Improver
{
    List<Word> dict;
    public Improver(List<Word> dictionary)
    {
        this.dict = dictionary;
    }
    public void Start()
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
            Console.Write($"{item.key} ---> ");
            Console.ReadLine();
            Console.WriteLine($"Correct answer ---> {item.value}\n");
        }
        Console.WriteLine("End of the dictionary.");
        Console.ReadKey();
    }
}