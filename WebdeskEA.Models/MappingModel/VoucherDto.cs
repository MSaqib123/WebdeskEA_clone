
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Models.BaseEntites;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Models.DbModel;

public class VoucherDto : BaseEntity
{
    public VoucherDto()
    {
        SIDtos = new List<SIDto>();
        PIDtos = new List<PIDto>();
        CustomerDtos = new List<CustomerDto>();
        SupplierDtos = new List<SupplierDto>();
        COADtos = new List<COADto>();
        COATypeDtos = new List<COATypeDto>();
        BankDtos = new List<BankDto>();
        VoucherDtlDtos = new List<VoucherDtlDto>();
        PaymentTypeList = new SelectList(new List<KeyValuePair<string, string>>());
    }

    //__________ Main Columns __________
    #region Main_Columns
    public int Id { get; set; }
    public int VoucherTypeId { get; set; }
    public string? VoucherType { get; set; }
    public string VoucherCode { get; set; }
    public DateTime VoucherDate { get; set; } = DateTime.Now;

    [ValidateNever]
    public string VoucherNarration { get; set; }
    [ValidateNever]
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

    //__________ List and Single Object__________
    #region Note_Mapped
    [ValidateNever]
    [NotMapped]
    public IEnumerable<SIDto> SIDtos { get; set; }
    [ValidateNever]
    [NotMapped]
    public IEnumerable<PIDto> PIDtos { get; set; }

    [ValidateNever]
    [NotMapped]
    public IEnumerable<BankDto> BankDtos { get; set; }

    [ValidateNever]
    [NotMapped]
    public IEnumerable<CustomerDto> CustomerDtos { get; set; }

    [ValidateNever]
    [NotMapped]
    public IEnumerable<SupplierDto> SupplierDtos { get; set; }

    [ValidateNever]
    [NotMapped]
    public IEnumerable<COADto> COADtos { get; set; }

    [ValidateNever]
    [NotMapped]
    public IEnumerable<COATypeDto> COATypeDtos { get; set; }
    
    [ValidateNever]
    [NotMapped]
    public IEnumerable<VoucherDtlDto> VoucherDtlDtos { get; set; }

    [NotMapped]
    [ValidateNever]
    public IEnumerable<SelectListItem> PaymentTypeList { get; set; } = new List<SelectListItem>();
    #endregion

    //__________ DtlDto Fields ________
    #region Dtl_Fields
    [NotMapped]
    [ValidateNever]
    public string? PaidInvoiceType { get; set; }
    #endregion
}
