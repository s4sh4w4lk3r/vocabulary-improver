namespace console
{
    class Program
    {
        private const string path = "dict.json";

        static void Main(string[] args)
        {
            DBProcessing database = new DBProcessing("localhost", "3306", "admin", "admin", "vocabulary-improver", "en-ru");
            

            FileProcessing file = new FileProcessing(path);
            
            
        }
    }
}