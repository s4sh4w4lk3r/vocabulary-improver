using ConsoleClient.sutff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    internal class ConsoleStartup
    {
        #region Singleton
        private static ConsoleStartup instance = null!;

        private ConsoleStartup()
        { }

        public static ConsoleStartup GetInstance()
        {
            if (instance == null)
                instance = new ConsoleStartup();
            return instance;
        }
        #endregion

        private List<WordProcessor> processorList = new();

        public void Start()
        {
            while (true)
            {
                Console.WriteLine(new string('*', 120));
                Console.WriteLine("Select an action.\nPlay - Enter, Add - Insert, Remove - Del, Exit - Pause");
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine(new string('*', 120));
                        SelectProcessor();
                        break;
                    case ConsoleKey.Pause:
                        Environment.Exit(0);
                        break;
                    case ConsoleKey.Insert:
                        Console.WriteLine(new string('*', 120));
                        AddProcessor();
                        break;
                    case ConsoleKey.Delete:
                        Console.WriteLine(new string('*', 120));
                        RemoveProcessor();
                        break;
                }
            }
        }
        public void AddProcessor()
        {
            string? name;
            string? desc;
            string? sourceInput;
            bool isOnline;

            Console.Write("name: "); name = Console.ReadLine();
            Console.Write("desc: "); desc = Console.ReadLine();
            Console.Write("source: "); sourceInput = Console.ReadLine();
            Console.Write("Is online (y/n): "); isOnline = IsOnlineSelector(Console.ReadKey().Key);
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(desc) || string.IsNullOrEmpty(sourceInput))
            {
                throw new Exception("name or desc is null or empty");
            }
            else
            {
            WordProcessor.AddNewProcessor(name, desc, sourceInput, isOnline);
            }
            Console.WriteLine();
        }//OK
        public void SelectProcessor()
        {
            Console.WriteLine("Scanning dicts...");
            processorList = WordProcessor.GetWordProcessors();
            Console.WriteLine($"Dictionaries found: {processorList.Count}");
            int i = 0;
            for (i = 0; i < processorList.Count; i++)
            {
                Console.WriteLine($"{i} \t {processorList[i].Name}");
            }
            Console.Write("Enter dictionary number to play: ");
            int.TryParse(Console.ReadLine(), out int numToPlay);
            processorList[numToPlay].StartIWordProcessing();
            Console.WriteLine();
        }
        public void RemoveProcessor()
        {
            Console.WriteLine("Scanning dicts...");
            processorList = WordProcessor.GetWordProcessors();
            if (processorList.Count == 0)
            {
                Console.WriteLine("Nothing to remove.");
                return;
            }
            Console.WriteLine($"Dictionaries found: {processorList.Count}");
            int i = 0;

            for (i = 0; i < processorList.Count; i++)
            {
                Console.WriteLine($"{i} \t {processorList[i].Name}");
            }
            Console.Write("Enter dictionary number to remove: ");
            int.TryParse(Console.ReadLine(), out int numToDelete);

            Guid guidToRemove = processorList[numToDelete].Guid;
            WordProcessor.RemoveWordProcessor(guidToRemove);
            Console.WriteLine();
        }//OK
        private static bool IsOnlineSelector(ConsoleKey isOnlineKey)
        {
            if (isOnlineKey == ConsoleKey.Y)
            {
                return true;
            }
            else if (isOnlineKey == ConsoleKey.N)
            {
                return false;
            }
            else { throw new Exception("Bad key (isOnline)."); }
        }
    }
}
