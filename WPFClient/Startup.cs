using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using ConsoleClient;

namespace ConsoleClient
{
    internal class Startup
    {
        public static void Start()
        {
            ViTools.CheckFiles();
            string path = System.IO.Path.Combine(ViTools.ViDictsDirectory, "test.json");
            IWordProcessing lwp = new LocalWordProcessing(path);
/*            lwp.AddWordsFromFile(@"C:\Users\sanchous\AppData\Roaming\VocabularyImprover\ViDicts\tet.txt");*/
            lwp.WordProcessingLogging += DebugLogger;
            foreach (var item in lwp.Words)
            {
                Debug.WriteLine(item.ToString());
            }
            
        }

        private static void DebugLogger(string obj)
        {
            Debug.WriteLine(obj);
        }

        private static void CreateLocalDict(string name)
        {
            string path = System.IO.Path.Combine(ViTools.ViDictsDirectory, name, ".json");
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }
    }
}
