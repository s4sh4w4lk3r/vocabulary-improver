using ViAPI.Handlers;

namespace ViAPI.Entities
{
    public class Word
    {
        public Guid Guid { get; set; }
        public string SourceWord { get; set; }
        public string TargetWord { get; set; }
        public int Rating { get; private set; }
        public Word(Guid guid, string sourceWord, string targetWord, int rating)
        {
            InputExceptions.CheckRatingException(rating);
            InputExceptions.CheckStringException(sourceWord, targetWord);
            Guid = guid;
            SourceWord = sourceWord;
            TargetWord = targetWord;
            Rating = rating;
        }
    }
}
