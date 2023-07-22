using Newtonsoft.Json;
using System.Collections;

namespace ViTelegramBot;

public class ViSessionList : ICollection<ViSession>
{
    private List<ViSession> ViSessions { get; set; } = new List<ViSession>();
    private string Path { get; set; }

    public ViSessionList(string sessionsPath)
    {
        if (File.Exists(sessionsPath) is false) { File.Create(sessionsPath).Close(); }
        Path = sessionsPath;
        ReadJson();
    }


    private void SaveJson()
    {
        string jsonString = JsonConvert.SerializeObject(ViSessions, Formatting.Indented);
        File.WriteAllText(Path, jsonString);
    }

    private void ReadJson()
    {
        string jsonString = File.ReadAllText(Path);
        List<ViSession>? viSessionsNew = JsonConvert.DeserializeObject<List<ViSession>>(jsonString);
        if (viSessionsNew is not null) { ViSessions = viSessionsNew; }
    }

    public int Count => ViSessions.Count;

    public bool IsReadOnly => false;

    public void Add(ViSession item) 
    {
        ViSessions.Add(item);
        SaveJson();
    }

    public void Clear() 
    {
        ViSessions.Clear();
        SaveJson();
    }

    public bool Contains(ViSession item) => ViSessions.Contains(item);

    public void CopyTo(ViSession[] array, int arrayIndex) => ViSessions.CopyTo(array, arrayIndex);

    public IEnumerator<ViSession> GetEnumerator() => ViSessions.GetEnumerator();

    public bool Remove(ViSession item) 
    { 
        bool removed = ViSessions.Remove(item);
        SaveJson();
        return removed;
    }

    IEnumerator IEnumerable.GetEnumerator() => ViSessions.GetEnumerator();
}
