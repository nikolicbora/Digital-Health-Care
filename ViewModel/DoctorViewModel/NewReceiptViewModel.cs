using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalnaApoteka.ViewModel.DoctorViewModel
{
    public class NewReceiptViewModel
    {
        [DisplayName("Broj")]
        [Required(ErrorMessage ="Morate uneti {0}")]
        public string Broj { get; set; }

        [DisplayName("Izbor")]
        [Required(ErrorMessage = "Morate uneti {0}")]
        public string Izbor { get; set; }

        
        [Required()]
        public int IdLeka { get; set; }



        //[DisplayName("Dnevna Doza")]
        //[Range(0,5,ErrorMessage ="{0} mara biti izmedju  {1} i {2} puta")]
        
        public int? Dnevno { get; set; }
        //[DisplayName("Dnevna Kolicina")]
        //[Range(0, 200, ErrorMessage = "{0} mara biti izmedju  {1} i {2} mg")]
        public int? Kolicina { get; set; }



        //[DisplayName("Prepodnevna Doza")]
        //[Range(0, 5, ErrorMessage = "{0} mara biti izmedju  {1} i {2} tableta")]
        public int? PrePodne { get; set; }
        //[DisplayName("Popodnevna Doza")]
        //[Range(0, 5, ErrorMessage = "{0} mara biti izmedju  {1} i {2} tableta")]
        public int? Popodne { get; set; }
        //[DisplayName("Vecernja Doza")]
        //[Range(0, 5, ErrorMessage = "{0} mara biti izmedju  {1} i {2} tableta")]
        public int? Uvece { get; set; }
    }
}