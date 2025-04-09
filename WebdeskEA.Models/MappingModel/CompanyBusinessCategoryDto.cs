using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.MappingModel
{
    public class CompanyBusinessCategoryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? CompanyId { get; set; }

        [ValidateNever]
        public IEnumerable<CompanyBusinessCategoryDto> BusinessCategories { get;set;}
        
        //____ View Properites __
        [ValidateNever]
        public string? CompanyName { get; set; }

    }
}
