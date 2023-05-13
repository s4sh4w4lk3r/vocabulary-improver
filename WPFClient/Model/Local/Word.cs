using System;

namespace WPFClient
{
    public class Word
    {
        const int MAX_RATING = 10;
        const int MIN_RATING = 0;
        public string Word1 { get; private set; }
        public string Word2 { get; private set; }
        public Guid Guid { get; private set; }
        public int Rating { get; private set; }
        public Word(string word1, string word2, Guid guid, int rating)
        {
            Word1 = word1;
            Word2 = word2;
            Guid = guid;

            if (rating < 0 || rating > 10)
            {
                throw new VIException($"The value must be from 0 to 10 inclusive. Value = {rating}");
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
            return $"{Word1}:{Word2}:{Rating}";
        }
    }
}