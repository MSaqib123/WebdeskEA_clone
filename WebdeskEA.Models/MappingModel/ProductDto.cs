using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Models.BaseEntites;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Models.MappingModel;

public class ProductDto : BaseEntity
{
    public ProductDto()
    {
        BrandDtos = new List<BrandDto>();
        CategoryDtos = new List<CategoryDto>();
        TaxMasterDtos = new List<TaxMasterDto>();
        COADtos = new List<COADto>();

        COAIncomeDtos = new List<COADto>();
        COAExpenseDtos = new List<COADto>(); 
        SelectedIncomeCOAs = new List<int>();
        SelectedExpenseCOAs = new List<int>();
    }

    //__________ Main Columns __________
    #region Main_Columns
    public int Id { get; set; }
    [ValidateNever]
    public string ProductCode { get; set; }
    public int BrandId { get; set; }
    public string ProductName { get; set; }
    public string ProductSKU { get; set; }
    public string ProductDescription { get; set; }
    public decimal ProductPrice { get; set; }
    public bool IsProductSale { get; set; }
    public bool IsProductBuy { get; set; }
    [ValidateNever]
    public string Image { get; set; }
    [ValidateNever]
    public int CategoryId { get; set; }
    public int? TaxId { get; set; }
    [ValidateNever]
    public int TenantId { get; set; }
    [ValidateNever]
    public int CompanyId { get; set; }
    #endregion

    //__________ Joined Columns _________
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

    //__________ Conditional Column _________
    #region Conditional Column
    [ValidateNever]
    [NotMapped]
    public string typeOfPartialView { get; set; }
    #endregion

    //__________ List and Single Object __________
    #region Note_Mapped
    [ValidateNever]
    [NotMapped]
    public IEnumerable<BrandDto> BrandDtos { get; set; }
    [ValidateNever]
    [NotMapped]
    public IEnumerable<TaxMasterDto> TaxMasterDtos { get; set; }

    [ValidateNever]
    [NotMapped]
    public IEnumerable<COADto> COADtos { get; set; }
    [ValidateNever]
    [NotMapped]
    public IEnumerable<COADto> COAIncomeDtos { get; set; }
    [ValidateNever]            
    [NotMapped]
    public IEnumerable<COADto> COAExpenseDtos { get; set; }
    [ValidateNever]
    [NotMapped]
    public IEnumerable<CategoryDto> CategoryDtos { get; set; }
    [ValidateNever]
    [NotMapped]
    public IEnumerable<int> SelectedIncomeCOAs { get; set; }
    [ValidateNever]
    [NotMapped]
    public IEnumerable<int> SelectedExpenseCOAs { get; set; }

    [NotMapped]
    [ValidateNever]
    public IFormFile? ImageForSave { get; set; } // File upload for profile image

    #endregion
}
