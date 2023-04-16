namespace console
{
    class Program
    {
        private const string path = "test.json";

        static void Main(string[] args)
        {
            FileProcessing.CreateFile(path);

            var file = new FileProcessing(path);

            var dict = file.Dict;
            file.AddFromTxt("dict.vi");

            var suffledDict = file.ShuffledList;

            file.Add("test", "test");

            file.Remove("test");

            file.ImproveRatingFile("figure out");

            file.ReduceRatingFile("figure out");

            Console.ReadKey();
        }
    }
}