using System;
using System.Collections.Generic;

namespace WebdeskEA.Models.DbModel;

public partial class Sysdiagram
{
    public string Name { get; set; } = null!;

    public int PrincipalId { get; set; }

    public int DiagramId { get; set; }

    public int? Version { get; set; }

    public byte[]? Definition { get; set; }
}
