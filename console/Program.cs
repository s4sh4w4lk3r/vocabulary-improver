namespace console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var database = new DBProcessing("localhost", "3306", "admin", "admin", "vocabulary-improver", "en-ru");
            var dict = FileProcessing.GetDict("dict.vi");
            database.Add(dict);
            Console.ReadKey();
        }
    }
}