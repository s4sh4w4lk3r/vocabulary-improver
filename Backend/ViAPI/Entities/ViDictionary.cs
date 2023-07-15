using System.Collections;
using ViAPI.StaticMethods;

namespace ViAPI.Entities;

public class ViDictionary : IList<Word>
{
    public Guid Guid { get; set; }
    public string Name { get; set; } = string.Empty;

    public Guid UserGuid { get; set; }
    public virtual User? User { get; set; }
    public virtual IList<Word> Words { get; set; } = new List<Word>();

    protected ViDictionary() { }
    public ViDictionary(Guid guid, string name, User user)
    {
        InputChecker.CheckStringException(name);
        InputChecker.CheckGuidException(guid);
        Guid = guid;
        Name = name;
        User = user;
    }
    public ViDictionary(Guid guid, string name, User user, IEnumerable<Word> words) : this(guid, name, user)
    {
        Words = words.ToList();
    }

    public override string ToString() => $"[{GetType()}] Guid {Guid}, Name: {Name}, WordsCount: {Count}, UserGuid: {UserGuid}";

    #region Реализация методов интерфейса IList<T>.
    public int Count => Words.Count;

    public bool IsReadOnly => Words.IsReadOnly;

    public Word this[int index] { get => Words[index]; set => Words[index] = value; }

    public int IndexOf(Word item) => Words.IndexOf(item);

    public void Insert(int index, Word item) => Words.Insert(index, item);

    public void RemoveAt(int index) => Words.RemoveAt(index);

    public void Add(Word item) => Words.Add(item);

    public void Clear() => Words.Clear();

    public bool Contains(Word item) => Words.Contains(item);

    public void CopyTo(Word[] array, int arrayIndex) => Words.CopyTo(array, arrayIndex);

    public bool Remove(Word item) => Words.Remove(item);

    public IEnumerator<Word> GetEnumerator() => Words.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Words.GetEnumerator();
    #endregion
}
