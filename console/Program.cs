namespace console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connString = "server=localhost;port=3306;username=admin;password=admin;database=vocabulary-improver";
            string tableName = "en-ru";
            var dict = DBProcessing.GetDict(connString, tableName);
        }
    }
}