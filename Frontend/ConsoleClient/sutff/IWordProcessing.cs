using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient.sutff
{
    interface IWordProcessing
    {
        public event Action<string>? WordProcessingLogging;
        public List<Word> Words { get; }
        public List<Word> ShuffledWords { get; }
        public void Save();
        public void Load();

        public void IncreaseRating(Guid guid);
        public void DecreaseRating(Guid guid);

        public void Add(string word1, string word2);
        public void Add(List<Word> words);
        public void AddWordsFromFile(string path);

        public void Edit(Guid guid, string word2); //Find word by GUID and change its word2.
        public void Edit(string word1, string word2);//Find word by word1 and change its word2.

        public void Remove(string word1);
        public void Remove(Guid guid);
        public void Remove(List<Guid> guids);
        public void Clear();
    }
}
