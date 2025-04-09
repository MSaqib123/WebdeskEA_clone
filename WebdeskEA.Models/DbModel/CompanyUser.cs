using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.DbModel
{
    public class CompanyUser
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string UserId { get; set; }

        [NotMapped]
        public string CompanyName { get; set; }
        [NotMapped]
        public string UserName { get; set; }
    }
}
