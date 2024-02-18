using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Race
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string SportKind { get; set; } = null!;

    public long Date { get; set; }

    public string? TermsUrl { get; set; }

    public string? EmailTemplate { get; set; }

    public int? PlayersLimit { get; set; }

    public bool RegistrationEnabled { get; set; }

    public string? WebsiteUrl { get; set; }

    public DateTime? RegistrationCutoff { get; set; }

    public virtual ICollection<Absence> Absences { get; set; } = new List<Absence>();

    public virtual ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();

    public virtual ICollection<BibNumber> BibNumbers { get; set; } = new List<BibNumber>();

    public virtual ICollection<Classification> Classifications { get; set; } = new List<Classification>();

    public virtual ICollection<Disqualification> Disqualifications { get; set; } = new List<Disqualification>();

    public virtual ICollection<ManualSplitTime> ManualSplitTimes { get; set; } = new List<ManualSplitTime>();

    public virtual ICollection<PlayerProfile> PlayerProfiles { get; set; } = new List<PlayerProfile>();

    public virtual ICollection<PlayerRegistration> PlayerRegistrations { get; set; } = new List<PlayerRegistration>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual ICollection<SplitTime> SplitTimes { get; set; } = new List<SplitTime>();

    public virtual Stopwatch? Stopwatch { get; set; }

    public virtual ICollection<TimePenalty> TimePenalties { get; set; } = new List<TimePenalty>();

    public virtual ICollection<TimingPointAccessUrl> TimingPointAccessUrls { get; set; } = new List<TimingPointAccessUrl>();

    public virtual TimingPointOrder? TimingPointOrder { get; set; }

    public virtual ICollection<TimingPoint> TimingPoints { get; set; } = new List<TimingPoint>();
}
