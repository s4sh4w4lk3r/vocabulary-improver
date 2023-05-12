using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFClient.Model.sutff;
using WPFClient.Model.sutff.Local.Enties;

namespace WPFClient.Model
{
    internal class LocalWordProcessing : IWordProcessing
    {
        // TODO : Сделать события
        public string Path { get; set; }
        public List<Word> Words { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<Word> ShuffledWords { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public LocalWordProcessing(string path) 
        {
            Path = path;
            Words = new List<Word>();
            if (File.Exists(Path) == false)
            { 
                throw new VIException($"Еhe file with the path {Path} does not exist.");
            }
            Load();
        }

        public void Add(string word1, string word2)
        {
            Words.Add(new Word(word1, word2));
        }

        public void Add(List<Word> words)
        {
            Words.AddRange(words);
        }

        public void DecreaseRating(Guid guid)
        {
            Word? word = Words.Find(x => x.Guid == guid);
            word?.Decrease();
            Save();
        }

        public void IncreaseRating(Guid guid)
        {
            Word? word = Words.Find(x => x.Guid == guid);
            word?.Increase();
            Save();
        }

        public void Load()
        {
            using StreamReader sr = File.OpenText(Path);
            JsonSerializer serializer = new JsonSerializer();
            var seralizerOutput = serializer.Deserialize(sr, typeof(List<Word>));
            Words = (List<Word>)seralizerOutput! ?? Words;
            /*if (Dict?.Count == 0) LocalProcessingEvent?.Invoke("This dictionary is empty.");*/
        }

        public void LoadFromFile(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var word = line.Split(" - ");
                    Words.Add(new Word(word[0], word[1]));
                }
            }
            Save();
        }

        public void Remove(string word1)
        {
            var wordIndex = Words.FindIndex(x => x.Word1 == word1);
            if (wordIndex >= 0)
            {
                Words.RemoveAt(wordIndex);
            }
            Save();
        }

        public void Remove(Guid guid)
        {
            var wordIndex = Words.FindIndex(x => x.Guid == guid);
            if (wordIndex >= 0)
            {
                Words.RemoveAt(wordIndex);
            }
            Save();
        }

        public void Remove(List<Guid> guids)
        {
            foreach (var guid in guids)
            {
                int wordIndex = Words.FindIndex(x => x.Guid == guid);
                if (wordIndex >= 0)
                {
                    Words.RemoveAt(wordIndex);
                }
            }
            Save();
        }

        public void Save()
        {
            string WordsJSON = JsonConvert.SerializeObject(Words, Formatting.Indented);
            File.WriteAllText(Path, WordsJSON);
        }
    }
}
