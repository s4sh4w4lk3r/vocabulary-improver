namespace console;
class Word
{
    public readonly string key = null!;
    public readonly string value = string.Empty;
    public readonly byte rating;
    public Word(string key, string value, byte rating)
    {
        this.key = key;
        this.value = value;
        this.rating = rating;
    }
    public Word(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}