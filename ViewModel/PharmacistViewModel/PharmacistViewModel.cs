using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalnaApoteka.ViewModel.PharmacistViewModel
{
    public class PharmacistViewModel
    {
        [Required(ErrorMessage = "Morate uneti password")]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Morate uneti username")]
        [Display(Name = "Username")]
        public string Username { get; set; }
    }
}