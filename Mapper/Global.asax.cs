using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Mapper.Models;
using System.Web.SessionState;
using Mapper.Models.Repository;
using System.Globalization;
using System.Threading;
using System.Configuration;
using Mapper.Authentication;
using Newtonsoft.Json;
using System.Web.Security;
using Mapper.Controllers;

namespace Mapper
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

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpContext httpContext = HttpContext.Current;
            if (httpContext == null)
                return;

            HttpRequest httpRequest = httpContext.Request;

            string codiceFiscale = httpRequest.Headers["CodiceFiscale"];
            string nome = httpRequest.Headers["nome"];
            string cognome = httpRequest.Headers["cognome"];
            string autMode = httpRequest.Headers["authenticationMethod"];
            string spidCode = httpRequest.Headers["spidCode"];
            string trustLevel = httpRequest.Headers["trustlevel"];
            string policyLevel = httpRequest.Headers["policylevel"];
            string username = !String.IsNullOrEmpty($"{httpRequest.Headers["domain"]}") ? $"{httpRequest.Headers["domain"]}\\{httpRequest.Headers["username"]}" : "";
            string email = httpRequest.Headers["email"];

#if (DEBUG)
            codiceFiscale = "AAABBB70B15F097T";
            nome = "Utente";
            cognome = "Prova";
            email = "utente.prova@email.it";
            username = "utenteRegionale";

            trustLevel = "alto";
            policyLevel = "alto";
#endif

            User utente = new User()
            {
                ActivationCode = Guid.NewGuid(),
                CodiceFiscale = codiceFiscale,
                Email = email,
                FirstName = nome,
                LastName = cognome,
                IsActive = true,
                Password = "",
                UserId = 1,
                Username = !String.IsNullOrEmpty(username) ? username : codiceFiscale,
                TrustLevel = trustLevel,
                PolicyLevel = policyLevel,
                AuthenticationMethod = autMode,
                SpidCode = spidCode,
                UsaServiziFederati = String.IsNullOrEmpty(username),
                Roles = new List<Role>()
            };

            string userData = JsonConvert.SerializeObject(utente);
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket
                (
                1, utente.Username, DateTime.Now, DateTime.Now.AddMinutes(30), false, userData
                );

            string enTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie("Mapper.AuthCookie", enTicket);
            Response.Cookies.Add(faCookie);
        }

        void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            CultureInfo culture = new CultureInfo("it-IT");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            if (Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState)
            {
                List<string> ignoreUrl = new List<string>();
                ignoreUrl.Add(string.Format("{0}/{1}", Request.ApplicationPath, "Errore"));
                ignoreUrl.Add(string.Format("{0}/{1}", Request.ApplicationPath, "NonAutenticato"));
                ignoreUrl.Add(string.Format("{0}/{1}", Request.ApplicationPath, "api/Protocollo"));

                if (ignoreUrl.Contains(Context.Request.ServerVariables["SCRIPT_NAME"]))
                    return;

                HttpCookie authCookie = Request.Cookies["Mapper.AuthCookie"];
                if (authCookie != null)
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                    var serializeModel = new RERMembershipUser(JsonConvert.DeserializeObject<User>(authTicket.UserData));

                    RERPrincipal principal = new RERPrincipal(authTicket.Name);

                    principal.UserId = serializeModel.UserId;
                    principal.FirstName = serializeModel.FirstName;
                    principal.LastName = serializeModel.LastName;
                    principal.CodiceFiscale = serializeModel.CodiceFiscale;
                    principal.AuthenticationMethod = serializeModel.AuthenticationMethod;
                    principal.PolicyLevel = serializeModel.PolicyLevel;
                    principal.TrustLevel = serializeModel.TrustLevel;
                    principal.SpidCode = serializeModel.SpidCode;
                    principal.Email = serializeModel.Email;
                    principal.UserName = serializeModel.UserName;

                    principal.Roles = (from r in serializeModel.Roles select r.RoleName).ToArray();

                    HttpContext.Current.User = principal;

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

                    if (!string.IsNullOrEmpty(principal.SpidCode))
                    {
                        if (!principal.IsAbilitato())
                        {
                            throw new ErroreInterno("Non autenticato");
                        }
                    }

                    if (daCandidatura)
                    {
                        /*in candidatura ci può entrare solo un utente federato che non sia già stato inserito nel db*/
                        if (!serializeModel.UsaServiziFederati)
                            throw new ErroreInterno("Utente non autorizzato");
                        else
                        {
                            Entities db = new Entities(true);
                            if (db.Utente.Any(x => x.username == principal.CodiceFiscale && x.attivato && !x.cancellato))
                                throw new ErroreInterno("Utente non autorizzato");
                        }
                    }
                    else
                    {
                        User user = (User)Session["user"];
                        if (user == null)
                        {
                            user = new User();
                            user.Carica(principal.UserName, principal.CodiceFiscale);
                        }
                        user.UsaServiziFederati = serializeModel.UsaServiziFederati;
                        Session["user"] = user;
                    }
                }

            }
        }

        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            if (exception != null && exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            try
            {
#if (!DEBUG)
                if ((bool.Parse(ConfigurationManager.AppSettings["LogErroreInterno"]) && Convert.ToBoolean(ConfigurationManager.AppSettings["LogErroreInterno"]) == true))
                    RER.Tools.ApplicationLogger.WebApplicationLogger.LogEvent(string.Format("{0} - WebUIMVC", "Mapper"), this, exception);
#endif

                Server.ClearError();

                if (exception is Controllers.ErroreInterno)
                {
                    var routeData = new RouteData();
                    routeData.Values["controller"] = "Errore";
                    routeData.Values["action"] = "MostraErrore";
                    routeData.Values.Add("erroreInterno", exception);
                    Response.StatusCode = 200;

                    IController errorController = new ErroreController();
                    errorController.Execute(new RequestContext(
                            new HttpContextWrapper(Context), routeData));

                }
                else
                {
                    Response.Redirect("~/ErroreSistema.htm", true);
                }
            }
            catch (Exception ex)
            {
                // Se non sono riuscito a loggare non mi resta che mostrare l'errore anche all'utente
                throw (ex);
            }
        }
    }
}
