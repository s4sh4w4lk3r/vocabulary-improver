using ViAPI.Other;

namespace ViAPI.Entities;

public class Word
{
    public Guid Guid { get; set; }
    public string SourceWord { get; set; } = string.Empty;
    public string TargetWord { get; set; } = string.Empty;
    public int Rating { get; private set; } = 0;
    
    public Guid DictionaryGuid { get; set; }
    public virtual ViDictionary? Dictionary { get; set; }

    protected Word() { }
    public Word(Guid guid, string sourceWord, string targetWord, Guid dictGuid, int rating)
    {
        if (InputChecker.CheckRating(rating) is false) throw new ArgumentException("The rating value is not in the range from 0 to 10 inclusive.");
        InputChecker.CheckStringException(sourceWord, targetWord);
        InputChecker.CheckGuidException(guid);
        Guid = guid;
        SourceWord = sourceWord;
        TargetWord = targetWord;
        Rating = rating;
        DictionaryGuid = dictGuid;
    }

    public override string ToString() => $"[{GetType().Name}] Guid: {Guid}, Source-Target: {SourceWord} - {TargetWord}, {Rating}, DictGuid: {Guid}";
    
    public void IncreaseRating()
    {
        if (Rating < 10) Rating++;
    }
    public void DecreaseRating()
    {
        if (Rating > 0) Rating--;
    }
}
