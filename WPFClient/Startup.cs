using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WPFClient;

namespace WPFClient
{
    internal class Startup
    {
        public static void Start()
        {
            string path = @"C:\Users\roman\Desktop\vocabulary-improver\WPFClient\test.json";
            IWordProcessing wp = new LocalWordProcessing(path);
            wp.WordProcessingLogging += DebugLogger;
        }

        private static void DebugLogger(string obj)
        {
            Debug.WriteLine(obj);
        }
    }
}
