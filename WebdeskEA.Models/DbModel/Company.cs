using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel;

public partial class Company : BaseEntity
{
    public int Id { get; set; }
    public string CompanyCode { get; set; }
    public int TenantId { get; set; }
    [Required]
    public string? Name { get; set; }
    public int? ParentCompanyId { get; set; }
    [Required]
    [EmailAddress]
    public string? Address { get; set; }
    [Required]
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? PostalCode { get; set; }
    public string? Logo { get; set; }
    public string? Trn { get; set; }
    public int CountryId { get; set; }
    public int StateId { get; set; }
    public int CityId { get; set; }
    public bool IsMainCompany { get; set; }

}
