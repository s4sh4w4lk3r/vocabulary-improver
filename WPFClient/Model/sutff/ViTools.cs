using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WPFClient
{
    public static class ViTools
    {
        private static string userRoamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string AppDirectory { get; } = Path.Combine(userRoamingDirectory, "VocabularyImprover");
        public static string ViDictsDirectory { get; } = Path.Combine(AppDirectory, "ViDicts");
        public static string ConfigFilePath { get; } = Path.Combine(AppDirectory, "config.json");
        public static string WordProcessorsPath { get; } = Path.Combine(AppDirectory, "wordprocessor.json");
        public static void SaveToJSON(object obj, string path)
        {
            string WordsJSON = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(path, WordsJSON);
        }
        public static T? ReadFromJSON<T>(string path)
        {
            string json = File.ReadAllText(path);
            var JSONobj = JsonConvert.DeserializeObject<T>(json);
            return JSONobj;
        }
    }
}
