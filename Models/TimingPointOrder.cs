using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class TimingPointOrder
{
    public string Order { get; set; } = null!;

    public int RaceId { get; set; }

    public virtual Race Race { get; set; } = null!;
}
