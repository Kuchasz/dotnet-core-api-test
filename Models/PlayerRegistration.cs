using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class PlayerRegistration
{
    public int Id { get; set; }

    public long RegistrationDate { get; set; }

    public bool HasPaid { get; set; }

    public long? PaymentDate { get; set; }

    public int RaceId { get; set; }

    public int PlayerProfileId { get; set; }

    public virtual PlayerProfile PlayerProfile { get; set; } = null!;

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual Race Race { get; set; } = null!;
}
