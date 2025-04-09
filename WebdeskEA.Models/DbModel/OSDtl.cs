using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel
{
    public class OSDtl
    {
        public OSDtl()
        {
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public int OSId { get; set; }
        public int ProductId { get; set; }
        public int OSDtlQty { get; set; }
        #endregion

        //__________ Joined Columns __________
        #region Joined_Columns
        #endregion
    }
}
