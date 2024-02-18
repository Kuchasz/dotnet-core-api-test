using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class BibNumber
{
    public int Id { get; set; }

    public string Number { get; set; } = null!;

    public int RaceId { get; set; }

    public virtual Race Race { get; set; } = null!;
}
