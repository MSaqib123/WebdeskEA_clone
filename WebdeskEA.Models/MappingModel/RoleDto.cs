using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.MappingModel
{
    public class RoleDto:BaseEntity
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
    }
}
