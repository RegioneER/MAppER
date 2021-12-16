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
    public class ErroreInterno : Exception
    {
        public ErroreInterno(string message) : base(message)
        {

        }
    }

    [Authorize]
    public class BaseController : Controller
    {
        private MapperRepository repository = MapperRepository.GetMapperRepository();

        public bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (User != null && User.Identity.IsAuthenticated)
            {
                User utente = (User)Session["user"];
                string actionName = filterContext.ActionDescriptor.ActionName;
                string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                int idScheda;
                string keyReparto;
                int idOsservazione;
                int idUtente;
                string codStruttura;

                if (filterContext.HttpContext.Request.HttpMethod.ToUpper() == "POST")
                {
                    if (filterContext.Controller is SchedaController || filterContext.Controller is OsservazioneController)
                    {
                        if (int.TryParse(filterContext.HttpContext.Request.QueryString["IDScheda"], out idScheda) && idScheda > 0)
                        {
                            if (!VerificaAccessoScheda(idScheda, utente))
                                throw new ErroreInterno("Utente non autorizzato");

                        }
                    }
                    else if (filterContext.Controller is OpportunitaController)
                    {
                        if (int.TryParse(filterContext.HttpContext.Request.QueryString["IDOsservazione"], out idOsservazione) && idOsservazione > 0)
                        {
                            Osservazione osservazione = repository.GetOsservazione(idOsservazione);
                            if (osservazione == null)
                                throw new ErroreInterno("Osservazione non trovata");

                            idScheda = osservazione.idScheda;
                            if (!VerificaAccessoScheda(idScheda, utente))
                                throw new ErroreInterno("Utente non autorizzato");

                        }
                    }
                    else if (filterContext.Controller is UtenteController || filterContext.Controller is UtentiController)
                    {
                        if (utente.Ruolo == Ruolo.NonAssociato || utente.Ruolo == Ruolo.Osservatore)
                            throw new ErroreInterno("Utente non autorizzato");

                        if (int.TryParse(filterContext.HttpContext.Request.QueryString["idUtente"], out idUtente) && idUtente > 0)
                        {
                            if (!VerificaAccessoUtente(idUtente, utente))
                                throw new ErroreInterno("Utente non autorizzato");
                        }
                    }
                    else if (filterContext.Controller is UtenteStrutturaController)
                    {
                        if (utente.Ruolo == Ruolo.NonAssociato || utente.Ruolo == Ruolo.Osservatore)
                            throw new ErroreInterno("Utente non autorizzato");

                        if (int.TryParse(filterContext.HttpContext.Request.QueryString["idUtente"], out idUtente) && idUtente > 0)
                        {
                            if (!VerificaAccessoUtente(idUtente, utente))
                                throw new ErroreInterno("Utente non autorizzato");

                            if (int.TryParse(filterContext.HttpContext.Request.QueryString["ID"], out int id) && id > 0)
                            {
                                if (!VerificaAccessoUtenteStruttura(id, idUtente, utente))
                                    throw new ErroreInterno("Utente non autorizzato");
                            }
                        }
                    }
                    else if (filterContext.Controller is UtenteStrutturaController)
                    {
                        if (utente.Ruolo == Ruolo.NonAssociato || utente.Ruolo == Ruolo.Osservatore)
                            throw new ErroreInterno("Utente non autorizzato");

                        if (int.TryParse(filterContext.HttpContext.Request.QueryString["idUtente"], out idUtente) && idUtente > 0)
                        {
                            if (!VerificaAccessoUtente(idUtente, utente))
                                throw new ErroreInterno("Utente non autorizzato");

                            if (int.TryParse(filterContext.HttpContext.Request.QueryString["ID"], out int id) && id > 0)
                            {
                                if (!VerificaAccessoUtenteStruttura(id, idUtente, utente))
                                    throw new ErroreInterno("Utente non autorizzato");
                            }
                        }
                    }
                    else if (filterContext.Controller is RepartoController || filterContext.Controller is RepartiController)
                    {
                        if (utente.Ruolo == Ruolo.NonAssociato || utente.Ruolo == Ruolo.Osservatore)
                            throw new ErroreInterno("Utente non autorizzato");

                        if (!String.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["keyReparto"]))
                        {
                            keyReparto = filterContext.HttpContext.Request.QueryString["keyReparto"];
                            if (!VerificaAccessoReparto(keyReparto, utente))
                                throw new ErroreInterno("Utente non autorizzato");
                        }
                    }
                    else if (filterContext.Controller is CandidatureController)
                    {
                        if (utente.Ruolo == Ruolo.NonAssociato || utente.Ruolo == Ruolo.Osservatore)
                            throw new ErroreInterno("Utente non autorizzato");
                    }
                }
                else if (filterContext.HttpContext.Request.HttpMethod.ToUpper() == "GET")
                {
                    if (filterContext.Controller is SchedaController || filterContext.Controller is OsservazioneController)
                    {
                        if (int.TryParse(filterContext.HttpContext.Request.QueryString["IDScheda"], out idScheda) && idScheda > 0)
                        {
                            if (!VerificaAccessoScheda(idScheda, utente))
                                throw new ErroreInterno("Utente non autorizzato");
                        }
                    }
                    else if (filterContext.Controller is OpportunitaController)
                    {
                        if (int.TryParse(filterContext.HttpContext.Request.QueryString["IDOsservazione"], out idOsservazione) && idOsservazione > 0)
                        {
                            Osservazione osservazione = repository.GetOsservazione(idOsservazione);
                            if (osservazione == null)
                                throw new ErroreInterno("Osservazione non trovata");

                            idScheda = osservazione.idScheda;
                            if (!VerificaAccessoScheda(idScheda, utente))
                                throw new ErroreInterno("Utente non autorizzato");

                        }
                    }
                    else if (filterContext.Controller is UtenteController || filterContext.Controller is UtentiController)
                    {
                        if (utente.Ruolo == Ruolo.NonAssociato || utente.Ruolo == Ruolo.Osservatore)
                            throw new ErroreInterno("Utente non autorizzato");

                        if (int.TryParse(filterContext.HttpContext.Request.QueryString["idUtente"], out idUtente) && idUtente > 0)
                        {
                            if (!VerificaAccessoUtente(idUtente, utente))
                                throw new ErroreInterno("Utente non autorizzato");
                        }
                    }
                    else if (filterContext.Controller is UtenteStrutturaController)
                    {
                        if (utente.Ruolo == Ruolo.NonAssociato || utente.Ruolo == Ruolo.Osservatore)
                            throw new ErroreInterno("Utente non autorizzato");

                        if (int.TryParse(filterContext.HttpContext.Request.QueryString["idUtente"], out idUtente) && idUtente > 0)
                        {
                            if (!VerificaAccessoUtente(idUtente, utente))
                                throw new ErroreInterno("Utente non autorizzato");

                            if (int.TryParse(filterContext.HttpContext.Request.QueryString["ID"], out int id) && id > 0)
                            {
                                if (!VerificaAccessoUtenteStruttura(id, idUtente, utente))
                                    throw new ErroreInterno("Utente non autorizzato");
                            }
                        }
                    }
                    else if (filterContext.Controller is RepartoController || filterContext.Controller is RepartiController)
                    {
                        if (utente.Ruolo == Ruolo.NonAssociato || utente.Ruolo == Ruolo.Osservatore)
                            throw new ErroreInterno("Utente non autorizzato");

                        if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["keyReparto"]))
                        {
                            keyReparto = filterContext.HttpContext.Request.QueryString["keyReparto"];
                            if (!VerificaAccessoReparto(keyReparto, utente))
                                throw new ErroreInterno("Utente non autorizzato");
                        }

                    }
                    else if (filterContext.Controller is CandidatureController)
                    {
                        if (utente.Ruolo == Ruolo.NonAssociato || utente.Ruolo == Ruolo.Osservatore)
                            throw new ErroreInterno("Utente non autorizzato");
                    }
                }
            }
        }

        public bool VerificaAccessoReparto(string keyReparto, User utente)
        {
            bool autorizzato = false;
            vwReparto reparto = repository.GetReparto(keyReparto);
            if (reparto == null)
            {
                throw new ErroreInterno("Reparto non trovato");
            }

            if (utente.Ruolo == Ruolo.Regionale)
            {
                if (utente.UtenteStrutture.Count(x => x.codRegione == reparto.CodRegione) > 0)
                {
                    autorizzato = true;
                }
            }
            else if (utente.Ruolo == Ruolo.Aziendale)
            {
                List<String> strutture = new List<String>();
                foreach (UtenteStruttura us in utente.UtenteStrutture)
                {
                    strutture.AddRange((from s in repository.GetStruttureBuff(us.codRegione, us.codAzienda, null, true, null, null, true)
                                        select s.KeyStruttura).ToList());
                }
                if (utente.UtenteStrutture.Count(x => x.codRegione == reparto.CodRegione && x.codAzienda == reparto.CodAzienda) > 0 && strutture.Contains(reparto.keyStruttura))
                    autorizzato = true;
            }
            else if (utente.Ruolo == Ruolo.ReferenteStruttura)
            {
                if (utente.UtenteStrutture.Count(x => x.CodiceStruttura == reparto.CodiceStruttura) > 0)
                    autorizzato = true;
            }
            else if (utente.Ruolo == Ruolo.Osservatore)
            {
                return false;
            }
            return autorizzato;
        }

        public bool VerificaAccessoScheda(int idScheda, User utente)
        {
            bool autorizzato = false;
            Scheda scheda = repository.GetScheda(idScheda);
            if (scheda == null)
                throw new ErroreInterno("Scheda non trovata");

            if (utente.Ruolo == Ruolo.Regionale)
            {
                if (utente.UtenteStrutture.Count(x => x.codRegione == scheda.Reparto.CodRegione) > 0)
                    autorizzato = true;
            }
            else if (utente.Ruolo == Ruolo.Aziendale)
            {
                List<String> strutture = new List<String>();
                foreach (UtenteStruttura us in utente.UtenteStrutture)
                {
                    strutture.AddRange((from s in repository.GetStruttureBuff(us.codRegione, us.codAzienda, null, true, null, null, true)
                                        select s.KeyStruttura).ToList());
                }
                if (utente.UtenteStrutture.Count(x => x.codRegione == scheda.Reparto.CodRegione && x.codAzienda == scheda.Reparto.CodAzienda) > 0 && strutture.Contains(scheda.Reparto.keyStruttura))
                    autorizzato = true;
            }
            else if (utente.Ruolo == Ruolo.ReferenteStruttura)
            {
                if (utente.UtenteStrutture.Count(x => x.CodiceStruttura == scheda.Reparto.CodiceStruttura) > 0)
                    autorizzato = true;
            }
            else if (utente.Ruolo == Ruolo.Osservatore)
            {
                if (utente.UtenteStrutture.Count(x => x.idReparto == scheda.idReparto && x.idWebServiceReparto == scheda.Reparto.IdWebServiceReparto && x.idUtente == scheda.idUtente) > 0)
                    autorizzato = true;
            }

            return autorizzato;

        }
        public bool VerificaAccessoUtente(int idUtente, User utente)
        {
            bool autorizzato = false;
            Utente utenteDaVerificare = repository.GetUtente(idUtente);

            if (utenteDaVerificare == null)
                throw new ErroreInterno("Utente non trovato");

            if (utenteDaVerificare.idRuolo == (int)Ruolo.NonAssociato)
            {
                autorizzato = true;
            }
            else
            {
                if (utente.Ruolo == Ruolo.Regionale)
                {
                    foreach (UtenteStruttura us in utente.UtenteStrutture)
                    {
                        if (utenteDaVerificare.UtenteStruttura.Count == 0 || utenteDaVerificare.UtenteStruttura.Any(x => x.codRegione == us.codRegione))
                        {
                            autorizzato = true;
                            break;
                        }
                    }
                }
                else if (utente.Ruolo == Ruolo.Aziendale)
                {
                    List<String> strutture = new List<String>();
                    foreach (UtenteStruttura us in utente.UtenteStrutture)
                    {
                        strutture.AddRange((from s in repository.GetStruttureBuff(us.codRegione, us.codAzienda, null, true, null, null, true)
                                            select s.KeyStruttura).ToList());
                    }
                    foreach (UtenteStruttura us in utente.UtenteStrutture)
                    {
                        if (utenteDaVerificare.UtenteStruttura.Count == 0 || utenteDaVerificare.UtenteStruttura.Any(x => x.codRegione == us.codRegione && x.codAzienda == us.codAzienda && (string.IsNullOrEmpty(x.CodiceStruttura) || strutture.Contains(x.CodiceStruttura))))
                        {
                            autorizzato = true;
                            break;
                        }
                    }
                }
                else if (utente.Ruolo == Ruolo.ReferenteStruttura)
                {
                    foreach (UtenteStruttura us in utente.UtenteStrutture)
                    {
                        if (utenteDaVerificare.UtenteStruttura.Count == 0 || utenteDaVerificare.UtenteStruttura.Any(x => x.CodiceStruttura == us.CodiceStruttura))
                        {
                            autorizzato = true;
                            break;
                        }
                    }
                }
                else if (utente.Ruolo == Ruolo.Osservatore)
                {
                    return false;
                }
            }
            return autorizzato;

        }

        public bool VerificaAccessoUtenteStruttura(int id, int idUtente, User utente)
        {
            bool autorizzato = false;
            Utente utenteDaVerificare = repository.GetUtente(idUtente);
            UtenteStruttura utenteStruttura = repository.GetUtenteStruttura(id);

            if (utenteDaVerificare == null)
                throw new ErroreInterno("Utente non trovato");

            if (utenteDaVerificare.idRuolo == (int)Ruolo.NonAssociato)
            {
                autorizzato = true;
            }
            else
            {
                if (utente.Ruolo == Ruolo.Regionale)
                {
                    autorizzato = utente.UtenteStrutture.Any(x => x.codRegione == utenteStruttura.codRegione);
                }
                else if (utente.Ruolo == Ruolo.Aziendale)
                {
                    autorizzato = utente.UtenteStrutture.Any(x => x.codRegione == utenteStruttura.codRegione && x.codAzienda == utenteStruttura.codAzienda);
                }
                else if (utente.Ruolo == Ruolo.ReferenteStruttura)
                {
                    autorizzato = utente.UtenteStrutture.Any(x => x.codRegione == utenteStruttura.codRegione && x.codAzienda == utenteStruttura.codAzienda && x.idStrutturaErogatrice == utenteStruttura.idStrutturaErogatrice && x.idWebServiceStruttura == utenteStruttura.idWebServiceStruttura);
                }
                else if (utente.Ruolo == Ruolo.Osservatore)
                {
                    return false;
                }
            }
            return autorizzato;

        }
    }


}