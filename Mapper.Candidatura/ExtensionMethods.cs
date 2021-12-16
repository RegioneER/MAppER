using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Mvc.Razor;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Mapper.Candidatura.Controllers;
using System.Text.RegularExpressions;
using Mapper.Candidatura.Authentication;

namespace Mapper.Candidatura
{
    public enum Ruolo
    {
        Osservatore = 1,
        Aziendale = 2,
        Regionale = 3,
        ReferenteStruttura = 4,
        NonAssociato = 5
        //Amministratore = 6
    }
    public static class ExtensionMethods
    {
             public static bool ValidaCodiceFiscale(string codicefiscale)
        {
            Regex rgx = new Regex(@"^[A-Z]{6}\d{2}[A-Z]\d{2}[A-Z]\d{3}[A-Z]$");
            return rgx.IsMatch(codicefiscale);
        }

        public static bool IsDebug(this HtmlHelper htmlHelper)
        {
#if DEBUG
            return true;
#else
      return false;
#endif
        }

        public static string BuildBreadcrumbNavigation(this HtmlHelper helper, string title)
        {
            StringBuilder breadcrumb = new StringBuilder();
            string controller = helper.ViewContext.RouteData.Values["controller"].ToString();
            string action = helper.ViewContext.RouteData.Values["action"].ToString();
            string briciola = "Candidatura";


            breadcrumb.Append("<li>&nbsp;&nbsp;/&nbsp;&nbsp;</li>");
            breadcrumb.Append("<li>");
            breadcrumb.Append(helper.ActionLink(title, "Index", controller, null, new { @class = "linkbriciola" }).ToHtmlString());
            breadcrumb.Append("</li>");
            if (action.Equals("Cerca", StringComparison.InvariantCultureIgnoreCase) || action.Equals("FilterAndSort", StringComparison.InvariantCultureIgnoreCase))
                breadcrumb.Append("<li>&nbsp;&nbsp;/&nbsp;&nbsp;" + briciola + "</li>");
            else if (action.Equals("Dettaglio", StringComparison.InvariantCultureIgnoreCase))
                breadcrumb.Append("<li>&nbsp;&nbsp;/&nbsp;&nbsp;Dettaglio</li>");



            return breadcrumb.ToString();
        }

    }

  
}