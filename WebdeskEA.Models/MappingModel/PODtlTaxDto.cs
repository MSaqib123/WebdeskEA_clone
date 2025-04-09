using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.MappingModel
{

    public class PODtlTaxDto
    {
        public PODtlTaxDto()
        {
        }

        //__________ Main Columns __________
        #region Main_Columns
        public decimal AfterTaxAmount { get; set; }
        public int Id { get; set; }
        public int PODtlId { get; set; }
        public int POId { get; set; }
        public decimal TaxAmount { get; set; }
        public int TaxId { get; set; }
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped

        #endregion

    }

}
