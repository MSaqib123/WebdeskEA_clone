using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.MappingModel
{
    public class ProductCOADto: BaseEntity
    {
        public ProductCOADto()
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
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        #endregion
    }
}
