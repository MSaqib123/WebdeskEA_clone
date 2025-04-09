using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel
{
    public class Tenant: BaseEntity
    {
        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public int TenantTypeId { get; set; }
        public string TenantName { get; set; }
        public string TenantEmail { get; set; }
        public DateTime TenantExpiryDate { get; set; }
        public int TenantCompanies { get; set; }
        public int TenantUsers { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined Columns
        [NotMapped]
        public string? TenantTypeName { get; set; }
        #endregion

    }

}
