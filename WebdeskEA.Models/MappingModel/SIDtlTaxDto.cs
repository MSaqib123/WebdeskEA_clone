using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.DbModel
{
    public class SIDtlTaxDto
    {
        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        [Required]
        public int SIDtlId { get; set; }
        [Required]
        public int SIId { get; set; }
        [Required]
        public decimal TaxAmount { get; set; }
        [Required]
        public int TaxId { get; set; }
        [Required]
        public decimal AfterTaxAmount { get; set; }
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped

        #endregion

    }
}
