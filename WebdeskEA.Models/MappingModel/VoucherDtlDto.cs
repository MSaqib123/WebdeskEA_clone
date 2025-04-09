using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel;

public partial class VoucherDtlDto
{
    //__________ Main Columns __________
    #region Main_Columns
    public int Id { get; set; }
    public int VoucherId { get; set; }
    public int COAId { get; set; }
    public decimal DbAmount { get; set; }
    public decimal CrAmount { get; set; }
    [ValidateNever]
    public string AccountNo { get; set; }
    [ValidateNever]
    public int BankId { get; set; }
    [ValidateNever]
    public string ChequeNo { get; set; }
    [ValidateNever]
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
    [ValidateNever]
    [NotMapped]
    public string? BankName { get; set; }

    [ValidateNever]
    [NotMapped]
    public string? COATypeName { get; set; }
    [ValidateNever]
    [NotMapped]
    public string? TenantName { get; set; }

    [ValidateNever]
    [NotMapped]
    public string? CompanyName { get; set; }
    #endregion


}
