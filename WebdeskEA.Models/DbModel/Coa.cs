using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel;

public partial class Coa:BaseEntity
{
    //__________ Main Columns __________
    #region Main_Columns
    public int Id { get; set; }
    public string? AccountCode { get; set; }
    public string? Code { get; set; }
    [Required]
    public string? AccountName { get; set; }
    public int? ParentAccountId { get; set; }
    public int CoaCategoryId { get; set; }
    [Required]
    public int? CoatypeId { get; set; }
    [Required]
    public string? CoaTranType { get; set; }
    public string? Description { get; set; }
    public bool Transable { get; set; }
    public int? LevelNo { get; set; }
    public int TenantId { get; set; }
    public int TenantCompanyId { get; set; }
    #endregion

    //------ Joined Columns -----
    #region Joined_Columns

    [NotMapped]
    public string? ParentAccountName { get; set; }

    [NotMapped]
    public string? COATypeName { get; set; }

    [NotMapped]
    public string? TenantName { get; set; }


    [NotMapped]
    public string? CompanyName { get; set; }
    #endregion


}
