using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace console
{
    class FileProcessing
    {
        static Dictionary<string, string> ReadFile(string path)
        {
            string[] lines = File.ReadLines(path).ToArray();
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (var item in lines)
            {
                string[] word = item.Split(" - ");
                try { dict.Add(word[0], word[1]); }
                catch (Exception) { }
            }

            return dict;
        }
        static Dictionary<string, string> Shuffle(Dictionary<string, string> dict)
        {
            Dictionary<string, string> randDict = new Dictionary<string, string>();
            while (randDict.Count < dict.Count)
            {
                int randomIndex = new Random().Next(0, dict.Count);
                try { randDict.Add(dict.ElementAt(randomIndex).Key, dict.ElementAt(randomIndex).Value); }
                catch (Exception) { }
            }
            return randDict;
        }
        public static Dictionary<string, string> GetDict(string path) => Shuffle(ReadFile(path));
    }
}
