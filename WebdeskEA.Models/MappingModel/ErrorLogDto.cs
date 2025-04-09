using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.MappingModel
{
    public class ErrorLogDto
    {
        public int Id { get; set; }
        public string Area { get; set; }
        public string Controller { get; set; }
        public string FormName { get; set; }
        public string ActionName { get; set; }
        public string StatusCode { get; set; }
        public string ErrorLogShortDescription { get; set; }
        public string ErrorLogLongDescription { get; set; }
        public string ErrorFrom { get; set; }
        public string Username { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }
}
