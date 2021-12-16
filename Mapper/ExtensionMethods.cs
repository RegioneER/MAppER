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
using Mapper.Controllers;
using System.Text.RegularExpressions;
using Mapper.Authentication;
using Mapper.Models;
using RER.Tools.MVC.Agid;

namespace Mapper
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
        #region MenuNavigation
        public static string BuildMenuNavigation(this HtmlHelper helper, bool isHome, string title, string titleMin, string pathBack, User utente)
        {
            StringBuilder breadcrumb = null;
            string pathSprite = VirtualPathUtility.ToAbsolute("~/bootstrapitalia/svg/sprite.svg#it-burger");
            string pathSpriteBack = VirtualPathUtility.ToAbsolute("~/bootstrapitalia/svg/sprite.svg#it-arrow-left");

            breadcrumb = new StringBuilder();
            if (string.Equals(titleMin, "Errore", StringComparison.CurrentCultureIgnoreCase))
                return breadcrumb.ToString();

            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
            var actionName = "";
            var controllerName = "";
            if (routeValues != null)
            {
                if (routeValues.ContainsKey("action"))
                {
                    actionName = routeValues["action"].ToString();
                }
                if (routeValues.ContainsKey("controller"))
                {
                    controllerName = routeValues["controller"].ToString();
                }
            }
            bool daCandidatura = string.Equals(controllerName, "Candidatura", StringComparison.CurrentCultureIgnoreCase);
            int avvisiInEvidenza = 0;
            try
            {
                CruscottoViewModel model = new CruscottoViewModel();
                model.NewsCruscotto = new List<NewsCruscotto>();
                avvisiInEvidenza = model.NewsCruscotto.Count(x => x.IsNew.HasValue && x.IsNew.Value);
            }
            catch (Exception ex)
            { }

            if (isHome)
            {
                /* burger */
                breadcrumb.Append("<div class='col-3'> ");
                breadcrumb.Append("<button class='custom-navbar-toggler' type='button' aria-controls='nav0' aria-expanded='false' aria-label='Toggle navigation' data-target='#nav0'>");
                breadcrumb.Append("<svg class='icon'>");
                breadcrumb.Append("<use xlink:href='" + pathSprite + "'>");
                breadcrumb.Append("</use>");
                breadcrumb.Append("</svg>");
                breadcrumb.Append("</button>");
                breadcrumb.Append("</div> ");
            }
            else
            {
                breadcrumb.Append("<div id='linkBackMin' class='col-3'> ");
                breadcrumb.Append("<a href='/Mapper/" + pathBack + "' class='custom-navbar-toggler' type='button'>");
                breadcrumb.Append("<svg class='icon'>");
                breadcrumb.Append("<use xlink:href='" + pathSpriteBack + "'>");
                breadcrumb.Append("</use>");
                breadcrumb.Append("</svg>");
                breadcrumb.Append("</a>");
                breadcrumb.Append("</div> ");
            }

            breadcrumb.Append("<div class='col-6' id='divTitle'> ");
            breadcrumb.Append("<h4 class='divTitle'>Mapper</h4>");
            breadcrumb.Append("<p class='divTitle'>" + titleMin + "</p>");
            breadcrumb.Append("</div>");

            breadcrumb.Append("<div class='divTitle' class='col-3'> ");
            breadcrumb.Append("<a href='/Mapper/Avvisi/Index' class='divTitle'>");
            breadcrumb.Append("<span>Avvisi (" + avvisiInEvidenza.ToString() + ")</span>");
            breadcrumb.Append("</a>");
            breadcrumb.Append("</div> ");

            breadcrumb.Append("<div class='navbar-collapsable' id='nav0' style='display:none;'>");
            breadcrumb.Append("<div class='overlay' style='display:none;'></div>");
            breadcrumb.Append("<div class='close-div sr-only'>");
            breadcrumb.Append("<button class='btn close-menu' type='button'><span class='it-close'></span>close</button>");
            breadcrumb.Append("</div>");

            breadcrumb.Append("<div class='menu-wrapper'>");
            breadcrumb.Append("<ul class='navbar-nav'>");
            if (daCandidatura)
            {
                breadcrumb.Append("<li class='nav-item'><a class='nav-link' href='/Mapper/Candidatura/Index'><span>Candidatura</span></a></li>");
                breadcrumb.Append("<li class='nav-item'><a class='nav-link' href='/Mapper/Candidatura/Logout'><span>Esci</span></a></li>");
            }
            else
            {
                breadcrumb.Append("<li class='nav-item'><a class='nav-link' href='/Mapper/Avvisi/Index'><span>Avvisi (" + avvisiInEvidenza.ToString() + ")</span></a></li>");
                breadcrumb.Append("<li class='nav-item'><a class='nav-link' href='/Mapper/Schede/Index'><span>Schede</span></a></li>");
                if (utente != null && utente.Ruolo != Ruolo.NonAssociato && utente.Ruolo != Ruolo.Osservatore)
                {
                    breadcrumb.Append("<li class='nav-item'><a class='nav-link' href='/Mapper/Reparti/Index'><span>Reparti</span></a></li>");
                    breadcrumb.Append("<li class='nav-item'><a class='nav-link' href='/Mapper/Utenti/Index'><span>Utenti</span></a></li>");
                    breadcrumb.Append("<li class='nav-item'><a class='nav-link' href='/Mapper/Candidature/Index'><span>Candidature</span></a></li>");
                }
                breadcrumb.Append("<li class='nav-item'><a class='nav-link' href='/Mapper/Schede/Logout'><span>Esci</span></a></li>");
            }
            breadcrumb.Append("</ul>");
            breadcrumb.Append("</div>");
            breadcrumb.Append("</div>");

            return breadcrumb.ToString();
        }
        #endregion

        public static MvcHtmlString SetDisabled(this MvcHtmlString html, bool isDisabled)
        {
            var xDocument = XDocument.Parse(html.ToHtmlString());
            if (!(xDocument.FirstNode is XElement element))
            {
                return html;
            }

            element.SetAttributeValue("disabled", isDisabled ? "disabled" : null);
            return MvcHtmlString.Create(element.ToString());
        }

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

    }
}