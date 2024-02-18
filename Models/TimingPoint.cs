using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class TimingPoint
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string ShortName { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Description { get; set; }

    public int Laps { get; set; }

    public int RaceId { get; set; }

    public virtual ICollection<Absence> Absences { get; set; } = new List<Absence>();

    public virtual ICollection<ManualSplitTime> ManualSplitTimes { get; set; } = new List<ManualSplitTime>();

    public virtual Race Race { get; set; } = null!;

    public virtual ICollection<SplitTime> SplitTimes { get; set; } = new List<SplitTime>();

    public virtual ICollection<TimingPointAccessUrl> TimingPointAccessUrls { get; set; } = new List<TimingPointAccessUrl>();
}
