using WebdeskEA.Models.DbModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.MappingModel
{
    public class ModuleDto:BaseEntity
    {
        public ModuleDto()
        {
            ModuleList = new List<ModuleDto>();
            MenuList = new List<ModuleDto>();
        }
       
        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string ModuleName { get; set; } = "";
        public bool IsModule { get; set; }
        public bool IsSubModule { get; set; }
        public bool IsForm { get; set; }
        public bool SubForm { get; set; }
        public int? ParentModuleId { get; set; }
        [ValidateNever]
        [MaxLength(500)]
        public string ModuleUrl { get; set; } = "";
        [ValidateNever]
        [MaxLength(500)]
        public string ModulePath { get; set; } = "";
        public bool IsTab { get; set; }
        [MaxLength(200)]
        public string ModuleIcon { get; set; }
        public bool IsExpand { get; set; }
        [ValidateNever]
        public bool IsLabel { get; set; }
        [ValidateNever]
        public string LabelText { get; set; }
        public int Order { get; set; }  // New property

        #endregion

        //__________ List and Single Object__________
        #region Note Mapped
        [ValidateNever]
        public IEnumerable<ModuleDto> ModuleList { get; set; }
        [ValidateNever]
        public IEnumerable<ModuleDto> MenuList { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined Columns
        #endregion
    }
}
