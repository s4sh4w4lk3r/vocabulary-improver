using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient.sutff
{
    class WordProcessor
    {
        private const string OFFLINE_SOURCE_TAG = "[OFFLINE]";
        private const string ONLINE_SOURCE_TAG = "[ONLINE]";
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }

        public WordProcessor(Guid guid, string name, string description, string source)
        {
            bool a = guid == Guid.Empty;
            bool b = string.IsNullOrWhiteSpace(name);
            bool c = string.IsNullOrWhiteSpace(description);
            bool d = string.IsNullOrWhiteSpace(source);
            if (a || b || c || d)
            {
                throw new Exception("Null or empty strings");
            }

            Guid = guid;
            Name = name;
            Description = description;
            Source = source;
        }

        public void StartIWordProcessing()
        {

            if (Source.Contains(ONLINE_SOURCE_TAG))
            {
                string sourceOnline = Source.Replace(ONLINE_SOURCE_TAG, string.Empty);
                throw new NotImplementedException();
            }

            if (Source.Contains(OFFLINE_SOURCE_TAG))
            {
                string path = Source.Replace(OFFLINE_SOURCE_TAG, string.Empty);
                GuessingGameLocal game = new(path);
                game.Start();
            }
        }
        public static void AddNewProcessor(string name, string description, string? onlineSource = null)
        {
            if (File.Exists(ViTools.ViDictsListFilePath) == false)
            {
                throw new Exception("JSON dicts list not exists.");
            }

            List<WordProcessor> JSONlist = ViTools.ReadFromJSON<List<WordProcessor>>(ViTools.ViDictsListFilePath) ?? new List<WordProcessor>();




            if (onlineSource != null)
            {
                JSONlist.Add(new WordProcessor(Guid.NewGuid(), name, description, ONLINE_SOURCE_TAG + onlineSource));
            }
            if (onlineSource == null)
            {
                string path = Path.Combine(ViTools.ViDictsDirectory, $"{name}.json");
                JSONlist.Add(new WordProcessor(Guid.NewGuid(), name, description, OFFLINE_SOURCE_TAG + path));
            }
            ViTools.SaveToJSON(JSONlist, ViTools.ViDictsListFilePath);
        }

        public static void RemoveWordProcessor(Guid guidToRemove)
        {
            if (File.Exists(ViTools.ViDictsListFilePath) == false)
            {
                throw new Exception("JSON dicts list not exists.");
            }

            List<WordProcessor> JSONlist = ViTools.ReadFromJSON<List<WordProcessor>>(ViTools.ViDictsListFilePath) ?? new List<WordProcessor>();
            JSONlist.RemoveAll(x => x.Guid == guidToRemove);
            ViTools.SaveToJSON(JSONlist, ViTools.ViDictsListFilePath);
        }
        public static List<WordProcessor> GetWordProcessors()
        {
            List<WordProcessor> JSONlist = ViTools.ReadFromJSON<List<WordProcessor>>(ViTools.ViDictsListFilePath) ?? new List<WordProcessor>();
            return JSONlist;
        }
        public override string ToString() => $"{Guid}:{Name}:{Description}:{Source}";
    }
}
