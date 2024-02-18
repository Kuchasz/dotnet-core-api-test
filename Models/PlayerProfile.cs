using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class PlayerProfile
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public long BirthDate { get; set; }

    public string Gender { get; set; } = null!;

    public string? Team { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? IcePhoneNumber { get; set; }

    public int RaceId { get; set; }

    public virtual ICollection<PlayerRegistration> PlayerRegistrations { get; set; } = new List<PlayerRegistration>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual Race Race { get; set; } = null!;
}
