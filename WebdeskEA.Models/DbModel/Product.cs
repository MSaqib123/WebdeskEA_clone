using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel;

public class Product : BaseEntity
{
    public Product()
    {
    }

    //__________ Main Columns __________
    #region Main_Columns
    public int Id { get; set; }
    public string ProductCode { get; set; }
    public int BrandId { get; set; }
    public string ProductName { get; set; }
    public string ProductSKU { get; set; }
    public string ProductDescription { get; set; }
    public string Image { get; set; }
    public int CategoryId { get; set; }
    public decimal ProductPrice { get; set; }
    public bool IsProductSale { get; set; }
    public bool IsProductBuy { get; set; }
    public int? TaxId { get; set; }
    public int TenantId { get; set; }
    public int CompanyId { get; set; }
    #endregion

    //------ Joined Columns -----
    #region Joined_Columns
    [ValidateNever]
    [NotMapped]
    public int Stock { get; set; }
    [ValidateNever]
    [NotMapped]
    public string BrandName { get; set; }
    [ValidateNever]
    [NotMapped]
    public string TaxName { get; set; }
    [ValidateNever]
    [NotMapped]
    public string CategoryName { get; set; }

    #endregion

    //__________ List and Single Object__________
    #region Note_Mapped
    #endregion
}
