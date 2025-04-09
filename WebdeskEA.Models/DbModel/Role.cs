using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel
{
    public class Role:BaseEntity
    {
        [Key]
        public int Id { get; set; } // Corresponds to [id] [int] NOT NULL
        public string RoleName { get; set; } // Corresponds to   NOT NULL
    }

}
