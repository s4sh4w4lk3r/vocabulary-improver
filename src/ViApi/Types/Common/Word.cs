using MongoDB.Bson.Serialization.Attributes;
using Throw;

namespace ViApi.Types.Common;

public class Word
{
    public Guid Guid { get; set; }
    public string SourceWord { get; set; } = string.Empty;
    public string TargetWord { get; set; } = string.Empty;
    public int Rating { get; private set; }
    public Guid DictionaryGuid { get; set; }
    [BsonIgnore] public Dictionary? Dictionary { get; set; }


    private Word() { }
    public Word(Guid guid, string sourceWord, string targetWord, Guid dictGuid, int rating = 0)
    {
        guid.Throw("В конструктор Word попал пустой Guid.").IfDefault();
        dictGuid.Throw("В конструктор Word попал пустой dictGuid.").IfDefault();
        sourceWord.Throw("В конструктор Word попало пустое sourceWord.").IfNullOrWhiteSpace(_ => _);
        targetWord.Throw("В конструктор Word попало пустое targetWord.").IfNullOrWhiteSpace(_ => _);
        rating.Throw($"В конструктор Word попало недопустимое значение рейтинга: {rating}.").IfGreaterThan(10).IfLessThan(0);

        Guid = guid;
        SourceWord = sourceWord;
        TargetWord = targetWord;
        Rating = rating;
        DictionaryGuid = dictGuid;
    }

    public void IncreaseRating()
    {
        Rating = RatingEdtior.IncreaseRating(Rating);
    }
    public void DecreaseRating()
    {
        Rating = RatingEdtior.DecreaseRating(Rating);
    }

    public override string ToString() => $"Guid: {Guid}, Source-Target: {SourceWord} - {TargetWord}, {Rating}, DictGuid: {Guid}";

}
