using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Absence
{
    public int Id { get; set; }

    public string BibNumber { get; set; } = null!;

    public int RaceId { get; set; }

    public int TimingPointId { get; set; }

    public virtual Player Player { get; set; } = null!;

    public virtual Race Race { get; set; } = null!;

    public virtual TimingPoint TimingPoint { get; set; } = null!;
}
