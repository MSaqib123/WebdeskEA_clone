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

    public class SRVATBreakdown
    {
        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        [Required]
        public int SRId { get; set; }
        [Required]
        public decimal TaxAmount { get; set; }
        [Required]
        public int TaxId { get; set; }
        [StringLength(100)]
        public string TaxName { get; set; }
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped

        #endregion
    }
}
