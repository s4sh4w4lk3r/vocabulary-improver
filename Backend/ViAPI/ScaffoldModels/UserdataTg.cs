using System;
using System.Collections.Generic;

namespace ViAPI.ScaffoldModels;

public partial class UserdataTg
{
    public ulong TelegramId { get; set; }

    public Guid UserGuidFk { get; set; }

    public virtual User UserGuidFkNavigation { get; set; } = null!;
}
