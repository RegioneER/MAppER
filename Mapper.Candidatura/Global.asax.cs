using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Mapper.Candidatura.Models;
using System.Web.SessionState;
using Mapper.Candidatura.Models.Repository;
using System.Globalization;
using System.Threading;
using System.Configuration;
using Mapper.Candidatura.Authentication;
using Newtonsoft.Json;
using System.Web.Security;
using Mapper.Candidatura.Controllers;
using Mapper.Candidatura.Models.Exceptions;

namespace Mapper.Candidatura
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("x-frame-options", "DENY");
        }

       
        void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            

            CultureInfo culture = new CultureInfo("it-IT");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

        }

        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            if (exception != null && exception.InnerException != null)
                exception = exception.InnerException;

            try
            {
#if (!DEBUG)
                RER.Tools.ApplicationLogger.WebApplicationLogger.LogEvent("Mapper - Candidature - WWWServizi", this, exception);
#endif

                Server.ClearError();


                //if (exception is Models.Exceptions.ErroreInterno)
                //{
                //    var routeData = new RouteData();
                //    routeData.Values["controller"] = "Errore";
                //    //routeData.Values["ente"] = "RegioneEmiliaRomagna";
                //    routeData.Values["action"] = "Index";
                //    Response.StatusCode = 500;

                //    IController controller = new ErroreController();
                //    var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
                //    controller.Execute(rc);
                //}
                //else
                //{
                    //Response.Redirect($"~/Error");
                    Response.Redirect("~/ErroreSistema.htm", true);
                //}


            }
            catch (Exception ex)
            {
                // Se non sono riuscito a loggare non mi resta che mostrare l'errore anche all'utente
                throw (ex);
            }

        }
    }
}
