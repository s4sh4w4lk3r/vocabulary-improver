namespace console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // string path = "D:\\en-ru.vi";
            // if (args.Length == 1) path = args[0];
            // Learn.Start(Input.GetDict(path));

            string connString = "server=localhost;port=3306;username=admin;password=admin;database=vocabulary-improver";
            string tableName = "en-ru";
            Input.ConnString = connString;
            var dict = Input.GetDict(Input.ConnString, tableName);
            Learn.Start(dict);
            
            Console.ReadKey();
        }
    }
}