using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.MappingModel
{
    public class PackageDto:BaseEntity
    {
        public PackageDto()
        {
            PackageTypeDtoList = new List<PackageTypeDto>();
        }
        public int Id { get; set; }
        public int? PackageTypeId { get; set; }
        public string PackageName { get; set; }
        public int? TotalCompany { get; set; }
        public int? TotalUser { get; set; }

        
        //__________ List and Single Object__________
        #region Note Mapped
        [ValidateNever]
        public IEnumerable<PackageTypeDto> PackageTypeDtoList { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined Columns
        [NotMapped]
        [ValidateNever]
        public string PackageTypeName { get; set; }

        #endregion
    }
}
