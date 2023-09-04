using System.Collections;
using ViApi.Types.Common.Users;

namespace ViApi.Types.Common;

public class Dictionary : IEnumerable<Word>
{
    public Guid Guid { get; init; }
    public string? Name { get; set; }
    public Guid UserGuid { get; init; }
    public UserBase? User { get; init; }
    public List<Word>? Words { get; init; }

    private Dictionary() { }
    public Dictionary(Guid dictGuid, string name, Guid userGuid)
    {
        Guid = dictGuid;
        Name = name;
        UserGuid = userGuid;
        Words = new List<Word>();
    }

    public override string ToString() => $"Guid {Guid}, Name: {Name}, WordsCount: {Words?.Count}, UserGuid: {UserGuid}";

    public IEnumerator<Word> GetEnumerator() => Words!.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Words!.GetEnumerator();
}
