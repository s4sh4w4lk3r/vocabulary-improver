using ViAPI.StaticMethods;

namespace ViAPI.Entities;

public class Word
{
    public Guid Guid { get; set; }
    public string SourceWord { get; set; } = string.Empty;
    public string TargetWord { get; set; } = string.Empty;
    public int Rating { get; private set; } = 0;
    
    public Guid DictionaryGuid { get; set; }
    public virtual ViDictionary? Dictionary { get; set; }

    public Word() { }
    public Word(Guid guid, string sourceWord, string targetWord, int rating = 0)
    {
        InputChecker.CheckRatingException(rating);
        InputChecker.CheckStringException(sourceWord, targetWord);
        InputChecker.CheckGuidException(guid);
        Guid = guid;
        SourceWord = sourceWord;
        TargetWord = targetWord;
        Rating = rating;
    }

    public override string ToString() => $"[Word] Guid: {Guid}, Source-Target: {SourceWord} - {TargetWord}, {Rating}, DictGuid: {Guid}";
    
    public void IncreaseRating()
    {
        if (Rating < 10) Rating++;
    }
    public void DecreaseRating()
    {
        if (Rating > 0) Rating--;
    }
}
