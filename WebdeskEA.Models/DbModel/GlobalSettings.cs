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

    public class GlobalSettings
    {
        //__________ Main Columns __________
        #region Main_Columns

        public int? CompanyId { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        public int Id { get; set; }
        [StringLength(200)]
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public int? TenantId { get; set; }
        [Required]
        public DateTime UpdatedDate { get; set; }
        public string UserId { get; set; }
        public string ValueType { get; set; }

        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped

        #endregion
    }

}
