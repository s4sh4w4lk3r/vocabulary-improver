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
        FileSource?.ReduceRatingFile(key);
    }
    public void IncreaseRating(string key)
    {
        DBSource?.IncreaseRatingDB(key);
        FileSource?.IncreaseRatingFile(key);
    }
    public void ResetRatingAll()
    {
        foreach (Word item in DictList)
        {
            item.ResetRatingWord();
        }
    }
}
class Word
{
    public string Key {get; private set;}
    public string Value {get; private set;}
    public byte Rating {get; private set;} = 0;
    public Word(string key, string value, byte rating = 0)
    {
        this.Key = key;
        this.Value = value;
        this.Rating = rating;
    }
    public void ReduceRatingWord()
    {
        const byte minRating = 0;
        try
        {
            checked
            {
                Rating -= 1;
            }
        }
        catch (System.OverflowException)
        {
            Rating = minRating;
        }
    }
    public void IncreaseRatingWord()
    {
        const byte maxRating = 10;
        Rating += 1;
        if (Rating > 10) Rating = maxRating;
    }
    public void ResetRatingWord()
    {
        this.Rating = 0;
    }
}
    