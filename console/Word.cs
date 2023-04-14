namespace console;
class VIDictionary
{
    public DBProcessing? DBSource {get; private set;}
    public FileProcessing? FileSource {get; private set;}
    public List<Word> DictList {get; private set;}
    public VIDictionary(DBProcessing source, List<Word> dictList)
    {
        this.DictList = dictList;
        DBSource = source;
    }
    public VIDictionary(FileProcessing source, List<Word> dictList)
    {
        this.DictList = dictList;
        FileSource = source;
    }
    public void ReduceRating(string key)
    {
        DBSource?.ReduceRatingDB(key);
        a//добавить сюда такое же но с файлом
    }
    public void IncreaseRating(string key)
    {
        DBSource?.IncreaseRatingDB(key);
        a//добавить сюда такое же но с файлом
    }
}
class Word
{
    public string Key {get; private set;}
    public string Value {get; private set;}
    public byte Rating {get; private set;} = 0;
    public Word(string key, string value, byte rating)
    {
        this.Key = key;
        this.Value = value;
        this.Rating = rating;
    }
    public Word(string key, string value)
    {
        this.Key = key;
        this.Value = value;
    }
}