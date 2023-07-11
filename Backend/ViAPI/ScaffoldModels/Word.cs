using System;
using System.Collections.Generic;

namespace ViAPI.ScaffoldModels;

public partial class Word
{
    public Guid WordGuidPk { get; set; }

    public string SourceWord { get; set; } = null!;

    public string TargetWord { get; set; } = null!;

    public int Rating { get; set; }

    public Guid DictionaryGuidFk { get; set; }

    public virtual Dictionary DictionaryGuidFkNavigation { get; set; } = null!;
}
