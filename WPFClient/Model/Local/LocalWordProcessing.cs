using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFClient
{
    public class LocalWordProcessing : IWordProcessing
    {
        public event Action<string>? LocalProcessingLogging;
        public string Path { get; set; }

        private List<Word> words;

        public List<Word> Words //OK
        {
            get { return words; }
            set { words = value; }
        }

        public List<Word> ShuffledWords
        {
            get
            {
                if (Words.Count > 0)
                {
                    return Words.OrderBy(x => new Random().Next()).ToList();
                }
                else
                {
                    return new List<Word>();
                }
            }
            private set
            {
                this.ShuffledWords = value;
            }
        } //OK
        public LocalWordProcessing(string path)
        {
            Path = path;
            words = new List<Word>();
            if (File.Exists(Path) == false)
            {
                throw new VIException($"Еhe file with the path {Path} does not exist.");
            }
        }

        public void Add(string word1, string word2)
        {
            Words.Add(new Word(word1, word2, Guid.NewGuid()));
            Save();
            LocalProcessingLogging?.Invoke($"A couple of words have been added, Word1: {word1}, Word2: {word2}.");
        }//OK

        public void Add(List<Word> words)
        {
            Words.AddRange(words);
            Save();
            LocalProcessingLogging?.Invoke($"A collection of words has been added.");
        }//OK

        public void DecreaseRating(Guid guid)
        {
            Word? word = Words.Find(x => x.Guid == guid);
            word?.Decrease();
            Save();
            if (word is null)
            {
                LocalProcessingLogging?.Invoke($"The word by GUID {guid} was not found.");
            }
            else
            {
                LocalProcessingLogging?.Invoke($"The rating of the word {word.Word1} has been decreased. Now it is {word.Value}.");
            }
        }

        public void IncreaseRating(Guid guid)
        {
            Word? word = Words.Find(x => x.Guid == guid);
            word?.Increase();
            Save();
            Load();
            if (word is null)
            {
                LocalProcessingLogging?.Invoke($"The word by GUID {guid} was not found.");
            }
            else
            {
                LocalProcessingLogging?.Invoke($"The rating of the word {word.Word1} has been increased. Now it is {word.Value}.");
            }
        }

        public void Load()
        {
            using StreamReader sr = File.OpenText(Path);
            JsonSerializer serializer = new JsonSerializer();

            if (serializer.Deserialize(sr, typeof(List<Word>)) is List<Word> loadedWords)
            {
                this.Words = loadedWords;
                LocalProcessingLogging?.Invoke($"The words were loaded, the number of words is {Words.Count}");
            }
        }

        public void LoadFromFile(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var word = line.Split(" - ");
                    Words.Add(new Word(word[0], word[1], Guid.NewGuid()));
                }
            }
            Save();
            LocalProcessingLogging?.Invoke($"The download from the external file has been completed, the total number of words is {Words.Count}");
        }//OK

        public void Remove(string word1) // OK
        {
            int wordIndex = Words.FindIndex(x => x.Word1 == word1);
            if (wordIndex >= 0)
            {
                Words.RemoveAt(wordIndex);
                Save();
                LocalProcessingLogging?.Invoke($"The word {word1} has been removed.");
            }
            else
            {
                LocalProcessingLogging?.Invoke($"The word {word1} was not found to be deleted.");
            }
        }

        public void Remove(Guid guid)
        {
            int wordIndex = Words.FindIndex(x => x.Guid == guid);
            if (wordIndex >= 0)
            {
                string wordToDelete = Words[wordIndex].Word1;
                Words.RemoveAt(wordIndex);
                Save();
                LocalProcessingLogging?.Invoke($"The word {wordToDelete} with GUID {guid} has been removed.");
            }
            else
            {
                LocalProcessingLogging?.Invoke($"The word with GUID {guid} was not found to be deleted.");
            }
        } // OK

        public void Remove(List<Guid> guids)
        {
            int processedGuids = 0;
            foreach (var guid in guids)
            {
                int wordIndex = Words.FindIndex(x => x.Guid == guid);
                if (wordIndex >= 0)
                {
                    Words.RemoveAt(wordIndex);
                    processedGuids++;
                }
            }
            Save();
            if (processedGuids == guids.Count)
            {
                LocalProcessingLogging?.Invoke("All words by GUID were found and deleted.");
            }
            else if (processedGuids != guids.Count && processedGuids != 0)
            {
                LocalProcessingLogging?.Invoke("Some words on the entered GUIDs were not deleted.");
            }
            else if (processedGuids == 0)
            {
                LocalProcessingLogging?.Invoke("The words were not found by the entered GUIDs and were not deleted.");
            }
        }//OK

        public void Save()
        {
            string WordsJSON = JsonConvert.SerializeObject(Words, Formatting.Indented);
            File.WriteAllText(Path, WordsJSON);
            LocalProcessingLogging?.Invoke($"The current object was saved by {Path}");
        }
    }
}
