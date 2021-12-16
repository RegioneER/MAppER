using Mapper.Models;
using RER.Tools.MVC.Agid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mapper.Controllers
{
    public class AvvisiController : Controller
    {
        // GET: Avvisi
        public ActionResult Index()
        {
            CruscottoViewModel model = new CruscottoViewModel();
            model.NewsCruscotto = new List<NewsCruscotto>();
            return View(model);
        }
    }
}