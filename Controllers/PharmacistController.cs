using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using DigitalnaApoteka.Models;
using DigitalnaApoteka.ViewModel.PharmacistViewModel;

namespace DigitalnaApoteka.Controllers
{
    public class PharmacistController : Controller
    {

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(PharmacistViewModel input)
        {
            if (ModelState.IsValid)
            {
                Apotekar apotekar = null;
                using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                {
                    apotekar = (from a in db.Apotekars where a.Username.Equals(input.Username) select a).FirstOrDefault();
                }
                if (apotekar == null)
                {
                    string loginerror = "ne postoji takav korisnik";
                    ViewData["loginerror"] = loginerror;
                    return View();
                }
                else
                {
                    if (!apotekar.Password.Equals(input.Password))
                    {
                        string loginerror = "pogresna sifra";
                        ViewData["loginerror"] = loginerror;
                        return View();
                    }
                    Session["user"] = apotekar;
                    return RedirectToAction("Index", "Pharmacist");
                }
            }
            return View(input);
        }
        
        [HttpGet]
        public ActionResult Index()
        {
            Apotekar apotekar = (Apotekar)Session["user"];

            ViewData["apotekar"] = apotekar;

            return View("Index");
        }

        public ActionResult FindPatient()
        {
            Apotekar apotekar = (Apotekar)Session["user"];

            ViewBag.ImePrezime = apotekar.Prezime + " " + apotekar.Ime;

            string broj = Request.Params.Get("broj");
            string izbor = Request.Params.Get("izbor");

            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                Pacijent pacijent = null;

                if (izbor.Equals("BLK"))
                {
                    pacijent = (from p in db.Pacijents where p.BLK.Equals(broj) select p).FirstOrDefault();
                }
                else if (izbor.Equals("BZK"))
                {
                    pacijent = (from p in db.Pacijents where p.BZK.Equals(broj) select p).FirstOrDefault();
                }
                else if (izbor.Equals("LBO"))
                {
                    pacijent = (from p in db.Pacijents where p.LBO.Equals(broj) select p).FirstOrDefault();
                }
                else
                {
                    pacijent = (from p in db.Pacijents where p.JMBG.Equals(broj) select p).FirstOrDefault();
                }

                if (pacijent != null)
                {
                    ViewBag.Nepodignuti = (from rec in pacijent.Recepts where rec.Obradio == null select rec).Count();
                    List<Lek> lekovi = (from rec in pacijent.Recepts select rec.Lek).ToList();
                    List<Tip> tipovi = (from rec in pacijent.Recepts select rec.Lek.Tip).ToList();
                    List<Lek> zamena = (from rec in pacijent.Recepts select rec.Lek.Zamenas).Aggregate((x,y) => x.Concat(y).ToList()).ToList().Select(x => x.Lek1).ToList();
                    ViewBag.Pacijent = pacijent;

                    List<Recept> recepti = pacijent.Recepts.OrderBy(x => x.Obradio).ThenByDescending(x => x.DatumIzdavanja).Select(x => x).ToList();
                    ViewBag.Recepti = recepti;
                } 
                else
                {
                    ViewBag.Message = "Nije pronadjen pacijent sa unetim " + izbor + "-om!";
                    ViewBag.MessageType = "danger";
                }
            }

            return View("Index");
        }

        [HttpPost]
        [Route("ProcessReceipt")]
        public ContentResult ProcessReceipt()
        {
            int idRecepta = int.Parse(Request.Params.Get("id").ToString());
            int zamena = int.Parse(Request.Params.Get("zamena").ToString());

            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                Recept r = db.Recepts.Find(idRecepta);

                if (zamena == -1)
                {
                    r.Obradio = ((Apotekar)Session["user"]).Username;
                } else
                {
                    r.Lek = db.Leks.Find(zamena);
                    r.Obradio = ((Apotekar)Session["user"]).Username;
                }

                db.SaveChanges();
            }

            return Content("success", "text/plain");
        }

        [HttpPost]
        public ActionResult PacientReceipts(string izbor,string broj)
        {
            Apotekar apotekar = (Apotekar)Session["user"];
            
            ViewData["apotekar"] = apotekar;

            if (izbor.Equals("none") || broj.Equals("")) { ViewData["error"] = "morate uneti sve podatke korisnika"; return View("Index"); }

            List<ReceptViewModel> result=new List<ReceptViewModel>();
            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                Pacijent pacijent = null;

                if (izbor.Equals("BLK"))
                {
                    pacijent = db.Pacijents.Where(p => p.BLK.Equals(broj)).Include("Recepts.Lek.Tip").Include("Recepts.Pacijent").Include("Recepts.Lekar").FirstOrDefault();
                }
                else if (izbor.Equals("BZK"))
                {
                    pacijent = db.Pacijents.Where(p => p.BZK.Equals(broj)).Include("Recepts.Lek.Tip").Include("Recepts.Pacijent").Include("Recepts.Lekar").FirstOrDefault();
                }
                else if (izbor.Equals("LBO"))
                {
                    pacijent = db.Pacijents.Where(p => p.LBO.Equals(broj)).Include("Recepts.Lek.Tip").Include("Recepts.Pacijent").Include("Recepts.Lekar").FirstOrDefault();
                }
                else
                {
                    pacijent = db.Pacijents.Where(p => p.JMBG.Equals(broj)).Include("Recepts.Lek.Tip").Include("Recepts.Pacijent").Include("Recepts.Lekar").FirstOrDefault();
                }
                if (pacijent != null)
                {
                    foreach(Recept recept in pacijent.Recepts)
                    {
                        if(recept.DatumIzdavanja >= DateTime.Now.AddDays(-30) && recept.Obradio == null)
                        {
                            List<Lek> zamene = new List<Lek>();
                            List<Zamena> pom = db.Zamenas.Where(z => z.idLek1 == recept.idLek || z.idLek2 == recept.idLek).Include("Lek.Tip").Include("Lek1.Tip").ToList();
                            foreach (Zamena z in pom) 
                            {
                                zamene.Add(z.idLek1 == recept.idLek ? z.Lek1 : z.Lek);
                            }
                            result.Add(new ReceptViewModel() { Recept = recept, Zamene = zamene });
                        }
                    }
                    return View(result);
                }
                else
                {
                    ViewData["error"] = "Ne postoji takav pacijent";
                    return View("Index");
                }
            }
           
        }
        [HttpPost]
        public ContentResult Obrada(string idR,string idL)
        {
            int id1 = Int32.Parse(idR);
            int id2 = Int32.Parse(idL);
            var json = "";
            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                    Recept r = db.Recepts.Find(id1);
                    r.Lek = db.Leks.Find(id2);
                    r.Obradio = ((Apotekar)Session["user"]).Username;

                    db.SaveChanges();
                    var result = new { id =idR};
                    json = new JavaScriptSerializer().Serialize(result);
            }
            return Content(json, "application/json");
        }
        public ActionResult LogOut()
        {
            Session.Remove("user");


            return RedirectToAction("Index", "Home", null);
        }
        
    }
}