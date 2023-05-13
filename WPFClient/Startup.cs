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
            LocalWordProcessing lp = new LocalWordProcessing(path);
            lp.LocalProcessingLogging += Lp_LocalProcessingLogging;
            lp.Load();
            foreach (var item in lp.Words)
            {
                Debug.WriteLine(item.Word1 + item.Word2);
            }
        }

        private static void Lp_LocalProcessingLogging(string obj)
        {
            Debug.WriteLine(obj);
        }
    }
}
