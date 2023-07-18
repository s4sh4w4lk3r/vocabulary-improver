using ViAPI.StaticMethods;

namespace ViAPI.Entites.DTO
{
    public class WordDto
    {
        public Guid Guid { get;}
        public string SourceWord { get; }
        public string TargetWord { get; }   
        public int Rating { get; }
        public WordDto(Guid guid, string sourceWord, string targetWord, int rating)
        {
            if (InputChecker.CheckRating(rating) is false) throw new ArgumentException("The rating value is not in the range from 0 to 10 inclusive.");
            InputChecker.CheckGuidException(guid);
            Guid = guid;
            SourceWord = sourceWord;
            TargetWord = targetWord;
            Rating = rating;
        }
    }
}
