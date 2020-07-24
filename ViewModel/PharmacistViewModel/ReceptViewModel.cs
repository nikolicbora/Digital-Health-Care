using DigitalnaApoteka.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalnaApoteka.ViewModel.PharmacistViewModel
{
    public class ReceptViewModel
    {
        public Recept Recept { get; set; }
        public List<Lek> Zamene { get; set; }
    }
}