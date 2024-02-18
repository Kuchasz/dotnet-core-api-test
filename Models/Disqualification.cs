using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Disqualification
{
    public int Id { get; set; }

    public string BibNumber { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public int RaceId { get; set; }

    public virtual Race Race { get; set; } = null!;
}
