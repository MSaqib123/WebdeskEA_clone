using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.MappingModel
{
    public class StateProvinceDto
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string StateProvinceName { get; set; }
        public string StateProvinceCode { get; set; }
        public string StateProvinceLatitude { get; set; }
        public string StateProvinceLongitude { get; set; }
    }
}
