using Mapper.Candidatura.Authentication;
using Mapper.Candidatura.Models;
using Mapper.Candidatura.Models.Cookies;
using Mapper.Candidatura.Models.Exceptions;
using Mapper.Candidatura.Models.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mapper.Candidatura.Controllers
{
    public class CandidaturaController : Controller
    {
        // GET: Reparti
        private MapperRepository repository = MapperRepository.GetMapperRepository();
        private bool paginaPubblica { get { return !(User?.Identity?.IsAuthenticated ?? false); } }

        public ActionResult Index()
        {
            SetViewBag();
            return View();
        }
        public void SetViewBag(UtenteDaCensire utente = null, string TipoStruttura = null)
        {
            ViewBag.PaginaPubblica = paginaPubblica;

            List<vwRegione> listaRegioni = new List<vwRegione>();
            List<vwAzienda> listaAziende = new List<vwAzienda>();
            List<TipologiaStruttura> listaTipiStruttura = paginaPubblica ? repository.GetTipiStruttureAttivePubbliche("080") : repository.GetTipiStruttureAttive();
            List<vwStruttura> listaStrutture = new List<vwStruttura>();
            List<Ruoli> listaRuoli = repository.DBContext.Ruoli.Where(x => x.id == 1 || x.id == 4).ToList();

            // per la parte ad accesso anonimo possono essere fatte solo richieste per la regione E.R.
            if (paginaPubblica)
            {
                listaRegioni = repository.GetRegioni().Where(x => x.CodRegione == "080").ToList();
                listaAziende = repository.GetAziende("080");
            }
            else
                listaRegioni = repository.GetRegioni();

            if (utente?.CodRegione != null)
                if (paginaPubblica)
                    listaAziende = repository.GetAziendePubbliche(utente.CodRegione);
                else
                    listaAziende = repository.GetAziende(utente.CodRegione);

            if (utente?.CodAzienda != null)
            {
                if (paginaPubblica)
                    listaStrutture = repository.GetvwStrutturaPubbliche(utente.CodRegione, utente.CodAzienda, TipoStruttura).Where(x => !(x?.isPrivata ?? true)).ToList();
                else
                    listaStrutture = repository.GetvwStrutture(utente.CodRegione, utente.CodAzienda, TipoStruttura);
            }

            ViewBag.Regione = new SelectList(listaRegioni, "CodRegione", "Denominazione", utente?.CodRegione ?? (paginaPubblica ? "080" : null));
            ViewBag.Azienda = new SelectList(listaAziende, "CodAzienda", "Denominazione", utente?.CodAzienda);
            ViewBag.TipoStruttura = new SelectList(listaTipiStruttura, "CodTipologia", "DescrizioneForDisplay", TipoStruttura);
            ViewBag.Struttura = new SelectList(listaStrutture, "KeyStruttura", "Denominazione", utente?.KeyStruttura);
            ViewBag.Ruolo = new SelectList(listaRuoli, "id", "nome", utente?.IdRuolo);
            ViewBag.ServiziFederati = ConfigurationManager.AppSettings["UrlServiziFederati"];
        }



        public ActionResult Salva()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Salva(UtenteDaCensire candidato, string TipoStruttura)
        {
            bool result = RER.Tools.MVC.Agid.Captcha.VerificaTokenReCaptcha(Request, ConfigurationManager.AppSettings["reCaptcha.secret"]);
            if (ModelState.IsValid && result)
            {

                int minPerVerificaIP = int.Parse(ConfigurationManager.AppSettings["verificaIP.DurataPeriodoMin"].ToString());
                int tentativiPerVerificaIP = int.Parse(ConfigurationManager.AppSettings["verificaIP.NumeroMassimoRichiestePeriodo"].ToString());

                // L'utente non deve essere già censito (verifica su CF o email già presenti in tabella utenti)
                // L'utente non deve poter fare altre richieste se ha delle richieste in stato diverso da rifiutato
                bool utenteEsistente = repository.DBContext.Utente.Any(x => x.CodiceFiscale == candidato.CodiceFiscale || x.email == candidato.Email)
                                                        || repository.DBContext.UtenteDaCensire.Any(x => x.CodiceFiscale == candidato.CodiceFiscale && x.IdStato != 2);

                if (utenteEsistente)
                {
                    Alert alert = new Alert();
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
                    Alert alert = new Alert();
                    alert.AlertType = Alert.AlertTypeEnum.Error;
                    alert.Title = "ATTENZIONE! Sono presenti degli errori! Nessun dato è stato salvato";
                    alert.Messages.Add($"Sono state rilevate troppe richieste di candidatura negli ultimi {minPerVerificaIP} minuti. Attendere e riprovare più tardi");
                    TempData["Alert"] = alert;

                    SetViewBag(candidato, TipoStruttura);
                    return View("Index", candidato);
                }

                candidato.IdRuolo = candidato.IdRuolo;
                candidato.IdStato = (int)UtenteDaCensire.TipoStato.In_Attesa;
                candidato.DataCandidatura = DateTime.Now;
                candidato.IndirizzoIPCandidatura = Request.UserHostAddress;
                candidato.Pubblico = true;

                bool strutturaValida = true;
                bool regioneValida = true;
                if (paginaPubblica)
                {
                    if (!repository.DBContext.vwStruttura.Any(x => x.KeyStruttura == candidato.KeyStruttura && !(x.isPrivata ?? false) && (!x.DataFine.HasValue || (x.DataFine.HasValue && x.DataFine > DateTime.Now))))
                    {
                        strutturaValida = false;
                    }

                    if (candidato.CodRegione != "080")
                    {
                        regioneValida = false;
                    }
                }

                if (strutturaValida && regioneValida)
                {
                    repository.SaveUtenteDaCensire(candidato);
                    //TempData["Alert"] = new Alert { Title = "Candidatura inviata correttamente", AlertType = Alert.AlertTypeEnum.Success };
                    Alert alert = new Alert();
                    alert.AlertType = Alert.AlertTypeEnum.Success;
                    alert.Title = "Candidatura inviata correttamente";
                    alert.Messages.Add("Riceverai notizie sulla tua email");

                    ViewBag.ReadOnly = true;

                    TempData["Alert"] = alert;
                }
                else
                {
                    // Qua ci finisce solo se qualcuno magheggia con il codice html, altrimenti non deve poterci arrivare. Il controllo lo metto comunque per sicurezza
                    Alert alert = new Alert();
                    alert.AlertType = Alert.AlertTypeEnum.Error;
                    alert.Title = "ATTENZIONE! Sono presenti degli errori! Nessun dato è stato salvato";
                    if (!regioneValida)
                        alert.Messages.Add("La regione inserita non è valida per la modalità di candidatura corrente!");
                    if (!strutturaValida)
                        alert.Messages.Add("La struttura inserita non è valida per la modalità di candidatura corrente!");
                    TempData["Alert"] = alert;
                }
            }
            else if (!result)
            {
                Alert alert = new Alert();
                alert.AlertType = Alert.AlertTypeEnum.Error;
                alert.Title = "ATTENZIONE! Sono presenti degli errori! Nessun dato è stato salvato";
                alert.Messages.Add("Verifica captcha fallita!");
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
            // se ad accesso anonimo, solo aziende della regione ER
            if (paginaPubblica && codRegione != "080")
                throw new InvalidOperationException("Codice regione non valido");

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
            List<vwStruttura> listaStrutture = repository.GetvwStrutture(codRegione, codAzienda, tipoStruttura);

            // se ad accesso anonimo, solo strutture pubbliche
            if (paginaPubblica)
                listaStrutture = listaStrutture.Where(x => !(x?.isPrivata ?? true)).ToList();

            var result = from struttura in listaStrutture
                         select new
                         {
                             codice = struttura.KeyStruttura,
                             nome = struttura.Denominazione
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}