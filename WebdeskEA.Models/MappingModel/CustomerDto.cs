using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Models.MappingModel
{
    public class CustomerDto : BaseEntity
    {
        public CustomerDto()
        {
            TenantDtos = new List<TenantDto>();
            CompanyDtos = new List<CompanyDto>();
            COADtos = new List<COADto>();
        }
        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        [ValidateNever]
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public int CreditLimit { get; set; }
        public int TenantId { get; set; }
        public int CompanyId { get; set; }
        [Required]
        public int COAId { get; set; }
        public int CountryId { get; set; }
        public int StateProvinceId { get; set; }
        public int CityId { get; set; }
        public bool IsCustomerLogin { get; set; }
        [ValidateNever]
        public string CustomerLoginLink { get; set; }
        [ValidateNever]
        public string CustomerUserName { get; set; }
        [ValidateNever]
        public string CustomerPassword { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string TenantName { get; set; }
        [NotMapped]
        [ValidateNever]
        public string CountryName { get; set; }
        [NotMapped]
        [ValidateNever]
        public string StateName { get; set; }
        [NotMapped]
        [ValidateNever]
        public string CityName { get; set; }
        [NotMapped]
        [ValidateNever]
        public string CompanyName { get; set; }
        [NotMapped]
        [ValidateNever]
        public string AccountName { get; set; }
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        [ValidateNever]
        [NotMapped]
        public IEnumerable<TenantDto> TenantDtos { get; set; }
        [ValidateNever]
        [NotMapped]
        public IEnumerable<CompanyDto> CompanyDtos { get; set; }
        [ValidateNever]
        [NotMapped]
        public IEnumerable<COADto> COADtos { get; set; }

        [ValidateNever]
        [NotMapped]
        public string CustomerCommingFrom { get; set; }
        #endregion
    }
}
