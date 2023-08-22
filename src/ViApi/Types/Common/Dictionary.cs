using System.Collections;
using ViApi.Types.Common.Users;

namespace ViApi.Types.Common;

public class Dictionary : IEnumerable<Word>
{
    private Guid _guid;
    private string _name = null!;
    private Guid _userGuid;
    private List<Word> _words = null!;

    public Guid Guid
    {
        get => _guid;
        set => _guid = value.Throw("В конструктор Dictionary получен пустой dictGuid.").IfDefault().Value;
    }
    public string Name
    {
        get => _name;
        set => _name = value.Throw("В конструктор Dictionary получен пустое имя.").IfNullOrWhiteSpace(_ => _).Value;
    }
    public Guid UserGuid
    {
        get => _userGuid;
        set => _userGuid = value.Throw("В конструктор Dictionary получен пустой userGuid.").IfDefault().Value;
    }
    public UserBase? User { get; set; }
    public List<Word> Words
    {
        get => _words;
        set => _words = value.ThrowIfNull("В конструктор Dictionary получена null коллекция.").Value;
    }

    private Dictionary() { }
    public Dictionary(Guid dictGuid, string name, Guid userGuid, IEnumerable<Word> words)
    {
        Guid = dictGuid;
        Name = name;
        UserGuid = userGuid;
        Words = words.ToList();
    }

    public override string ToString() => $"[Guid {Guid}, Name: {Name}, WordsCount: {Words.Count}, UserGuid: {UserGuid}";

    public IEnumerator<Word> GetEnumerator() => Words.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Words.GetEnumerator();
}
