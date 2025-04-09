using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel
{
    public class Supplier : BaseEntity
    {
        //__________ Main Columns __________
        #region Main_Columns
        [Key]
        public int Id { get; set; }
        [ValidateNever]
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public decimal CreditLimit { get; set; }
        public int COAId { get; set; }
        public int TenantId { get; set; }
        public int CompanyId { get; set; }
        public int CountryId { get; set; }
        public int StateProvinceId { get; set; }
        public int CityId { get; set; }
        public bool IsSupplierLogin { get; set; }
        public string SupplierLoginLink { get; set; }
        public string SupplierUserName { get; set; }
        public string SupplierPassword { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        [NotMapped]
        public string TenantName { get; set; }
        [NotMapped]
        public string CountryName { get; set; }
        [NotMapped]
        public string StateName { get; set; }
        [NotMapped]
        public string CityName { get; set; }
        [NotMapped]
        public string CompanyName { get; set; }
        [NotMapped]
        public string AccountName { get; set; }
        #endregion
        //__________ List and Single Object__________
        #region Note_Mapped
        #endregion
    }


}
