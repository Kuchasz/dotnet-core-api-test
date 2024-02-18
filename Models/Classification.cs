using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Classification
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int RaceId { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual Race Race { get; set; } = null!;
}
