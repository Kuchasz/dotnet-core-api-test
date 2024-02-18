using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Player
{
    public int Id { get; set; }

    public string BibNumber { get; set; } = null!;

    public int? StartTime { get; set; }

    public int RaceId { get; set; }

    public int ClassificationId { get; set; }

    public string PromotedByUserId { get; set; } = null!;

    public int PlayerRegistrationId { get; set; }

    public int PlayerProfileId { get; set; }

    public virtual ICollection<Absence> Absences { get; set; } = new List<Absence>();

    public virtual Classification Classification { get; set; } = null!;

    public virtual ICollection<ManualSplitTime> ManualSplitTimes { get; set; } = new List<ManualSplitTime>();

    public virtual PlayerProfile PlayerProfile { get; set; } = null!;

    public virtual PlayerRegistration PlayerRegistration { get; set; } = null!;

    public virtual User PromotedByUser { get; set; } = null!;

    public virtual Race Race { get; set; } = null!;

    public virtual ICollection<SplitTime> SplitTimes { get; set; } = new List<SplitTime>();
}
