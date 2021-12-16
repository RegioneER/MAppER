using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mapper.Controllers
{
    public class ErroreController : Controller
    {
        public ViewResult MostraErrore(Exception erroreInterno)
        {
            return View("Index", erroreInterno);
        }
    }
}