using System;
using System.Collections.Generic;

namespace ViAPI.ScaffoldModels;

public partial class UserdataReg
{
    public Guid UserGuidFk { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Hash { get; set; } = null!;

    public virtual User UserGuidFkNavigation { get; set; } = null!;
}
