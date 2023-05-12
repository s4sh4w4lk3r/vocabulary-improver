using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Model.sutff.Local.Enties;

namespace WPFClient.Model
{
    internal interface IWordProcessing
    {
        public List<Word> Words { get; set; }
        public List<Word> ShuffledWords { get; set; }

        public void Save();
        public void Load();
        public void LoadFromFile(string path);

        public void IncreaseRating(Guid guid);
        public void DecreaseRating(Guid guid);

        public void Add(string word1, string word2);
        public void Add(List<Word> words);

        public void Remove(string word1);
        public void Remove(Guid guid);
        public void Remove(List<Guid> guids);


    }
}
