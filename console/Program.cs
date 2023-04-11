namespace console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // string connString = "server=localhost;port=3306;username=admin;password=admin;database=vocabulary-improver";
            // string tableName = "en-ru";
            string path = "dict.vi";

            List<Word> dict = Improver.GetDict(path);

            Improver improver = new Improver(dict);
            improver.Start();

            Console.ReadKey();
        }
    }
}