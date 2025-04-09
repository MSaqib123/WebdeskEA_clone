using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.ViewModel
{
    public class PosCartItemVM
    {
        public int Id { get; set; }              // Product ID
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int Qty { get; set; }
    }
}
