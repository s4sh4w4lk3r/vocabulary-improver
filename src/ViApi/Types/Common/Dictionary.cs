﻿using System.Collections;
using Throw;

namespace ViApi.Types.Common;

public class Dictionary : IEnumerable<Word>
{
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public Guid UserGuid { get; set; }
    public IEnumerable<Word> Words { get; set; }

    public Dictionary(Guid guid, string name, Guid userGuid, IEnumerable<Word> words)
    {
        guid.Throw("В конструктор Dictionary получен пустой Guid.").IfDefault();
        userGuid.Throw("В конструктор Dictionary получен пустой userGuid.").IfDefault();
        name.Throw("В конструктор Dictionary получен пустое имя.").IfNullOrWhiteSpace(_ => _);
        words.ThrowIfNull("В конструктор Dictionary получена null коллекция.");

        Guid = guid;
        Name = name;
        UserGuid = userGuid;
        Words = words;
    }

    public override string ToString() => $"[Guid {Guid}, Name: {Name}, WordsCount: {Words.Count()}, UserGuid: {UserGuid}";

    public IEnumerator<Word> GetEnumerator() => Words.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Words.GetEnumerator();
}
