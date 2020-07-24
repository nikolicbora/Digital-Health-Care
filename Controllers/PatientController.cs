using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitalnaApoteka.Models;
using DigitalnaApoteka.ViewModel.PatientViewModel;

namespace DigitalnaApoteka.Controllers
{
    public class PatientController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(PatientViewModel input)
        {
            if (ModelState.IsValid)
            {
                Pacijent pacijent = null;
                using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                {
                    pacijent = (from p in db.Pacijents where p.Username.Equals(input.Username) select p).FirstOrDefault();
                }
                if (pacijent == null)
                {
                    string loginerror = "ne postoji takav korisnik";
                    ViewData["loginerror"] = loginerror;
                    return View();
                }
                else
                {
                    if (!pacijent.Password.Equals(input.Password))
                    {
                        string loginerror = "pogresna sifra";
                        ViewData["loginerror"] = loginerror;
                        return View();
                    }
                    Session["user"] = pacijent;
                    return RedirectToAction("Index", "Patient");
                }
            }
            return View(input);
        }

        // GET: Patient
        public ActionResult Index()
        {
            /*Pacijent pacijent = (Pacijent)Session["user"];

            ViewBag.ImePrezime = pacijent.Prezime + " " + pacijent.Ime;

            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                Pacijent p = db.Pacijents.Find(pacijent.Username);

                List<Termin> termini = (from termin in p.Termins where termin.TerminPocetak > DateTime.Now orderby termin.TerminPocetak descending select termin).ToList();
                ViewBag.Termini = termini;

                List<Recept> podignutiRecepti = (from recept in p.Recepts where recept.Obradio != null orderby recept.DatumIzdavanja descending select recept).ToList();
                List<Recept> nepodignutiRecepti = (from recept in p.Recepts where recept.Obradio == null orderby recept.DatumIzdavanja descending select recept).ToList();

                foreach(Recept r in podignutiRecepti)
                {
                    Lek tmp = r.Lek;
                }

                foreach (Recept r in nepodignutiRecepti)
                {
                    Lek tmp = r.Lek;
                }

                ViewBag.PodignutiRecepti = podignutiRecepti;
                ViewBag.NepodignutiRecepti = nepodignutiRecepti;
            }*/

            return View();
        }
        public ActionResult Termini()
        {
            Pacijent pacijent = (Pacijent)Session["user"];

            if(pacijent == null)
            {
                return View("Login");
            }

            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                Pacijent p = db.Pacijents.Find(pacijent.Username);
                List<Termin> termini = new List<Termin>();
                termini = (from termin in p.Termins where termin.TerminPocetak > DateTime.Now orderby termin.TerminPocetak descending select termin).ToList();
                return View(termini);
            }
            
        }
        public ActionResult NepodignutiRecepti()
        {
            Pacijent pacijent = (Pacijent)Session["user"];

            if (pacijent == null)
            {
                return View("Login");
            }


            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                Pacijent p = db.Pacijents.Find(pacijent.Username);
                List<Recept> nepodignutiRecepti = new List<Recept>();
                nepodignutiRecepti = (from recept in p.Recepts where recept.Obradio == null && recept.DatumIzdavanja.AddDays(30) >= DateTime.Now orderby recept.DatumIzdavanja descending select recept).ToList();
                foreach (Recept r in nepodignutiRecepti)
                {
                    Lek tmp1 = r.Lek;
                    Tip tm2 = tmp1.Tip;
                }
                return View(nepodignutiRecepti);
            }
           
        }
        public ActionResult PodignutiRecepti()
        {

            Pacijent pacijent = (Pacijent)Session["user"];

            if (pacijent == null)
            {
                return View("Login");
            }

            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                Pacijent p = db.Pacijents.Find(pacijent.Username);
                List<Recept> podignutiRecepti = new List<Recept>();
                podignutiRecepti = (from recept in p.Recepts where recept.Obradio != null && recept.DatumIzdavanja.Year == DateTime.Now.Year orderby recept.DatumIzdavanja descending select recept).ToList();
                foreach (Recept r in podignutiRecepti)
                {
                    Lek tmp1 = r.Lek;
                    Tip tm2 = tmp1.Tip;
                }
                return View(podignutiRecepti);
            }
        }
        public ActionResult IstekliRecepti()
        {
            Pacijent pacijent = (Pacijent)Session["user"];

            if (pacijent == null)
            {
                return View("Login");
            }

            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
                Pacijent p = db.Pacijents.Find(pacijent.Username);
                List<Recept> istekliRecepti = new List<Recept>();
                istekliRecepti = (from recept in p.Recepts where recept.Obradio == null && recept.DatumIzdavanja.AddDays(30) < DateTime.Now && recept.DatumIzdavanja.Year == DateTime.Now.Year orderby recept.DatumIzdavanja descending select recept).ToList();
                foreach (Recept r in istekliRecepti)
                {
                    Lek tmp1 = r.Lek;
                    Tip tm2 = tmp1.Tip;
                }
                return View(istekliRecepti);
            }
        }
        [HttpGet]
        public ActionResult Mapa()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Apoteke()
        {

            using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
            {
               var apoteke = db.Apotekas.Select(a =>new { id = a.idApoteka,lat=a.lat,lon=a.lon,naziv=a.naziv}).ToList();
               /* foreach(Apoteka a in apoteke)
                {
                    Apoteka pom = a;
                }*/
                return Json(apoteke, JsonRequestBehavior.AllowGet);

            }

        }
        public ActionResult LogOut()
        {
            Session.Remove("user");


            return RedirectToAction("Index", "Home", null);
        }
    }
}