using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Session
{
    public string Id { get; set; } = null!;

    public string SessionToken { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public long Expires { get; set; }

    public bool Valid { get; set; }

    public virtual User User { get; set; } = null!;
}
