using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Model.sutff
{
    internal class DictionaryProperties
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? LocalPath { get; set; }
        public object? OnlineToken { get; set; }
        public DictionaryProperties(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public DictionaryProperties(string name, string description, string? localPath) : this(name, description)
        {
            LocalPath = localPath;
        }
        public DictionaryProperties(string name, string description, object? onlineToken) : this(name, description)
        {
            OnlineToken = onlineToken;
        }
    }
}
