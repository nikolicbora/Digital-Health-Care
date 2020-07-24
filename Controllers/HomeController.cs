using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalnaApoteka.Controllers
{
    public class HomeController : Controller
    {
      
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(int type)
        {
            // 1 - Pacijent, 2 - Farmaceut, 3 - Doktor
            object msg = TempData["Message"];
            
            if (msg != null)
            {
                ViewBag.Message = msg.ToString();
                ViewBag.MessageType = TempData["MessageType"].ToString();
            }

            ViewBag.Type = type;

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
    }
}