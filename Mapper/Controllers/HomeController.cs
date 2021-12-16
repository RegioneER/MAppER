using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Mapper.Models;
using System.Web.Routing;
using Mapper.Models.Repository;
using Microsoft.Ajax.Utilities;
using Mapper.Views;
using Mapper.Authentication;
using System.IO;
using RER.Tools.MVC.Agid;

namespace Mapper.Controllers
{
    
    [Authorize]
    public class HomeController : Controller
    {
        /// <summary>
        /// Scarica eventuali file da db
        /// </summary>
        /// <param name="IDDocumento">IDDocumento da scaricare</param>
        /// <returns></returns>
        public FileResult ScaricaFileNewsManager(int IDDocumento)
        {
            var res = new byte[100];
            return File(res, System.Net.Mime.MediaTypeNames.Application.Octet, "nomefile.pdf");
        }
    }


}