using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel;
public partial class VoucherType
{
    //__________ Main Columns __________
    #region Main_Columns
    public int Id { get; set; }
    public string VoucherTypeName { get; set; }
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
