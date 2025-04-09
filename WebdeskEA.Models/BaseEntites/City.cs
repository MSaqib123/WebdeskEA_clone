using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.BaseEntites
{
    public class City
    {
        public int Id { get; set; }
        public int StateProvinceId { get; set; }
        public string CityName { get; set; }
        public string CityLatitude { get; set; }
        public string CityLongitude { get; set; }


    }
}
