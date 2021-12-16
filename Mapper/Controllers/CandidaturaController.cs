using Mapper.Authentication;
using Mapper.Models;
using Mapper.Models.Cookies;
using Mapper.Models.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mapper.Controllers
{
    public class CandidaturaController : Controller
    {
        // GET: Reparti
        private MapperRepository repository = MapperRepository.GetMapperRepository();

        public ActionResult Index()
        {
            SetViewBag();

            if (User.Identity.IsAuthenticated)
            {
                var utenteSpid = (RERPrincipal)User;
                UtenteDaCensire utente = new UtenteDaCensire { Nome = utenteSpid.FirstName, Cognome = utenteSpid.LastName, Email = utenteSpid.Email, CodiceFiscale = utenteSpid?.SpidCode ?? utenteSpid?.CodiceFiscale };
                return View(utente);
            }
            else
            {
                return View();
            }
        }
        public void SetViewBag(UtenteDaCensire utente = null, string TipoStruttura = null)
        {
            List<vwRegione> listaRegioni = new List<vwRegione>();
            List<vwAzienda> listaAziende = new List<vwAzienda>();
            List<TipologiaStruttura> listaTipiStruttura = repository.GetTipiStruttureAttive();
            List<vwStruttura> listaStrutture = new List<vwStruttura>();
            List<Ruoli> listaRuoli = repository.DBContext.Ruoli.Where(x => x.id == 1 || x.id == 4).ToList();


            listaRegioni = repository.GetRegioni();
            string pubblicaPrivata = null;

            if (utente?.CodRegione != null)
            {
                listaAziende = repository.GetAziende(utente.CodRegione);
                pubblicaPrivata = utente.CodRegione == "080" ? "2" : null;
            }

            if (utente?.CodAzienda != null)
            {
                listaStrutture = repository.GetStruttureBuff(utente.CodRegione, utente.CodAzienda, TipoStruttura, false, "2", null, utente?.IdRuolo == (int)Ruolo.Aziendale);
            }

            ViewBag.Regione = new SelectList(listaRegioni, "CodRegione", "Denominazione", utente?.CodRegione);
            ViewBag.Azienda = new SelectList(listaAziende, "CodAzienda", "Denominazione", utente?.CodAzienda);
            ViewBag.TipoStruttura = new SelectList(listaTipiStruttura, "CodTipologia", "DescrizioneForDisplay", TipoStruttura);
            ViewBag.Struttura = new SelectList(listaStrutture, "KeyStruttura", "Denominazione", utente?.KeyStruttura);
            ViewBag.Ruolo = new SelectList(listaRuoli, "id", "nome", utente?.IdRuolo);
            ViewBag.isHome = false;
            ViewBag.IsSpid = ((RERPrincipal)User)?.SpidCode != null;
        }


        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Salva(UtenteDaCensire candidato, string TipoStruttura)
        {
            if (ModelState.IsValid)
            {
                Alert alert;
                int minPerVerificaIP = int.Parse(ConfigurationManager.AppSettings["verificaIP.DurataPeriodoMin"].ToString());
                int tentativiPerVerificaIP = int.Parse(ConfigurationManager.AppSettings["verificaIP.NumeroMassimoRichiestePeriodo"].ToString());

                // L'utente non deve essere già censito (verifica su CF o email già presenti in tabella utenti)
                // L'utente non deve poter fare altre richieste se ha delle richieste in stato diverso da rifiutato
                bool utenteEsistente = repository.DBContext.Utente.Any(x => x.CodiceFiscale == candidato.CodiceFiscale || x.email == candidato.Email)
                                                        || repository.DBContext.UtenteDaCensire.Any(x => x.CodiceFiscale == candidato.CodiceFiscale && x.IdStato != 2);

                if (utenteEsistente)
                {
                    alert = new Alert();
                    alert.AlertType = Alert.AlertTypeEnum.Error;
                    alert.Title = "ATTENZIONE! Sono presenti degli errori! Nessun dato è stato salvato";
                    alert.Messages.Add("Non è possibile inserire richieste multiple per lo stesso utente o inserire richieste per utenti già abilitati");
                    TempData["Alert"] = alert;

                    SetViewBag(candidato, TipoStruttura);
                    return View("Index", candidato);
                }

                DateTime tmpDataLimite = DateTime.Now.AddMinutes(-minPerVerificaIP);
                bool esitoVerificaIP = repository.DBContext.UtenteDaCensire.Count(x => x.IndirizzoIPCandidatura == Request.UserHostAddress && x.DataCandidatura > tmpDataLimite) < tentativiPerVerificaIP;
                if (!esitoVerificaIP)
                {
                    alert = new Alert();
                    alert.AlertType = Alert.AlertTypeEnum.Error;
                    alert.Title = "ATTENZIONE! Sono presenti degli errori! Nessun dato è stato salvato";
                    alert.Messages.Add($"Sono state rilevate troppe richieste di candidatura negli ultimi {minPerVerificaIP} minuti. Attendere e riprovare più tardi");
                    TempData["Alert"] = alert;

                    SetViewBag(candidato, TipoStruttura);
                    return View("Index", candidato);
                }

                var utenteSpid = (RERPrincipal)User;

                candidato.Nome = utenteSpid.FirstName;
                candidato.Cognome = utenteSpid.LastName;
                candidato.CodiceFiscale = utenteSpid.CodiceFiscale;

                candidato.IdRuolo = candidato.IdRuolo;
                candidato.IdStato = (int)UtenteDaCensire.TipoStato.In_Attesa;
                candidato.DataCandidatura = DateTime.Now;
                candidato.IndirizzoIPCandidatura = Request.UserHostAddress;


                repository.SaveUtenteDaCensire(candidato);

                alert = new Alert();
                alert.AlertType = Alert.AlertTypeEnum.Success;
                alert.Title = "Candidatura inviata correttamente";
                alert.Messages.Add("Riceverai notizie sulla tua mail");

                ViewBag.ReadOnly = true;

                TempData["Alert"] = alert;

            }
            else
            {
                Alert alert = new Alert();
                alert.AlertType = Alert.AlertTypeEnum.Error;
                alert.Title = "ATTENZIONE! Sono presenti degli errori! Nessun dato è stato salvato";
                foreach (var val in ModelState.Values)
                {
                    foreach (var err in val.Errors)
                    {
                        alert.Messages.Add(err.ErrorMessage);
                    }
                }
                TempData["Alert"] = alert;
            }

            SetViewBag(candidato, TipoStruttura);
            return View("Index", candidato);
        }



        public JsonResult GetAziende(string codRegione)
        {
            List<vwAzienda> listaAziende = repository.GetAziende(codRegione);
            var result = from azienda in listaAziende
                         select new
                         {
                             codice = azienda.CodAzienda,
                             nome = azienda.Denominazione
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStrutture(string codRegione, string codAzienda, string tipoStruttura)
        {
            string pubblicaPrivata = codRegione=="080"? "2": null;
            List<vwStruttura> listaStrutture = repository.GetStruttureBuff(codRegione, codAzienda, tipoStruttura,false, pubblicaPrivata, null, false);

            var result = from struttura in listaStrutture
                         select new
                         {
                             codice = struttura.KeyStruttura,
                             nome = struttura.Denominazione
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Abandon();
            return Redirect(ConfigurationManager.AppSettings["UrlDefault"]);
        }
    }
}