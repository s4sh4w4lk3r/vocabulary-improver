using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf.Model.Local
{
    public class LocalProcessing
    {
        public event Action<string>? LocalProcessingEvent;
        public string Path { get; private set; } // Путь до JSON файла.
        public List<Word>? Dict { get; private set; } // Список объектов Word.
        public List<Word>? ShuffledList // Свойство, перемешивающее список объектов Word для вывода.
        {
            get { return Dict?.OrderBy(x => new Random().Next()).ToList(); }
        }
        public static void CreateFile(string path) // Создает файл по полному пути.
        {
            File.Create(path).Close();
        }
        public void ReadFromJSON() // Чтение списка объектов Word из JSON. Событие если словарь пуст.
        {
            using StreamReader file = File.OpenText(Path);
            JsonSerializer serializer = new JsonSerializer();
            Dict = (List<Word>)serializer.Deserialize(file, typeof(List<Word>))!;
            if (Dict?.Count == 0 || Dict is null) LocalProcessingEvent?.Invoke("This dictionary is empty.");
        }
        public void WriteToJSON() // Сохранение текущего списка объектов Word в JSON.
        {
            string dictJSON = JsonConvert.SerializeObject(Dict, Formatting.Indented);
            File.WriteAllText(Path, dictJSON);
        }
        public LocalProcessing(string path) // ctor, который создает объект класса, читает файл если он есть.
        {
            Path = path;
            if (File.Exists(path)) ReadFromJSON();
            else throw new VIException($"Error reading the file {Path}");
        }
        public void Add(string key, string value) //Добавить слово в JSON
        {
            Dict?.Add(new Word(key, value));
            WriteToJSON();
        }
        public void Remove(string key) // Удалить слово по ключу из JSON.
        {
            Word? word = Dict?.Find(x => x.Word1.Contains(key));
            Dict?.Remove(word!);
            WriteToJSON();
        }
        public void IncreaseRatingFile(string key) // Увеличение рейтинга слова.
        {
            Word? word = Dict?.Find(x => x.Word1.Contains(key));
            if (word != null) { word.Increase();}
            WriteToJSON();
        }
        public void ReduceRatingFile(string key) // Увеличение рейтинга слова.
        {
            Word? word = Dict?.Find(x => x.Word1.Contains(key));
            if (word != null) { word.Reduce(); }
            WriteToJSON();
        }
    }
}
