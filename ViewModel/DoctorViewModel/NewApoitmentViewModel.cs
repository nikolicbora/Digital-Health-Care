using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DigitalnaApoteka.ViewModel.DoctorViewModel
{
    public class NewApoitmentViewModel
    {
        [DisplayName("Izbor")]
        [Required(ErrorMessage ="{0} mora biti unesen")]
        
        public string Izbor { get; set; }
       
        
        [DisplayName("Broj")]
        [Required(ErrorMessage ="{0} mora biti unesen")]
       
        public string Broj { get; set; }

        [DisplayName("Datum i vreme pocetka")]
        [Required(ErrorMessage ="{0} mora biti uneseno")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddThh:mm:ss}")]
        public DateTime DatumPocetka { get; set; }

        [DisplayName("Trajanje")]
        [Required(ErrorMessage ="{0} mora biti uneseno")]
        public int Trajanje { get; set; }

        [DisplayName("Opis")]
        [Required(ErrorMessage ="{0} mora biti unesen")]
        public string Opis { get; set; }
    }
}