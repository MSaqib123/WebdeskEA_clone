using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel;

public partial class Voucher : BaseEntity
{
    //__________ Main Columns __________
    #region Main_Columns
    public int Id { get; set; }
    public int VoucherTypeId { get; set; }
    public string? VoucherType { get; set; }
    public string VoucherCode { get; set; }
    public string VoucherNarration { get; set; }
    public string VoucherInvoiceNo { get; set; }
    public int TenantId { get; set; }
    public int TenantCompanyId { get; set; }
    public int FinancialYearId { get; set; }
    #endregion


    //------ Joined Columns -----
    #region Joined_Columns

    [NotMapped]
    [ValidateNever]
    public string? VoucherTypeName { get; set; }

    #endregion


}
