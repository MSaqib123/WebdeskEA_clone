using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.MappingModel
{

    public class PackagePermissionDto
    {
        public PackagePermissionDto()
        {
            ModuleList = new List<ModuleDto>();
            ModuleIds = new List<int>();
            PackageTypeDtoList = new List<PackageTypeDto>();
            packageDto = new PackageDto();
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public int? PackageId { get; set; }
        public int? ModuleId { get; set; }
        public bool? IsModuleActive { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        [ValidateNever]
        [NotMapped]
        public string PackageName { get; set; }
        #endregion

        //__________ List and Single Object__________
        #region List
        [ValidateNever]
        [NotMapped]
        public List<int> ModuleIds { get; set; }
        [ValidateNever]
        [NotMapped]
        public List<ModuleDto> ModuleList { get; set; }
        [ValidateNever]
        [NotMapped]
        public IEnumerable<PackageTypeDto> PackageTypeDtoList { get; set; }
        [ValidateNever]
        public PackageDto packageDto { get; set; }
        #endregion

    }
}
