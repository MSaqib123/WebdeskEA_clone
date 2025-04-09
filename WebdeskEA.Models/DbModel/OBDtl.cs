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
    public class OBDtl
    {
        public OBDtl()
        {
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public int OBId { get; set; }
        public int COAId { get; set; }
        public string OBDtlTranType { get; set; } = string.Empty;
        public decimal OBDtlOpenBlnc { get; set; }
        #endregion

        //__________ Joined Columns __________
        #region Joined_Columns
        #endregion
    }
}
