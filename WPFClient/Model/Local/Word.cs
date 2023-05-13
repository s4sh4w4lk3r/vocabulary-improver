using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WPFClient
{
    public class Word : Rating
    {
        public string Word1 { get; private set; }
        public string Word2 { get; private set; }
        public Guid Guid { get; private set; }
        public Word(string word1, string word2, Guid guid, int rating = 0) : base(rating) // Конструктор слова и рейтинга
        {
            Word1 = word1;
            Word2 = word2;
            Guid = guid;
        }
        public override string ToString()
        {
            return $"{Word1}:{Word2}:{Value}";
        }
    }
}
