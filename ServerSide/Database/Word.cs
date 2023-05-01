using System;
using System.Collections.Generic;

namespace ServerSide.Database;

public partial class Word
{
    public int WordId { get; set; }

    public string Word1 { get; set; } = null!;

    public string Word2 { get; set; } = null!;

    public sbyte Rating { get; set; }

    public Guid UserGuidFk { get; set; }

    public virtual User UserGuidFkNavigation { get; set; } = null!;
}
