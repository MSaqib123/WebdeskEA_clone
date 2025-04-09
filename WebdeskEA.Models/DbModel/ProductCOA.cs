using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.DbModel
{

    public class ProductCOA
    {
        public ProductCOA()
        {
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductSaleCoaId { get; set; }
        public int ProductBuyCoaId { get; set; }
        public int TenantId { get; set; }
        public int CompanyId { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        #endregion
    }
}
