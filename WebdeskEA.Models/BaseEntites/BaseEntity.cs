using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.BaseEntites
{
    public class BaseEntity
    {
        public bool Active { get; set; } = true;
        public string CreatedBy { get; set; } = "User";
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; } = "User";
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
    }
}
