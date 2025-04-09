using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Models.MappingModel
{

    public class BankDto:BaseEntity
    {
        public BankDto()
        {
            TenantDtos = new List<TenantDto>();
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public string BankName { get; set; }
        [ValidateNever]
        public string SwiftCode { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string AccountNo { get; set; }
        public int TenantId { get; set; }
        public int TenantCompanyId { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string TenantName { get; set; }
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        [ValidateNever]
        [NotMapped]
        public IEnumerable<TenantDto> TenantDtos { get; set; }
        #endregion
    }

}
