using System;
using System.Collections.Generic;

namespace ServerSide.Database;

public partial class User
{
    public Guid UserGuid { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public DateOnly RedDate { get; set; }

    public virtual ICollection<Word> Words { get; set; } = new List<Word>();
}
