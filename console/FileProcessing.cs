using Newtonsoft.Json;

namespace console;
class FileProcessing
{
    public string Path {get; private set;} // Путь до JSON файла.
    public List<Word>? Dict {get; private set;} // Список объектов Word.
    public List<Word>? ShuffledList // Свойство, перемешивающее список объектов Word для вывода.
    {
        get { return Dict?.OrderBy(x => new Random().Next()).ToList(); }
    }
    public FileProcessing(string path) // ctor, который создает объект класса, читает файл если он есть.
    {
        Path = path;
        if (File.Exists(path)) ReadFromJSON();
        else throw new Exception($"Error reading the file {Path}");
    }
    public static void CreateFile(string path) // Создает файл по полному пути.
    {
        File.Create(path).Close();
    }
    public void AddFromTxt(string path) // Чтение из тектосого файла в текущий список объектов Word, если файл не имеет ниодной строки, то ничего не произойдет.
    {
        Dict = new List<Word>();
        List<string> lines = new List<string>();
       
        lines = System.IO.File.ReadLines(path).ToList();
        if (lines.Count == 0) return;

        try 
        {
            foreach (var item in lines)
            {
                string[] word = item.Split(" - ");
                Dict.Add(new Word(word[0], word[1]));
            }
            SaveToJSON();
        }
        catch (System.IndexOutOfRangeException) 
        {
            throw new Exception($"Incorrect file formatting.\n{path}");
        }
    }
    public void SaveToJSON() // Сохранение текущего списка объектов Word в JSON.
    {
        string dictJSON = JsonConvert.SerializeObject(Dict, Formatting.Indented);
        File.WriteAllText(Path, dictJSON);
    }
    public void ReadFromJSON() // Чтение списка объектов Word из JSON.
    {

        using (StreamReader file = File.OpenText(Path))
        {
            JsonSerializer serializer = new JsonSerializer();
            Dict = (List<Word>)serializer.Deserialize(file, typeof(List<Word>))!;
        }
    }
    public void ReduceRatingFile(string key) // Уменьшение рейтинга слова.
    {
        Word? word = Dict?.Find(x => x.Key.Contains(key));
        word?.ReduceRatingWord();
        SaveToJSON();
    }
    public void IncreaseRatingFile(string key) // Увеличение рейтинга слова.
    {
        Word? word = Dict?.Find(x => x.Key.Contains(key));
        word?.ImproveRatingWord();
        SaveToJSON();
    }
    public void Add(string key, string value) //Добавить слово в JSON
    {
        Dict?.Add(new Word(key, value));
        SaveToJSON();
    }
    public void Remove(string key) // Удалить слово по ключу из JSON.
    {
        Word? word = Dict?.Find(x => x.Key.Contains(key));
        Dict?.Remove(word!); 
        SaveToJSON();
    }
}