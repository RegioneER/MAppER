using Mapper.Authentication;
using Mapper.Models;
using Mapper.Models.Cookies;
using Mapper.Models.Repository;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mapper.Controllers
{
    public class CandidatureController : BaseController
    {
        private MapperRepository repository = MapperRepository.GetMapperRepository();
        private static User utente = new User();
        private int utentiPerPagina = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["UtentiPerPagina"].ToString());
        // GET: Candidature
        public ActionResult Index()
        {
            utente = (User)Session["user"];
            SetViewBag(null);
            repository.UpdatePosizione(utente.Username, Request.RawUrl, null);
            return View();
        }

        private void SetViewBag(CookieRicercaCandidatura cookie)
        {
            string nome, cognome;
            string codRegione = null;
            string codAzienda = null;
            string codTipoStruttura = null;
            string codStruttura = null;
            string pubblicaPrivata = null;
            int? idRuolo = null;
            bool? pubblico = null;
            int? stato = 0;

            List<vwRegione> listaRegioni = null;
            List<vwAzienda> listaAziende = null;
            List<TipologiaStruttura> listaTipiStruttura = null;
            List<vwStruttura> listaStrutture = null;

            listaTipiStruttura = repository.GetTipiStruttureAttive();

            if (cookie != null)
            {
                codRegione = cookie.Regione;
                codAzienda = cookie.Azienda;
                codTipoStruttura = cookie.TipoStruttura;
                pubblicaPrivata = cookie.PubblicaPrivata;
                codStruttura = cookie.Struttura;
                nome = cookie.Nome;
                cognome = cookie.Cognome;
                idRuolo = cookie.Ruolo;
                pubblicaPrivata = cookie.PubblicaPrivata;
                stato = cookie.Stato;
            }
            else
            {
                codRegione = utente.UtenteStrutture[0].codRegione;
                codAzienda = utente.UtenteStrutture[0].codAzienda;
                codTipoStruttura = utente.UtenteStrutture[0].CodiceTipologiaStruttura;
                codStruttura = utente.UtenteStrutture[0].CodiceStruttura;
            }

            listaRegioni = repository.GetRegioniUtente(utente.Username);
            listaAziende = repository.GetAziende(codRegione, false, null);
            listaStrutture = string.IsNullOrEmpty(codAzienda) ? new List<vwStruttura>() : repository.GetStruttureBuff(codRegione, codAzienda, codTipoStruttura, false, pubblicaPrivata, null, utente.Ruolo == Ruolo.Aziendale);

            if (utente.Ruolo == Ruolo.Aziendale || utente.Ruolo == Ruolo.ReferenteStruttura || utente.Ruolo == Ruolo.Osservatore)
            {
                listaAziende = (from a in listaAziende
                                from us in utente.UtenteStrutture
                                where a.CodRegione == us.codRegione && a.CodAzienda == us.codAzienda
                                select a).Distinct().ToList();
            }
            if (utente.Ruolo == Ruolo.ReferenteStruttura || utente.Ruolo == Ruolo.Osservatore)
            {
                listaStrutture = (from s in listaStrutture
                                  from us in utente.UtenteStrutture
                                  where s.CodRegione == us.codRegione && s.CodAzienda == us.codAzienda && s.IdStrutturaErogatrice == us.idStrutturaErogatrice && s.idWebServiceStruttura == us.idWebServiceStruttura
                                  select s).Distinct().ToList();
            }

            ViewBag.Regione = new SelectList(listaRegioni, "CodRegione", "Denominazione", codRegione);
            ViewBag.Azienda = new SelectList(listaAziende, "CodAzienda", "Denominazione", codAzienda);
            ViewBag.TipoStruttura = new SelectList(listaTipiStruttura, "CodTipologia", "DescrizioneForDisplay", codTipoStruttura);
            ViewBag.Struttura = new SelectList(listaStrutture, "KeyStruttura", "Denominazione", codStruttura);

            ViewBag.EnableRegione = listaRegioni.Count == 1 ? "disabled" : "";
            ViewBag.EnableAzienda = listaAziende.Count == 1 ? "disabled" : "";
            ViewBag.EnableStruttura = listaAziende.Count == 1 && listaStrutture.Count == 1 ? "disabled" : "";

            var selectPubblicaPrivata = new List<SelectListItem>();
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Pubbliche e private", Value = "", Selected = string.IsNullOrEmpty(pubblicaPrivata) });
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Solo pubbliche", Value = "1", Selected = pubblicaPrivata == "1" });
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Solo private", Value = "2", Selected = pubblicaPrivata == "2" });
            ViewBag.PubblicaPrivata = selectPubblicaPrivata;

            var selectRuolo = new List<SelectListItem>();
            selectRuolo.Add(new SelectListItem { Text = "Responsabile struttura", Value = ((int)Ruolo.ReferenteStruttura).ToString(), Selected = idRuolo == (int)Ruolo.ReferenteStruttura });
            selectRuolo.Add(new SelectListItem { Text = "Osservatore", Value = ((int)Ruolo.Osservatore).ToString(), Selected = idRuolo == (int)Ruolo.Osservatore });
            ViewBag.IdRuolo = selectRuolo;

            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Text = "In Attesa di approvazione", Value = "0", Selected = stato == 0 });
            selectList.Add(new SelectListItem { Text = "Approvata", Value = "1", Selected = stato == 1 });
            selectList.Add(new SelectListItem { Text = "Rifiutata", Value = "2", Selected = stato == 2 });
            ViewBag.Stato = selectList;
            ViewBag.Stati = repository.GetStatoCandidatura();

        }

        public ActionResult Cerca()
        {
            return RedirectToAction("BackSearch");
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Cerca(string cognome, string nome, DateTime? DataDal, DateTime? DataAl, string codRegione, string codAzienda, string codTipoStruttura, string PubblicaPrivata, string codStruttura, int Stato, int? IdRuolo, string CodiceFiscale)
        {
            utente = (User)Session["user"];
            int totale = 0;
            CookieRicercaCandidatura cookie = new CookieRicercaCandidatura()
            {
                Regione = codRegione,
                Azienda = codAzienda,
                TipoStruttura = codTipoStruttura,
                PubblicaPrivata = PubblicaPrivata,
                Struttura = codStruttura,
                CodiceFiscale = CodiceFiscale,
                DataCandidaturaAl = DataAl,
                DataCandidaturaDal = DataDal,
                Nome = nome,
                Cognome = cognome,
                Stato = Stato,
                Ruolo = IdRuolo,
                PaginaCorrente = 1
            };
            cookie.CreateCookie(Response, Request);

            bool? pubblico = string.IsNullOrEmpty(PubblicaPrivata) ? (bool?)null : (PubblicaPrivata == "1" ? true : false);

            var listautenti = repository.GetUtentiDaCensire(0, utentiPerPagina, utente.UserId, cognome, nome, IdRuolo, pubblico, DataDal, DataAl, codRegione, codAzienda, codStruttura, Stato, CodiceFiscale, out totale);

            return ViewRisultati(1, listautenti, totale, cookie);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CambiaStato(int Id, int StatoCandidatura)
        {
            UtenteDaCensire u = repository.GetUtenteDaCensire(Id);

            if (ModelState.IsValid)
            {
                u.IdStato = StatoCandidatura;
                repository.SaveUtenteDaCensire(u);
                TempData["Alert"] = new Alert { Title = "Approvazione eseguita con successo", AlertType = Alert.AlertTypeEnum.Success };
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

            return RedirectToAction("BackSearch");
        }


        private ViewResult ViewRisultati(int page, List<UtenteDaCensire> utentiDaCensire, int totaleUtenti, CookieRicercaCandidatura cookie)
        {
            utente = (User)Session["user"];

            SetViewBag(cookie);

            UtenteDaCensireManager UtenteManager = new UtenteDaCensireManager
            {
                TotaleUtentiDaCensire = totaleUtenti,
                UtentiDaCensirePerPagina = utentiPerPagina,
                CurrentPage = page,
                UtentiDaCensirePaged = new StaticPagedList<UtenteDaCensire>(utentiDaCensire, page, utentiPerPagina, totaleUtenti)
            };

            ViewBag.CurrentPage = page;
            repository.UpdatePosizione(utente.Username, Request.RawUrl.Replace("/Cerca", "/BackSearch"), JsonConvert.SerializeObject(cookie));
            return View("Index", UtenteManager);
        }

        public ActionResult ChangePage(int page)
        {
            utente = (User)Session["user"];

            CookieRicercaCandidatura cookie = new CookieRicercaCandidatura().GetCookie(Request);
            cookie.PaginaCorrente = page;
            cookie.CreateCookie(Response, Request);

            int totaleCandidature = 0;
            bool? pubblico = string.IsNullOrEmpty(cookie.PubblicaPrivata) ? (bool?)null : (cookie.PubblicaPrivata == "1" ? true : false);
            List<UtenteDaCensire> candidature = repository.GetUtentiDaCensire(((int)page - 1) * utentiPerPagina, utentiPerPagina, utente.UserId, cookie.Cognome, cookie.Nome, cookie.Ruolo, pubblico, cookie.DataCandidaturaDal, cookie.DataCandidaturaAl, cookie.Regione, cookie.Azienda, cookie.Struttura, cookie.Stato, cookie.CodiceFiscale, out totaleCandidature);
            return ViewRisultati((int)page, candidature, totaleCandidature, cookie);
        }

        public ActionResult BackSearch()
        {
            utente = (User)Session["user"];
            int idUtente = utente.UserId;
            CookieRicercaCandidatura cookie = new CookieRicercaCandidatura().GetCookie(Request);
            if (cookie != null)
            {
                int totaleCandidature = 0;
                bool? pubblico = string.IsNullOrEmpty(cookie.PubblicaPrivata) ? (bool?)null : (cookie.PubblicaPrivata == "1" ? true : false);
                List<UtenteDaCensire> candidature = repository.GetUtentiDaCensire(((int)cookie.PaginaCorrente - 1) * utentiPerPagina, utentiPerPagina, utente.UserId, cookie.Cognome, cookie.Nome, cookie.Ruolo, pubblico, cookie.DataCandidaturaDal, cookie.DataCandidaturaAl, cookie.Regione, cookie.Azienda, cookie.Struttura, cookie.Stato, cookie.CodiceFiscale, out totaleCandidature);
                return ViewRisultati((int)cookie.PaginaCorrente, candidature, totaleCandidature, cookie);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Esporta()
        {
            utente = (User)Session["user"];
            CookieRicercaCandidatura cookie = new CookieRicercaCandidatura().GetCookie(Request);

            bool? pubblico = string.IsNullOrEmpty(cookie.PubblicaPrivata) ? (bool?)null : (cookie.PubblicaPrivata == "1" ? true : false);

            var listautenti = repository.GetUtentiDaCensire(0, 0, utente.UserId, cookie.Cognome, cookie.Nome, cookie.Ruolo, pubblico, cookie.DataCandidaturaDal, cookie.DataCandidaturaAl, cookie.Regione, cookie.Azienda, cookie.Struttura, cookie.Stato, cookie.CodiceFiscale, out int totale);


            if (listautenti != null && listautenti.Count > 0)
            {
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                MemoryStream memoryStream = new MemoryStream();
                TextWriter tw = new StreamWriter(memoryStream, encoding);
                /* intestazione csv */
                string csv = "Regione;Azienda;Struttura;Ruolo;Cognome;Nome;Email;Codice fiscale;Pubblico;Stato";
                tw.WriteLine(csv);

                /*Contenuti*/
                foreach (UtenteDaCensire riga in listautenti)
                {
                    csv = "";
                    csv += $"{riga.NomeRegione};{riga.NomeAzienda};{riga.NomeStruttura};{((Ruolo)riga.IdRuolo).ToString()};{riga.Cognome};{riga.Nome};{riga.Email};{riga.CodiceFiscale};{riga.Pubblico};{riga.NomeStato}";

                    tw.WriteLine(csv);
                }
                tw.Flush();
                tw.Close();

                return File(memoryStream.GetBuffer(), System.Net.Mime.MediaTypeNames.Application.Octet, "ElencoCandidature.csv");
            }

            throw new ErroreInterno("Non ci sono dati da esportare");
        }
    }
}