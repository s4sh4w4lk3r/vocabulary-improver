using ConsoleClient.sutff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http;

namespace ConsoleClient.Online
{
    public class OnlineWordProcessing : IWordProcessing
    {
        private string Token { get; set; }

        public List<Word> Words { get; set; }
        

        public List<Word> ShuffledWords
        {
            get
            {
                if (Words.Count > 0)
                {
                    return Words.OrderBy(x => new Random().Next()).ToList();
                }
                else
                {
                    return new List<Word>();
                }
            }
        }

        public event Action<string>? WordProcessingLogging;

        public OnlineWordProcessing(string token)
        {
            Token = token;
            Words = new List<Word>();
            AsyncHelper.RunSync(GetWordsFromServer);
        }

        private async Task GetWordsFromServer()
        {
            HttpClient httpClient = new();
            Words = await httpClient.GetFromJsonAsync<List<Word>>($"http://localhost:5107/vocabulary-improver/api/getall?userToken={Token}") ?? Words;
            WordProcessingLogging?.Invoke("JSON GETTED.");
        }

        public void Add(string sourceWord, string targetWord)
        {
            throw new NotImplementedException();
        }

        public void Add(List<Word> words)
        {
            throw new NotImplementedException();
        }

        public void AddWordsFromFile(string path)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void DecreaseRating(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void Edit(Guid guid, string targetWord)
        {
            throw new NotImplementedException();
        }

        public void Edit(string sourceWord, string targetWord)
        {
            throw new NotImplementedException();
        }

        public void IncreaseRating(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void Remove(string sourceWord)
        {
            throw new NotImplementedException();
        }

        public void Remove(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void Remove(List<Guid> guids)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
