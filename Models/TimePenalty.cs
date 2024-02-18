using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class TimePenalty
{
    public int Id { get; set; }

    public string BibNumber { get; set; } = null!;

    public int Time { get; set; }

    public string Reason { get; set; } = null!;

    public int RaceId { get; set; }

    public virtual Race Race { get; set; } = null!;
}
