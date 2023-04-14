namespace console;
class FileProcessing
{
    static List<Word> ReadFile(string path)
    {
        List<Word> dict = new List<Word>();
        List<string> lines = new List<string>();
        try
        {
            lines = System.IO.File.ReadLines(path).ToList();
        }
        catch (System.IO.IOException)
        {
            return dict;
        }


        foreach (var item in lines)
        {
            string[] word = item.Split(" - ");
            dict.Add(new Word(word[0], word[1]));
        }

        return dict;
    }
    static List<Word> Shuffle(List<Word> dict)
    {
        return dict.OrderBy(x => new Random().Next()).ToList();;
    }
    public static List<Word> GetDict(string path) => Shuffle(ReadFile(path));
}