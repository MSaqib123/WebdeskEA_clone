using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel
{

    public class SRDtl
    {
        public SRDtl()
        {
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public int SRId { get; set; } // int, not null
        public int ProductId { get; set; } // int, not null
        public int SRDtlQty { get; set; } // int, not null
        public decimal SRDtlPrice { get; set; } // decimal(18, 2), not null
        public decimal SRDtlTotal { get; set; } // decimal(18, 2), not null
        public decimal SRDtlTotalAfterVAT { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        #endregion
    }

}
