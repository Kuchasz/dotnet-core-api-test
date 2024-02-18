using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? MinAge { get; set; }

    public int? MaxAge { get; set; }

    public string? Gender { get; set; }

    public bool IsSpecial { get; set; }

    public int ClassificationId { get; set; }

    // public virtual Classification Classification { get; set; } = null!;
}
