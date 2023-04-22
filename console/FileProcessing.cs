using Newtonsoft.Json;

namespace console;
class FileProcessing
{
    #region internal
    public string Path { get; private set; } // Путь до JSON файла.
    public List<Word>? Dict { get; private set; } // Список объектов Word.
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
    static void CreateFile(string path) // Создает файл по полному пути.
    {
        File.Create(path).Close();
    }
    public void AddFromTxt(string path) // Чтение из тектосого файла в текущий список объектов Word, если файл не имеет ниодной строки, то ничего не произойдет.
    {
        Dict = new List<Word>();


        List<string> lines = System.IO.File.ReadLines(path).ToList();
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
    #endregion
    #region console
    public static void ConsoleCreateFile()
    {
        System.Console.WriteLine();
        System.Console.Write("File name:");
        string path = Console.ReadLine()!;
        try
        {
            CreateFile(path);
            System.Console.WriteLine("File Created");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"An error occurred during file creation.\n{ex.Message}");
            Thread.Sleep(10000);
            throw;
        }
    }
    public static void ConsoleOpenFile()
    {
        System.Console.WriteLine("1 - Select a file in the current directory, 2 - Specify the file path");
        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.D1:
                FileProcessing.ScanDir();
                ConsoleOpenFile();
                break;
            case ConsoleKey.D2:
                FileProcessing.SpecifyDir();
                ConsoleOpenFile();
                break;
        }
    }
    static void ScanDir()
    {
        System.Console.WriteLine($"\nCurrent directory: {Directory.GetCurrentDirectory()}");

        string[] files = Directory.GetFiles(Directory.GetCurrentDirectory()).ToArray();

        for (int i = 0; i < files.Length; i++)
        {
            System.Console.WriteLine($"{i}: {System.IO.Path.GetFileName(files[i])}");
        }

        System.Console.Write("\nEnter file number: ");

        string fileNumberString = Console.ReadLine()!;

        int.TryParse(fileNumberString, out int fileNumberInt);

        if (fileNumberInt < 0 || fileNumberInt > files.Length - 1)
        {
            System.Console.WriteLine("\nBad file number.");
            ScanDir();
        }
        string filePath = files[fileNumberInt];

        try
        {
            FileProcessing file = new FileProcessing(filePath);
            FileSelected(file);
        }

        catch (Exception)
        {
            System.Console.WriteLine("Incorrect file formatting.");
            return;
        }
    }
    static void SpecifyDir()
    {
        System.Console.Write("Enter the path: ");
        string path = Console.ReadLine()!;
        try
        {
            FileProcessing file = new FileProcessing(path);
            FileSelected(file);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            return;
        }
    }
    static void FileSelected(FileProcessing file)
    {
        System.Console.WriteLine("1 - Start, 2 - Fill dictionary from file");
        if (Console.ReadKey(true).Key == ConsoleKey.D1)
        {
            if (file.ShuffledList is null)
            {
                System.Console.WriteLine("Dictionary is null. Exiting...");
                Improver.ShutDown();
            }
            VIDictionary vIDictionary = new VIDictionary(file, file.ShuffledList!);
            Improver improver = new Improver(vIDictionary);
            improver.Start();
        }
        if (Console.ReadKey(true).Key == ConsoleKey.D2)
        {
            System.Console.Write("Enter the path: ");
            string path = Console.ReadLine()!;
            try 
            {
                file.AddFromTxt(path);
                System.Console.WriteLine("OK");
                FileSelected(file);
            }
            catch
            {
                System.Console.WriteLine("Bad file.");
            }
        }
    }
    #endregion
}