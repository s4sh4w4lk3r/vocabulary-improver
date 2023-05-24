using ConsoleClient.sutff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    internal class GuessingGameLocal
    {
        private IWordProcessing LocalWordProcessing { get; set; }
        public GuessingGameLocal(string source)
        {
            LocalWordProcessing = new LocalWordProcessing(source);
        }

        public void Start()
        {
            while (true)
            {
                Console.WriteLine($"\nThe dictionary is selected. Its volume is {LocalWordProcessing.Words.Count} words. \nPlay - Enter, Edit dictionary - E");
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Enter:
                        Play();
                        break;
                    case ConsoleKey.E:
                        WordsEdtior();
                        break;
                }
            }
        }
        private void Play()
        {
            foreach (var item in LocalWordProcessing.ShuffledWords)
            {
                Console.Write($"\n\n{item.Word1} ---> ");
                Console.ReadLine();
                Console.Write($"Correct answer ---> {item.Word2}\nDid you answer correctly? y/n");
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Y:
                        LocalWordProcessing.IncreaseRating(item.Guid);
                        break;

                    case ConsoleKey.N:
                        LocalWordProcessing.DecreaseRating(item.Guid);
                        break;

                    default:
                        Console.WriteLine("Skipped");
                        break;
                }
                Console.WriteLine("Красавчик, харош.");
                Console.ReadKey();
            }
        }//OK
        private void WordsEdtior()
        {
            Console.WriteLine("\nAdd words directly - Insert, Add words from file - O, Remove word - Delete, Clear dictionary - C");
            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.Insert:
                    AddDirectly();
                    break;

                case ConsoleKey.O:
                    AddFromFile();
                    break;

                case ConsoleKey.Delete:
                    Remove();
                    break;

                case ConsoleKey.C:
                    LocalWordProcessing.Clear();
                    break;
            }
        }//OK
        private void AddFromFile()
        {
            Console.WriteLine("Enter file path: "); string path = Console.ReadLine()!;

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("Bad input.");
                return;
            }

            if (path.Contains('"'))
            {
                path = path.Replace("\"", string.Empty);
            }


            if (File.Exists(path))
            {
                LocalWordProcessing.AddWordsFromFile(path);
                Console.WriteLine("Added");
                return;
            }
            else
            {
                Console.WriteLine("File not found.");
                return;
            }
        }//OK
        private void AddDirectly() 
        {
            Console.Write("Enter source-word: ");
            string? word1 = Console.ReadLine();
            Console.Write("Enter target-word: ");
            string? word2 = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(word1) == false && string.IsNullOrWhiteSpace(word2) == false) 
            {
                LocalWordProcessing.Add(word1, word2);
                Console.WriteLine("Added");
            }
            else
            {
                Console.WriteLine("Bad input");
            }
        }//OK
        private void Remove()
        {
            Console.WriteLine("Words list to remove:");

            int i;
            for (i = 0; i < LocalWordProcessing.Words.Count; i++)
            {
                Console.WriteLine($"{i}\t{LocalWordProcessing.Words[i].ToString()}");
            }
            Console.Write("Enter word number: ");
            bool parseOK = int.TryParse(Console.ReadLine(), out int numToRemove);

            if (parseOK == true) 
            {
                if (numToRemove < 0 || numToRemove > LocalWordProcessing.Words.Count - 1)
                {
                    Console.WriteLine("Bad number.");
                }
                else
                {
                    Guid guidToRemove = LocalWordProcessing.Words[numToRemove].Guid;
                    LocalWordProcessing.Remove(guidToRemove);
                    Console.WriteLine("Removed");
                }
            }
            else
            {
                Console.WriteLine("Bad input.");
                return;
            }
        }//OK
    }
}
