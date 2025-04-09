using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.MappingModel
{
    public class SODtlDto
    {
        public SODtlDto()
        {
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public int SOId { get; set; }
        public int ProductId { get; set; }
        public int SODtlQty { get; set; }
        public decimal SODtlPrice { get; set; }
        public decimal SODtlTotal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SODtlTotalAfterVAT { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string ProductName { get; set; }
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        #endregion
    }
}
