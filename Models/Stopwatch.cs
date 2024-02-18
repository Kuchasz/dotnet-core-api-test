using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Stopwatch
{
    public int RaceId { get; set; }

    public string State { get; set; } = null!;

    public virtual Race Race { get; set; } = null!;
}
