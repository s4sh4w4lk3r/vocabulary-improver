using System;
using System.Collections.Generic;

namespace ViAPI.ScaffoldModels;

public partial class User
{
    public Guid UserGuidPk { get; set; }

    public bool IsTelegram { get; set; }

    public virtual ICollection<Dictionary> Dictionaries { get; set; } = new List<Dictionary>();

    public virtual UserdataTg? UserdataTg { get; set; }
}
