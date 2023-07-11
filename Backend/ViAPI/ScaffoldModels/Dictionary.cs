using System;
using System.Collections.Generic;

namespace ViAPI.ScaffoldModels;

public partial class Dictionary
{
    public Guid DictionaryGuidPk { get; set; }

    public string Name { get; set; } = null!;

    public Guid UserGuidFk { get; set; }

    public virtual User UserGuidFkNavigation { get; set; } = null!;

    public virtual ICollection<Word> Words { get; set; } = new List<Word>();
}
