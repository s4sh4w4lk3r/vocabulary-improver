using System.Collections;
using ViAPI.Other;

namespace ViAPI.Entities;

public class ViDictionary : IReadOnlyList<Word>
{
    public Guid Guid { get; set; }
    public string Name { get; set; } = string.Empty;

    public Guid UserGuid { get; set; }
    public virtual User? User { get; set; }
    public virtual IList<Word> Words { get; set; } = new List<Word>();

    protected ViDictionary() { }
    public ViDictionary(Guid guid, string name, Guid userGuid)
    {
        InputChecker.CheckStringException(name);
        InputChecker.CheckGuidException(guid);
        Guid = guid;
        Name = name;
        UserGuid = userGuid;
    }

    public override string ToString() => $"[{GetType().Name}] Guid {Guid}, Name: {Name}, WordsCount: {Count}, UserGuid: {UserGuid}";

    #region Реализация методов интерфейса IList<T>.
    public int Count => Words.Count;
    public Word this[int index] { get => Words[index]; set => Words[index] = value; }
    public IEnumerator<Word> GetEnumerator() => Words.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Words.GetEnumerator();
    #endregion
}
