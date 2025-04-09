using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel;

public partial class VoucherDtl
{
    //__________ Main Columns __________
    #region Main_Columns
    public int Id { get; set; }
    public int VoucherId { get; set; }
    public int COAId { get; set; }
    public decimal DbAmount { get; set; }
    public decimal CrAmount { get; set; }
    public string AccountNo { get; set; }
    public string BankName { get; set; }
    public string ChequeNo { get; set; }
    public string PaymentType { get; set; }
    public string PaidInvoiceNo { get; set; }
    public string PaidInvoiceType { get; set; }
    public string Remarks { get; set; }
    public int PaidInvoiceId { get; set; }
    public int TenantId { get; set; }
    public int TenantCompanyId { get; set; }
    #endregion


    //------ Joined Columns -----
    #region Joined_Columns

    [ValidateNever]
    [NotMapped]
    public string? AccountName { get; set; }

    [NotMapped]
    public string? COATypeName { get; set; }

    [NotMapped]
    public string? TenantName { get; set; }


    [NotMapped]
    public string? CompanyName { get; set; }
    #endregion


}
