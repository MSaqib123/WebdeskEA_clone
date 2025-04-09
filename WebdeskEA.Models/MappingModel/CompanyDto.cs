using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.MappingModel;

public class CompanyDto:BaseEntity
{
    public CompanyDto()
    {
        CountryDtoList= new List<CountryDto>();;
        StateProvinceDtoList= new List<StateProvinceDto>();;
        CityDtoList = new List<CityDto>(); ;
    }

    #region Main_Columns
    public int Id { get; set; }
    [ValidateNever]
    public string CompanyCode { get; set; }
    public int TenantId { get; set; }
    [Required]
    public string? Name { get; set; }
    public int? ParentCompanyId { get; set; }
    [Required]
    public string? Address { get; set; }
    [ValidateNever]
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? PostalCode { get; set; }
    [ValidateNever]
    public string? Logo { get; set; }
    public string? Trn { get; set; }
    public int CountryId { get; set; }
    public int StateId { get; set; }
    public int CityId { get; set; }
    [ValidateNever]
    public bool IsMainCompany { get; set; }
    #endregion

    #region List_Record
    [ValidateNever]
    public IEnumerable<CountryDto> CountryDtoList { get; set; }
    [ValidateNever]
    public IEnumerable<StateProvinceDto> StateProvinceDtoList { get; set; }
    [ValidateNever]
    public IEnumerable<CityDto> CityDtoList { get; set; }
    #endregion

    #region Joined_Column
    [ValidateNever]
    [NotMapped]
    public string CountryName { get; set; }
    [ValidateNever]
    [NotMapped]
    public string CityName { get; set; }
    [ValidateNever]
    [NotMapped]
    public string StateName { get; set; }
    #endregion

    #region Binding Properties
    [NotMapped]
    [ValidateNever]
    public IFormFile? CompanyImageForSave { get; set; } // File upload for profile image
    #endregion
}
