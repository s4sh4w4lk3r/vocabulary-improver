namespace console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var database = new DBProcessing("localhost", "3306", "admin", "admin", "vocabulary-improver", "en-ru");
            var dict = database.GetDict();
            var viDict = new VIDictionary(database, dict);
            viDict.ReduceRating("aids");
        }
    }
}