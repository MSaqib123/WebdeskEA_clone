using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Models.ViewModel
{
    public class PosVM
    {
        public PosVM()
        {
            CustomerDtos = new List<CustomerDto>();
            POSConfig = new POSConfigDto();
        }

        public int CustomerId { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<CustomerDto> CustomerDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public POSConfigDto POSConfig { get; set; }
    }
}
