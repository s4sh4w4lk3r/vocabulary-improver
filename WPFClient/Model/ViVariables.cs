using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DraftClient.Model
{
    public sealed class ViVariables
    {
        private static string userRoamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string AppDirectory { get; } = Path.Combine(userRoamingDirectory, "VocabularyImprover");
        public static string ViDictsDirectory { get; } = Path.Combine(AppDirectory, "ViDicts");
        public static string ConfigFilePath { get; } = Path.Combine(AppDirectory, "config.json");
    }
}
