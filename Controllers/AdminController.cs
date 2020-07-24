using DigitalnaApoteka.Models;
using DigitalnaApoteka.ViewModel.AdminViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalnaApoteka.Controllers
{
    /*tata bora fam;fma;fa  aa*/
    public class AdminController : Controller
    {
        
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        
        [HttpPost]
        public ActionResult Login([Bind(Include="Username,Password")]AdminViewModel input)
        {
            if (ModelState.IsValid)
            {
                Administrator admin = null;
                using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                {
                    admin = (from a in db.Administrators where a.Username.Equals(input.Username) select a).FirstOrDefault();
                }
                if (admin == null)
                {
                    string loginerror = "ne postoji takav korisnik";
                    ViewData["loginerror"] = loginerror;
                    return View();
                }
                else
                {
                    if (!admin.Password.Equals(input.Password))
                    {
                        string loginerror = "pogresna sifra";
                        ViewData["loginerror"] = loginerror;
                        return View();
                    }
                    Session["user"] = admin;
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View(input);
        }
        // GET: Admin
        public ActionResult Index()
        {

            return View();
        }
        [HttpGet]
        
        public ActionResult RegPatient()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegPatient(Pacijent input)
        {
            if(ModelState.IsValid)
            {
                using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                {
                    input.Legalan = 1;
                    db.Pacijents.Add(input);
                    db.SaveChanges();
                    ViewData["success"] = "uspesna registracija";
                    return View();
                    
                }
                
            }
            return View(input);
        }
        [HttpGet]
        public ActionResult RegPharmacist()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegPharmacist(Apotekar input)
        {
            if (ModelState.IsValid)
            {
                using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                {
                    input.Legalan = 1;
                    db.Apotekars.Add(input);
                    db.SaveChanges();
                    ViewData["success"] = "uspesna registracija";
                    return View();
                }

            }
            return View(input);
        }
        [HttpGet]
        public ActionResult RegDoctor()
        { 
              return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegDoctor(Lekar input)
        {

            if (ModelState.IsValid)
            {
                using (DigitalnoZdravstvoEntities db = new DigitalnoZdravstvoEntities())
                {
                    input.Legalan = 1;
                    db.Lekars.Add(input);
                    db.SaveChanges();
                    ViewData["success"] = "uspesna registracija";
                    return View();
                }
              
            }
            return View(input);
            
        }
        [HttpGet]
        public ActionResult LogOut()
        {
            Session.Remove("user");


            return RedirectToAction("Index", "Home", null);
        }

    }
}