namespace console
{
    class Program
    {
        private const string path = "dict.vi";

        static void Main(string[] args)
        {
            FileProcessing.CreateFile("text.txt");
            var file = new FileProcessing("text.txt");
        //    var dict = file.ShuffledList;
        //    file.ReduceRatingFile("figure out");
        //    file.ReduceRatingFile("figure out");
        //    file.ImproveRatingFile("figure out");
           Console.ReadKey();
        }
    }
}