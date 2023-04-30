using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace wpf.backside.local
{
    public class Word : Rating
    {
        public string Word1 { get; private set; }
        public string Word2 { get; private set; }
        public Word(string word1, string word2, int rating = 0) : base(rating) // Конструктор слова и рейтинга
        {
            Word1 = word1;
            Word2 = word2;
        }
        public override string ToString()
        {
            return $"Key: {Word1} Value:{Word2} Rating: {Value}";
        }
    }
}
