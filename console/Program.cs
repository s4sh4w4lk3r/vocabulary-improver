namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello, select the operating mode\n1 - MySQL Database, 2 - Locally, Any Key - Exit\n");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    Improver.StartDatabase();
                    break;
                case ConsoleKey.D2:
                    Improver.StartLocally();
                    break;
            }
        }
    }
}