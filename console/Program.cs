namespace console
{
    enum StartMode {MySQLDatabase, LocalFile};

    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello, select the operating mode\n1 - MySQL Database, 2 - Locally\n");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    Improver.StartDatabase();
                    break;

                case ConsoleKey.D2:

                    break;
            }
        }
    }
}