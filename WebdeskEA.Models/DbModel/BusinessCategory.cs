using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebdeskEA.Models.DbModel;

public partial class BusinessCategory
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? CompanyId { get; set; }

    [NotMapped]
    public string? CompanyName { get; set; }
}
