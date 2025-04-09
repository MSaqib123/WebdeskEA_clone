using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.MappingModel
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string CountryName { get; set; }
        public string CountryIso3 { get; set; }
        public string CountryIso2 { get; set; }
        public string Capital { get; set; }
        public string Currency { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public string CountryLatitude { get; set; }
        public string CountryLongitude { get; set; }
    }
}
