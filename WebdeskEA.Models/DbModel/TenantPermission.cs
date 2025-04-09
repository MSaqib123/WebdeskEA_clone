using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel
{
    public class TenantPermission //: BaseEntity
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int ModuleId { get; set; }
        public bool IsModuleActive { get; set; }
    }
}
