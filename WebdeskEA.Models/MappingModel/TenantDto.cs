using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.MappingModel
{
    public class TenantDto : BaseEntity
    {
        public TenantDto()
        {
            TenantTypeDtoList = new List<TenantTypeDto>();
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public int TenantTypeId { get; set; }
        public string TenantName { get; set; }
        [ValidateNever]
        public string TenantEmail { get; set; }
        public DateTime TenantExpiryDate { get; set; }
        public int TenantCompanies { get; set; }
        public int TenantUsers { get; set; }

        #endregion

        //__________ List and Single Object__________
        #region Note Mapped
        [ValidateNever]
        public IEnumerable<TenantTypeDto> TenantTypeDtoList { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined Columns
        [NotMapped]
        public string? TenantTypeName { get; set; }
        #endregion

    }
    public enum PrefixType
    {
        PO,
        PI,
        SO,
        SI,
        Customer,
        Supplier,
        Product,
        Swift,
        COA,
        ACC,
        FinancialYear,
        OS,
        OB,
        POS,
        //------ Vouchers ------
        CRV,
        CV,
        EV,
        JV,
        OPV,
        PRV,
        PV,
        SRV,
        SV,
        BRV,
        CPV,
        BPV,
        PCV,
        PR,
        SR
    }
}
