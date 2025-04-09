using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.MappingModel
{
    public class CompanyUserDto
    {

        

        public CompanyUserDto()
        {
           UserList = new List<ApplicationUserDto>();
           CompanyList = new List<CompanyDto>();
           UserIds = new List<string>();
        }
        [Required]
        public int CompanyId { get; set; }
        [ValidateNever]
        public string UserId { get; set; }
        [Required]
        public List<string> UserIds { get; set; } 


        //________ Display Members only ________
        [ValidateNever]
        public string CompanyName { get; set; }
        [ValidateNever]
        public string UserName { get; set; }
        [ValidateNever]
        public IEnumerable<ApplicationUserDto> UserList { get; set; }

        [ValidateNever]
        public IEnumerable<CompanyDto> CompanyList { get; set; }
    }
}
