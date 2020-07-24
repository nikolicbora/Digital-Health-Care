using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DigitalnaApoteka.Models;
using DigitalnaApoteka.ViewModel.DoctorViewModel;
using DigitalnaApoteka.ViewModel.PatientViewModel;

namespace DigitalnaApoteka.Controllers
{
    
    public class DoctorController : Controller
    {
        
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(DoctorViewModel input)
        {
            if (ModelState.IsValid)
            {
                Lekar lekar = null;
                using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                {
                    lekar = (from l in db.Lekars where l.Username.Equals(input.Username)  select l).FirstOrDefault<Lekar>();
                }
                if (lekar == null)
                {
                    string loginerror = "ne postoji takav korisnik";
                    ViewData["loginerror"]= loginerror;
                    return View("Login");
                }
                else
                {
                    if(!lekar.Password.Equals(input.Password))
                    {
                        string loginerror = "pogresna sifra";
                        ViewData["loginerror"] = loginerror;
                        return View("Login");
                    }
                    Session["user"] = lekar;
                    return RedirectToAction("Index", "Doctor");
                }
            }
            return View("Login",input);
        }
        public ActionResult Index()
        {
            Lekar user = (Lekar)Session["user"];
            Lekar lekar = null;
            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                lekar = (from l in db.Lekars where l.Username.Equals(user.Username) select l).FirstOrDefault<Lekar>();
            }
            ViewData["lekar"] = lekar;
            return View("Index");
        }

        [HttpGet]
        public ActionResult Today()
        {
            Lekar lekar = (Lekar)Session["user"];
            if (lekar == null)
            {
                return View("Login");
            }
            String usernameLekara = ((Lekar)Session["user"]).Username;
            IEnumerable<Termin> danasnjiTermini;
            List<Termin> result = new List<Termin>();
            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                /*List<Termin> danasnji = new List<Termin>();
                foreach(Termin t in db.Lekars.Find(usernameLekara).Termins)
                {
                    if (t.TerminPocetak.Date == DateTime.Now.Date)
                    {
                        Pacijent p = t.Pacijent;
                        danasnji.Add(t);
                    }
                }
                danasnji.Sort(sortirajTermine);
                ViewBag.Termini = danasnji;*/
                DateTime today = DateTime.Today;
                DateTime tommorow = today.AddDays(1);
                danasnjiTermini = (from termin in db.Termins where termin.TerminPocetak >= today && termin.TerminKraj <= tommorow && termin.idLekar.Equals(usernameLekara) orderby termin.TerminPocetak select termin);
                foreach(Termin t in danasnjiTermini)
                {
                    Pacijent p = t.Pacijent;
                    result.Add(t);
                }
                
            }

