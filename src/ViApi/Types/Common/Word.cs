using MongoDB.Bson.Serialization.Attributes;

namespace ViApi.Types.Common;

public class Word
{
    public Guid Guid { get; init; }
    public string? SourceWord { get; init; }
    public string? TargetWord { get; init; }
    public int Rating { get; private set; }
    public Guid DictionaryGuid { get; init; }
    [BsonIgnore] public Dictionary? Dictionary { get; init; } 

    private Word() { }
    public Word(Guid guid, string sourceWord, string targetWord, Guid dictGuid, int rating = 0)
    {
        Guid = guid;
        SourceWord = sourceWord;
        TargetWord = targetWord;
        Rating = rating;
        DictionaryGuid = dictGuid;
    }

    public override string ToString() => $"Guid: {Guid}, Source-Target: {SourceWord} - {TargetWord}, {Rating}, DictGuid: {Guid}";

    public void IncreaseRating() => Rating = RatingEdtior.IncreaseRating(Rating);
    public void DecreaseRating() => Rating = RatingEdtior.DecreaseRating(Rating);
}
