using ConsoleClient.sutff;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleClient
{
    public class LocalWordProcessing : IWordProcessing
    {
        private const int DEFAULT_RATING_VALUE = 0;

        public event Action<string>? WordProcessingLogging;

        public string Path { get; set; }

        private List<Word> words;
        [JsonIgnore]
        public List<Word> Words //OK
        {
            get 
            { 
                return words; 
            }
            private set 
            { 
                words = value; 
            }
        }
        [JsonIgnore]
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
        }
        
        public LocalWordProcessing(string path)
        {
            Path = path;
            words = new List<Word>();
            if (File.Exists(Path) == false)
            {
                throw new Exception($"The file with the path {Path} does not exist.");
            }
            Load();
        }

        public void Add(string word1, string word2)
        {
            Words.Add(new Word(word1, word2, Guid.NewGuid(), DEFAULT_RATING_VALUE));
            Save();
            WordProcessingLogging?.Invoke($"A pair of words have been added, Word1: {word1}, Word2: {word2}.");
        }//OK

        public void Add(List<Word> words)
        {
            Words.AddRange(words);
            Save();
            WordProcessingLogging?.Invoke($"A collection of words has been added.");
        }//OK

        public void DecreaseRating(Guid guid)
        {
            Word? word = Words.Find(x => x.Guid == guid);
            word?.Decrease();
            Save();
            if (word is null)
            {
                WordProcessingLogging?.Invoke($"The word by GUID {guid} was not found.");
            }
            else
            {
                WordProcessingLogging?.Invoke($"The rating of the word {word.Word1} has been decreased. Now it is {word.Rating}.");
            }

        }//OK

        public void IncreaseRating(Guid guid)
        {
            Word? word = Words.Find(x => x.Guid == guid);
            word?.Increase();
            Save();
            if (word is null)
            {
                WordProcessingLogging?.Invoke($"The word by GUID {guid} was not found.");
            }
            else
            {
                WordProcessingLogging?.Invoke($"The rating of the word {word.Word1} has been increased. Now it is {word.Rating}.");
            }
        }//OK

        public void Load()
        {
            Words = ViTools.ReadFromJSON<List<Word>>(Path) ?? Words;
        }

        public void AddWordsFromFile(string path)
        {
            if (File.Exists(path) == false)
            {
                throw new Exception("The file was not found.");
            }
            using (StreamReader reader = new StreamReader(path))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var word = line.Split(" - ");
                    Words.Add(new Word(word[0], word[1], Guid.NewGuid(), DEFAULT_RATING_VALUE));
                }
            }
            Save();
            WordProcessingLogging?.Invoke($"The download from the external file has been completed, the total number of words is {Words.Count}");
        }//OK

        public void Remove(string word1) // OK
        {
            int wordIndex = Words.FindIndex(x => x.Word1 == word1);
            if (wordIndex >= 0)
            {
                Words.RemoveAt(wordIndex);
                Save();
                WordProcessingLogging?.Invoke($"The word {word1} has been removed.");
            }
            else
            {
                WordProcessingLogging?.Invoke($"The word {word1} was not found to be deleted.");
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
                WordProcessingLogging?.Invoke($"The word {wordToDelete} with GUID {guid} has been removed.");
            }
            else
            {
                WordProcessingLogging?.Invoke($"The word with GUID {guid} was not found to be deleted.");
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
                WordProcessingLogging?.Invoke("All words by GUID were found and deleted.");
            }
            else if (processedGuids != guids.Count && processedGuids != 0)
            {
                WordProcessingLogging?.Invoke("Some words on the entered GUIDs were not deleted.");
            }
            else if (processedGuids == 0)
            {
                WordProcessingLogging?.Invoke("The words were not found by the entered GUIDs and were not deleted.");
            }
        }//OK

        public void Save()
        {
            ViTools.SaveToJSON(Words, Path);
            WordProcessingLogging?.Invoke($"Saved to JSON.");
        }//OK

        public void Edit(Guid guid, string word2)
        {
            Word? word = Words.Find(x => x.Guid == guid);
            if (word != null)
            {
                string oldWord2 = word.Word2;
                word.Word2 = word2;
                Save();
                WordProcessingLogging?.Invoke($"The word pair \"{word.Word1}:{oldWord2}\" with GUID {guid} has been edited to \"{word.Word1}:{word2}\".");
            }
            else
            {
                WordProcessingLogging?.Invoke($"A pair of words with GUID {guid} was not found.");
            }
        }//OK

        public void Edit(string word1, string word2)
        {
            Word? word = Words.Find(x => x.Word1 == word1);
            if (word != null)
            {
                string oldWord2 = word.Word2;
                word.Word2 = word2;
                Save();
                WordProcessingLogging?.Invoke($"The word pair \"{word.Word1}{oldWord2}\" has been changed to \"{word.Word1}:{word.Word2}\"");
            }
            else
            {
                WordProcessingLogging?.Invoke($"Word2 was not found by {word1}.");
            }
        }//OK
        public void Clear()
        {
            int wordsQuantity = Words.Count;
            Words.Clear();
            Save();
            WordProcessingLogging?.Invoke($"List cleared. {wordsQuantity} elements have been removed.");
        }

    }
}
