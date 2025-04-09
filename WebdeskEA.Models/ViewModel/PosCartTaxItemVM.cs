using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.ViewModel
{
    public class PosCartTaxItemVM
    {
        public int ProductId { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal AfterTaxAmount { get; set; }
        public int TaxId { get; set; }
    }
}
