namespace console;
partial class Input
{
    static Dictionary<string, string> ReadFile(string path)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
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
            try { dict.Add(word[0], word[1]); }
            catch (Exception) { }
        }

        return dict;
    }
    static Dictionary<string, string> Shuffle(Dictionary<string, string> dict)
    {
        return dict.OrderBy(x => new Random().Next()).ToDictionary(item => item.Key, item => item.Value);
    }
    public static Dictionary<string, string> GetDict(string path) => Shuffle(ReadFile(path));
}