using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mapper.Controllers
{
    [AllowAnonymous]
    public class NonAutenticatoController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}