using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.ViewModel
{
    public class PosCartVM
    {
        public int CustomerId { get; set; }
        public string DiscountType { get; set; }     // "percent" or "fixed"
        public decimal DiscountValue { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }

        public int? SoId { get; set; } // Identify the hold record we are converting


        public List<PosCartItemVM> CartItems { get; set; } = new List<PosCartItemVM>();
        public List<PosCartTaxItemVM> CartTaxItems { get; set; } = new List<PosCartTaxItemVM>();

    }
}
