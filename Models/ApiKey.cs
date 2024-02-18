using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class ApiKey
{
    public int Id { get; set; }

    public string Key { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int RaceId { get; set; }

    public virtual Race Race { get; set; } = null!;
}
