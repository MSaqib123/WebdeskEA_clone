using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.MappingModel
{
    public class TenantPrefixDto : BaseEntity
    {

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string PrefixType { get; set; }
        public string Prefix { get; set; }
        public string Separator { get; set; }
        public int PrefixStartNum { get; set; }
        public int PrefixIncrement { get; set; }

        #endregion
    }
   

}
