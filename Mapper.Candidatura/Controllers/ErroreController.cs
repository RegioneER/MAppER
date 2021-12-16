using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mapper.Candidatura.Controllers
{
    public class ErroreController : Controller
    {
        public ActionResult Index()
        {
            
            var ex = (Exception)Session["LastEx"];
            Session["LastEx"] = null;
            return View(ex);
        }
    }
}