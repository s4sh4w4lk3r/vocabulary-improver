using MongoDB.Bson.Serialization.Attributes;

namespace ViApi.Types.Common;

public class Word
{
    private Guid _guid;
    private string _sourceWord = null!;
    private string _targetWord = null!;
    private int _rating;
    private Guid _dictionaryGuid;

    public Guid Guid 
    {   
        get => _guid;
        set => _guid = value.Throw("В конструктор Word попал пустой Guid.").IfDefault().Value;
    }
    public string SourceWord
    {
        get => _sourceWord;
        set => _sourceWord = value.Throw("В конструктор Word попало пустое sourceWord.").IfNullOrWhiteSpace(_ => _).Value;
    }
    public string TargetWord
    {
        get => _targetWord;
        set => _targetWord = value.Throw("В конструктор Word попало пустое targetWord.").IfNullOrWhiteSpace(_ => _).Value;
    }
    public int Rating
    {
        get => _rating;
        set => _rating = value.Throw($"В конструктор Word попало недопустимое значение рейтинга: {value}.").IfGreaterThan(10).IfLessThan(0).Value;
    }
    public Guid DictionaryGuid
    {
        get => _dictionaryGuid;
        set => _dictionaryGuid = value.Throw("В конструктор Word попал пустой dictGuid.").IfDefault().Value;
    }
    [BsonIgnore] public Dictionary? Dictionary { get; set; }


    private Word() { }
    public Word(Guid guid, string sourceWord, string targetWord, Guid dictGuid, int rating = 0)
    {
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
