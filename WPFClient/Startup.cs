using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using WPFClient;

namespace WPFClient
{
    internal class Startup
    {
        public static void Start()
        {
            string path = System.IO.Path.Combine(ViTools.ViDictsDirectory, "test.json");
            IWordProcessing lwp = new LocalWordProcessing(path);
            lwp.WordProcessingLogging += DebugLogger;
            var wp = new WordProcessor();
            wp.WordProcessings.Add(lwp);
            wp.Save();
        }

        private static void DebugLogger(string obj)
        {
            Debug.WriteLine(obj);
        }
    }
}
