using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class TimingPointAccessUrl
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public bool CanAccessOthers { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpireDate { get; set; }

    public int TimingPointId { get; set; }

    public int RaceId { get; set; }

    public virtual Race Race { get; set; } = null!;

    public virtual TimingPoint TimingPoint { get; set; } = null!;
}
