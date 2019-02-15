using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigiTools.Models
{
    public class RoleViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un rol...", AllowEmptyStrings = false)]
        [Display(Name = "Seleccione rol")]
        public string IdRole { get; set; }
        public List<SelectListItem> RoleList { get; set; }
    }
}