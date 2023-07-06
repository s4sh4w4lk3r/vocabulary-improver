using System;

namespace ConsoleClient.sutff
{
    public class Word
    {
        private const int MAX_RATING = 10;
        private const int MIN_RATING = 0;
        public string SourceWord { get; private set; }
        public string TargetWord { get; set; }
        public Guid Guid { get; private set; }
        public int Rating { get; private set; }
        public Word(string sourceWord, string targetWord, Guid guid, int rating)
        {
            SourceWord = sourceWord;
            TargetWord = targetWord;
            Guid = guid;

            if (rating < 0 || rating > 10)
            {
                throw new Exception($"The value must be from 0 to 10 inclusive. Value = {rating}");
            }
            else
            {
                Rating = rating;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns true if overflowed, else false</returns>
        public bool Decrease()
        {
            if (Rating == MIN_RATING)
            {
                return true;
            }
            else
            {
                Rating--;
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns true if overflowed, else false</returns>
        public bool Increase()
        {
            if (Rating == MAX_RATING)
            {
                return true;
            }
            else
            {
                Rating++;
                return false;
            }
        }

        public override string ToString()
        {
            return $"{SourceWord}:{TargetWord}:{Rating}";
        }
    }
}