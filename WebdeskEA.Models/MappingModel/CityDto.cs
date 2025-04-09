using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.MappingModel
{
    public class CityDto
    {
        public int Id { get; set; }
        public int StateProvinceId { get; set; }
        public string CityName { get; set; }
        public string CityLatitude { get; set; }
        public string CityLongitude { get; set; }
    }
}





