using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel;

public class CoaCategoryDto
{
    public CoaCategoryDto()
    {
    }

    //__________ Main Columns __________
    #region Main_Columns
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int? CoaTypeId { get; set; }
    [StringLength(50)]
    public string CoaCategoryName { get; set; }
    [StringLength(2000)]
    public string CoaCategoryDescription { get; set; }

    #endregion

    //------ Joined Columns -----
    #region Joined_Columns
    #endregion


}
