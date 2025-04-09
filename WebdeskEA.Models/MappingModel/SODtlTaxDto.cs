using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.MappingModel
{

    public class SODtlTaxDto
    {
        public SODtlTaxDto()
        {
        }

        //__________ Main Columns __________
        #region Main_Columns

        public decimal AfterTaxAmount { get; set; }
        public int Id { get; set; }
        public int SODtlId { get; set; }
        public int SOId { get; set; }
        public decimal TaxAmount { get; set; }
        public int TaxId { get; set; }

        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped

        #endregion

    }

}
