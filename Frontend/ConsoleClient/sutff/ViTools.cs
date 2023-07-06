using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ConsoleClient
{
    public static class ViTools
    {
        private static string userRoamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string AppDirectory { get; } = Path.Combine(userRoamingDirectory, "VocabularyImprover");
        public static string ViDictsDirectory { get; } = Path.Combine(AppDirectory, "ViDicts");
        public static string ConfigFilePath { get; } = Path.Combine(AppDirectory, "config.json");
        public static string ViDictsListFilePath { get; } = Path.Combine(AppDirectory, "viDictsList.json");
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

        public static void CheckFiles()
        {
            if (Directory.Exists(AppDirectory) == false)
            {
                Directory.CreateDirectory(AppDirectory);
            }

            if (Directory.Exists(ViDictsDirectory) == false)
            {
                Directory.CreateDirectory(ViDictsDirectory);
            }

            if (File.Exists(ConfigFilePath) == false)
            {
                File.Create(ConfigFilePath).Close();
            }
            
            if (File.Exists(ViDictsListFilePath) == false)
            {
                File.Create(ViDictsListFilePath);
            }
        }
    }
}