            return View("Today",result);
        }
        [HttpGet]
        public ActionResult ThisWeek()
        {
            Lekar lekar = (Lekar)Session["user"];
            if (lekar == null)
            {
                return View("Login");
            }
            String usernameLekara = ((Lekar)Session["user"]).Username;

            Dictionary<String, List<Termin>> week = new Dictionary<String, List<Termin>>();
            week.Add("Monday", new List<Termin>());
            week.Add("Tuesday", new List<Termin>());
            week.Add("Wednesday", new List<Termin>());
            week.Add("Thursday", new List<Termin>());
            week.Add("Friday", new List<Termin>());

            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                DateTime pon = DateTime.Now.AddDays(DayOfWeek.Monday - DateTime.Now.Date.DayOfWeek);
                DateTime pet = DateTime.Now.AddDays(DayOfWeek.Friday - DateTime.Now.Date.DayOfWeek);
                
                foreach (Termin t in db.Lekars.Find(usernameLekara).Termins)
                {
                    if (t.TerminPocetak.Date >= pon.Date && t.TerminPocetak.Date <= pet.Date)
                    {
                        Pacijent p = t.Pacijent;
                        //week[t.TerminPocetak.DayOfWeek.ToString()].Add(t);
                        switch (t.TerminPocetak.DayOfWeek)
                        {
                            case DayOfWeek.Monday: { week["Monday"].Add(t); break; }
                            case DayOfWeek.Tuesday: { week["Tuesday"].Add(t); break; }
                            case DayOfWeek.Wednesday: { week["Wednesday"].Add(t); break; }
                            case DayOfWeek.Thursday: { week["Thursday"].Add(t); break; }
                            case DayOfWeek.Friday: { week["Friday"].Add(t); break; }
                        }
                    }
                }
                week["Monday"].Sort(sortirajTermine);
                week["Tuesday"].Sort(sortirajTermine);
                week["Wednesday"].Sort(sortirajTermine);
                week["Thursday"].Sort(sortirajTermine);
                week["Friday"].Sort(sortirajTermine);
            }
            Int32[] niz = new Int32[] {
                week["Monday"].Count, week["Tuesday"].Count, week["Wednesday"].Count, week["Thursday"].Count, week["Friday"].Count
            };

            ViewBag.Most = niz.Max();
            ViewBag.Date = DateTime.Now;
            ViewBag.DayOfWeek = DateTime.Now.DayOfWeek;

            return View("ThisWeek",week);
        }
        [HttpGet]
        public ActionResult ThisMonth()
        {
            Lekar lekar = (Lekar)Session["user"];
            if(lekar == null)
            {
                return View("Login");
            }
            
            return View("ThisMonth");
        }
        public ActionResult WorkMonth(string mesec)
        {
            Lekar lekar = (Lekar)Session["user"];
            if (lekar == null)
            {
                return View("Login");
            }
            DateTime pocetak = new DateTime(DateTime.Today.Year,Int32.Parse(mesec),1);
            int count = DateTime.DaysInMonth(DateTime.Today.Year, Int32.Parse(mesec));
            Dictionary<DateTime, List<Termin>> result = new Dictionary<DateTime, List<Termin>>();
            DateTime kraj = new DateTime(DateTime.Today.Year, Int32.Parse(mesec), DateTime.DaysInMonth(DateTime.Today.Year,Int32.Parse(mesec)));
            for(int i=0;i<count;i++)
            {
                DateTime everyday = pocetak.AddDays(i);
                result.Add(everyday.Date,new List<Termin>());
            }
            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                List<Termin> filter = (from t in db.Lekars.Find(((Lekar)Session["user"]).Username).Termins where t.TerminPocetak.Date >= pocetak && t.TerminKraj.Date <= kraj select t).ToList();
                foreach(Termin t in filter)
                {
                    result[t.TerminPocetak.Date].Add(t);
                }
                foreach(KeyValuePair<DateTime,List<Termin>> pom in result)
                {
                    result[pom.Key].Sort(sortirajTermine);
                }
                return PartialView("WorkMonth",result);
            }
            
        }
        public ActionResult Week()
        {
            DateTime datum = DateTime.Parse(Request.Params.Get("datum"));

            String usernameLekara = ((Lekar)Session["user"]).Username;

            Dictionary<String, List<Termin>> week = new Dictionary<String, List<Termin>>();
            week.Add("Monday", new List<Termin>());
            week.Add("Tuesday", new List<Termin>());
            week.Add("Wednesday", new List<Termin>());
            week.Add("Thursday", new List<Termin>());
            week.Add("Friday", new List<Termin>());

            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                DateTime pon = datum.AddDays(DayOfWeek.Monday - datum.Date.DayOfWeek);
                DateTime pet = datum.AddDays(DayOfWeek.Friday - datum.Date.DayOfWeek);

                foreach (Termin t in db.Lekars.Find(usernameLekara).Termins)
                {
                    if (t.TerminPocetak.Date >= pon.Date && t.TerminPocetak.Date <= pet.Date)
                    {
                        Pacijent p = t.Pacijent;
                        week[t.TerminPocetak.DayOfWeek.ToString()].Add(t);
                    }
                }
                week["Monday"].Sort(sortirajTermine);
                week["Tuesday"].Sort(sortirajTermine);
                week["Wednesday"].Sort(sortirajTermine);
                week["Thursday"].Sort(sortirajTermine);
                week["Friday"].Sort(sortirajTermine);
            }
            Int32[] niz = new Int32[] {
                week["Monday"].Count, week["Tuesday"].Count, week["Wednesday"].Count, week["Thursday"].Count, week["Friday"].Count
            };

            ViewBag.Most = niz.Max();
            ViewBag.Date = datum;
            ViewBag.DayOfWeek = datum.DayOfWeek;

            return View("ThisWeek", week);
        }

        private int sortirajTermine(Termin t1, Termin t2)
        {
            return DateTime.Compare(t1.TerminPocetak, t2.TerminPocetak);
        }

        /*[HttpPost]
        [Route("TerminDetails")]
        public ContentResult detaljiTerminaOdgovor(int id)
        {
            Termin t = null;
            var json = "";
            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                t = db.Termins.Find(id);
                json = new JavaScriptSerializer().Serialize(
                  new { opis = t.Opis, pacijentIme = t.Pacijent.Ime, pacijentPrezime = t.Pacijent.Prezime,
                  pacijentPol = t.Pacijent.Pol, pacijentTelefon = t.Pacijent.Telefon, pacijentEmail = t.Pacijent.Mail}
                );
            }

            return Content(json, "application/json");
        }*/
        [HttpPost]
        public ContentResult detaljiTermin(int id)
        {
            Pacijent result = null;
            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                Termin pom = (from t in db.Termins where t.idTermin == id select t).FirstOrDefault();
                result = pom.Pacijent;
                var json = new JavaScriptSerializer().Serialize(new { ime = result.Ime,prezime=result.Prezime,datumRodjenja=result.DatumRodjenja.ToString("dd-MM-yyyy"),telefon=result.Telefon,mail=result.Mail,opis=pom.Opis,pol= result.Pol.Equals("m") ? "muski" : "zenski",osiguran = result.Osiguran == 1 ? "DA":"NE", hronicar = result.Hronicar == 1 ? "DA" : "NE" });
                return Content(json, "application/json");
            }
           
        }

        [HttpPost]
        //[Route("MedicineRecommendation")]
        public ContentResult predloziNazivaLeka(string naziv)
        {
            
            var json = "";
            
            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                var lekovi = (from lek in db.Leks where lek.Naziv.StartsWith(naziv) select 
                              new
                              {
                                naziv = lek.Naziv,
                                tip = lek.idTip,
                                  nazivTip=lek.Tip.Naziv,
                                  idLek=lek.idLek,
                                  recept=lek.NaRecept == 1?"jeste":"nije",
                                  popust=lek.SnizenjeRecept,
                                  proizvodjac=lek.Proizvodjac,
                                  cena=lek.Cena
                              }).ToList();
                json = new JavaScriptSerializer().Serialize(lekovi);
            }

            return Content(json, "application/json");
        }
        

        [HttpGet]
        public ActionResult NewAppointment()
        {
            Lekar lekar = (Lekar)Session["user"];
            if (lekar == null)
            {
                return View("Login");
            }
            return View("NewAppointment");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAppointment([Bind(Include ="Izbor,Broj,DatumPocetka,Trajanje,Opis")]NewApoitmentViewModel apoitment)
        {
            if (ModelState.IsValid)
            {
                string izbor = apoitment.Izbor;
                string broj = apoitment.Broj;
                DateTime pocetak = apoitment.DatumPocetka;
                int trajanje = apoitment.Trajanje;
                DateTime kraj = pocetak.AddMinutes(trajanje);
                string opis = apoitment.Opis;
                Lekar lekar = (Lekar)Session["user"];
                IEnumerable<Pacijent> pacijenti;
                string tip_broja = "";

                using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                {
                    if (izbor.Equals("JMBG"))
                    {
                        pacijenti = (from p in db.Pacijents where p.JMBG.Equals(broj) select p).ToList();
                        tip_broja = "JMBG";
                    }
                    else if (izbor.Equals("BZK"))
                    {
                        pacijenti = (from p in db.Pacijents where p.BZK.Equals(broj) select p).ToList();
                        tip_broja = "BZK";
                    }
                    else if (izbor.Equals("BLK"))
                    {
                        pacijenti = (from p in db.Pacijents where p.BLK.Equals(broj) select p).ToList();
                        tip_broja = "BLK";
                    }
                    else
                    {
                        pacijenti = (from p in db.Pacijents where p.LBO.Equals(broj) select p).ToList();
                        tip_broja = "LBO";
                    }
                }

                if (pacijenti.Count() == 0)
                {
                    /*ViewBag.MessageType = "danger";
                    ViewBag.Message = "Uneli ste pogresan" + tip_broja + " broj pacijenta!";*/
                    //ModelState.AddModelError("error", "Uneli ste pogresan " + tip_broja + " broj pacijenta!");
                    ViewData["error"] = "Uneli ste pogresan " + tip_broja + " broj pacijenta!";
                    return View("NewAppointment");
                }

                Pacijent pacijent = pacijenti.First();

                if (pocetak < DateTime.Now)
                {
                    /*ViewBag.MessageType = "danger";
                    ViewBag.Message = "Vreme pocetka termina ne sme biti u proslosti!";*/
                    //ModelState.AddModelError("error", "Vreme pocetka termina ne sme biti u proslosti!");
                    ViewData["error"] = "Vreme pocetka termina ne sme biti u proslosti!";
                    return View("NewAppointment");
                }

                DateTime RadnoPocetak = new DateTime(pocetak.Year, pocetak.Month, pocetak.Day, 8, 0, 0);
                DateTime RadnoKraj = new DateTime(pocetak.Year, pocetak.Month, pocetak.Day, 16, 0, 0);

                if (pocetak < RadnoPocetak || kraj > RadnoKraj)
                {
                    /*ViewBag.MessageType = "danger";
                    ViewBag.Message = "Vreme pocetka i kraja termina mora biti izmedju 8 i 16h!";*/
                    //ModelState.AddModelError("error", "Vreme pocetka i kraja termina mora biti izmedju 8 i 16h!");
                    ViewData["error"] = "Vreme pocetka i kraja termina mora biti izmedju 8 i 16h!";
                    return View("NewAppointment");
                }
                /*if(pocetak.DayOfWeek == DayOfWeek.Saturday || pocetak.DayOfWeek == DayOfWeek.Sunday)
                {
                    ViewData["error"] = "zakazani termin ne sme biti subotom ili nedeljom";
                    return View("NewAppointment");
                }*/
                if(pacijent.Osiguran == 0)
                {
                    ViewData["error"] = "pacijent nije osiguran";
                    return View("NewAppointment");
                }
                IEnumerable<Termin> termini;

                using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                {
                    termini = (from termin in db.Termins where termin.TerminPocetak >= RadnoPocetak && termin.TerminKraj <= RadnoKraj && termin.idLekar.Equals(lekar.Username) orderby termin.TerminPocetak ascending select termin).ToList();
                }

                IEnumerable<Termin> ukrstanja = (from termin in termini where (termin.TerminPocetak >= pocetak && termin.TerminPocetak <= kraj) || (termin.TerminPocetak <= pocetak && termin.TerminKraj >= kraj) || (termin.TerminKraj >= pocetak && termin.TerminKraj <= kraj) select termin).ToList();

                if (ukrstanja.Count() > 0)
                {
                    /*ViewBag.MessageType = "danger";
                    ViewBag.Message = "Odabran termin se ukrsta sa terminom zakazanim za " + ukrstanja.First().TerminPocetak.ToString("HH:mm") + " do " + ukrstanja.First().TerminKraj.ToString("HH:mm") + "!";*/
                    //ModelState.AddModelError("error", "Odabran termin se ukrsta sa terminom zakazanim za " + ukrstanja.First().TerminPocetak.ToString("HH:mm") + " do " + ukrstanja.First().TerminKraj.ToString("HH:mm") + "!");
                    ViewData["error"] = "Odabran termin se ukrsta sa terminom zakazanim za " + ukrstanja.First().TerminPocetak.ToString("HH:mm") + " do " + ukrstanja.First().TerminKraj.ToString("HH:mm") + "!";
                    return View("NewAppointment");
                }

                using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                {
                    Termin termin = new Termin();
                    termin.TerminPocetak = pocetak;
                    termin.TerminKraj = kraj;
                    termin.Opis = opis;
                    termin.idLekar = lekar.Username;
                    termin.idPacijent = pacijent.Username;
                    db.Termins.Add(termin);
                    db.SaveChanges();
                }

                /*ViewBag.MessageType = "success";
                 ViewBag.Message = "Uspesno zakazan termin!";*/
                //ModelState.AddModelError("success", "Uspesno zakazan termin!");
                ViewData["success"] = "Uspesno zakazan termin!";
                return View("NewAppointment");
            }
            return View("NewAppointment");
        }
        [HttpGet]
        public ActionResult NewReceipt()
        {
            Lekar lekar = (Lekar)Session["user"];
            if (lekar == null)
            {
                return View("Login");
            }
            return View("NewReceipt");
        }

        [HttpPost]
        public ActionResult AddReceipt(NewReceiptViewModel receiptViewModel)
        {
            if (ModelState.IsValid)
            {
                IEnumerable<Lek> izabranLek = null;
                string tip;
                using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                {
                    izabranLek = (from l in db.Leks where l.idLek == receiptViewModel.IdLeka select l).ToList();
                    tip = izabranLek.First().Tip.Naziv;
                }
                
                string izbor = receiptViewModel.Izbor;//Request.Params.Get("izbor");
                string broj = receiptViewModel.Broj;//Request.Params.Get("broj");
                string naziv = izabranLek.First().Naziv;//Request.Params.Get("naziv");
                Lekar lekar = (Lekar)Session["user"];
                IEnumerable<Pacijent> pacijenti;
                string tip_broja = "";

                using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                {
                    if (izbor.Equals("JMBG"))
                    {
                        pacijenti = (from p in db.Pacijents where p.JMBG.Equals(broj) select p).ToList();
                        tip_broja = "JMBG";
                    }
                    else if (izbor.Equals("BZK"))
                    {
                        pacijenti = (from p in db.Pacijents where p.BZK.Equals(broj) select p).ToList();
                        tip_broja = "BZK";
                    }
                    else if (izbor.Equals("BLK"))
                    {
                        pacijenti = (from p in db.Pacijents where p.BLK.Equals(broj) select p).ToList();
                        tip_broja = "BLK";
                    }
                    else
                    {
                        pacijenti = (from p in db.Pacijents where p.LBO.Equals(broj) select p).ToList();
                        tip_broja = "LBO";
                    }
                }

                if (pacijenti.Count() == 0)
                {
                    //ViewBag.MessageType = "danger";
                    //ViewBag.Message = "Uneli ste pogresan " + tip_broja + " broj pacijenta!";
                    ViewData["error"] = "Uneli ste pogresan " + tip_broja + " broj pacijenta!";
                    return View("NewReceipt");
                }

                /*Lek izabranLek;
                using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                {
                    izabranLek = (from lek in db.Leks where lek.Naziv.Equals(naziv) select lek).First();
                    string tmp = izabranLek.Tip.Naziv;
                }*/
                if(pacijenti.First().Osiguran == 0)
                {
                    ViewData["error"] = "pacijent nije osiguran";
                    return View("NewReceipt");
                }

                if (izabranLek != null)
                {
                    if (tip.Equals("sirup"))
                    {
                        string kolikoDnevno = receiptViewModel.Dnevno.ToString();//Request.Params.Get("kolikoDnevno");
                        string kolikoKolicina = receiptViewModel.Kolicina.ToString();//Request.Params.Get("kolikoKolicina");

                        string doziranjeSirup = kolikoDnevno + " x " + kolikoKolicina + "ml";

                        using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                        {
                            Recept r = new Recept();
                            r.DatumIzdavanja = DateTime.Now;
                            r.DoziranjeSirup = doziranjeSirup;
                            r.DoziranjePrepodne = 0;
                            r.DoziranjePopodne = 0;
                            r.DoziranjeUvece = 0;
                            r.idLek = izabranLek.First().idLek;
                            r.idLekar = lekar.Username;
                            r.idPacijent = pacijenti.First().Username;
                            db.Recepts.Add(r);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        int kolikoPrepodne = receiptViewModel.PrePodne == null?0: receiptViewModel.PrePodne.Value;//Request.Params.Get("kolikoPrepodne");
                        int kolikoPopodne = receiptViewModel.Popodne == null ? 0 : receiptViewModel.Popodne.Value;//Request.Params.Get("kolikoPopodne");
                        int kolikoUvece = receiptViewModel.Uvece == null ? 0 : receiptViewModel.Uvece.Value;//Request.Params.Get("kolikoUvece");

                        using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                        {
                            Recept r = new Recept();
                            r.DatumIzdavanja = DateTime.Now;
                            r.idLek = izabranLek.First().idLek;
                            r.idLekar = lekar.Username;
                            r.idPacijent = pacijenti.First().Username;
                            r.DoziranjePopodne = kolikoPopodne;
                            r.DoziranjePrepodne = kolikoPrepodne;
                            r.DoziranjeUvece = kolikoUvece;
                            r.DoziranjeSirup = "";
                            db.Recepts.Add(r);
                            db.SaveChanges();
                        }
                    }

                    //ViewBag.MessageType = "success";
                    //ViewBag.Message = "Uspesno izdat recept!";
                    ViewData["success"] = "Uspesno izdat recept!";
                    return View("NewReceipt");
                }
                else
                {
                    //ViewBag.MessageType = "danger";
                    //ViewBag.Message = "Ne postoji lek sa nazivom '" + naziv + "'!";
                    ViewData["error"] = "Ne postoji lek sa nazivom '" + naziv + "'!";
                    return View("NewReceipt");
                }
            }
            else
            {
                
                ViewData["error"] = "morate uneti sva polja forme!";
                return View("NewReceipt");
            }
        }
        public ActionResult LogOut()
        {
            Session.Remove("user");
            

            return RedirectToAction("Index", "Home", null);
        }

    }
    
}