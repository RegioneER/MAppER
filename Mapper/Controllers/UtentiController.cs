using Mapper.Models;
using Mapper.Models.Cookies;
using Mapper.Models.Repository;
using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Mapper.Authentication;
using Newtonsoft.Json;

namespace Mapper.Controllers
{
    public class UtentiController : BaseController

    {
        private MapperRepository repository = MapperRepository.GetMapperRepository();
        private User utente = new User();
        private int utentiPerPagina = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["UtentiPerPagina"].ToString());

        // GET: Utenti
        public ActionResult Index()
        {
            utente = (User)Session["user"];

            SetViewBag(null);

            repository.UpdatePosizione(utente.Username, Request.RawUrl, null);

            return View();
        }

        public ActionResult Cerca(string cognome, string nome, int? idRuolo, bool? Cancellate, string codRegione, string codAzienda, string codTipoStruttura, string codStruttura, string keyReparto, string PubblicaPrivata)
        {
            utente = (User)Session["user"];
            int totale = 0;
            CookieRicercaUtente cookie = new CookieRicercaUtente()
            {
                Regione = codRegione,
                Azienda = codAzienda,
                TipoStruttura = codTipoStruttura,
                Struttura = codStruttura,
                Reparto = keyReparto,
                Nome = nome,
                Cognome = cognome,
                Cancellate = Cancellate,
                Ruolo = idRuolo,
                PaginaCorrente = 1,
                PubblicaPrivata = PubblicaPrivata
            };
            cookie.CreateCookie(Response, Request);

            var listautenti = repository.GetUtenti(0, utentiPerPagina,utente.UserId, utente.Ruolo, cognome, nome, idRuolo, Cancellate, codRegione, codAzienda, codTipoStruttura, codStruttura, keyReparto, out totale);

            return ViewRisultati(1, listautenti, totale, cookie);
        }

        private ViewResult ViewRisultati(int page, List<Utente> utenti, int totaleUtenti, CookieRicercaUtente cookie)
        {
            utente = (User)Session["user"];

            SetViewBag(cookie);

            UtenteManager UtenteManager = new UtenteManager
            {
                TotaleUtenti = totaleUtenti,
                UtentiPerPagina = utentiPerPagina,
                CurrentPage = page,
                UtentiPaged = new StaticPagedList<Utente>(utenti, page, utentiPerPagina, totaleUtenti)
            };

            ViewBag.CurrentPage = page;
            repository.UpdatePosizione(utente.Username, Request.RawUrl.Replace("/Search", "/BackSearch"), JsonConvert.SerializeObject(cookie));
            return View("Index", UtenteManager);
        }

        public void SetViewBag(CookieRicercaUtente cookie)
        {
            string keyReparto = null;
            bool? cancellate = null;
            string nome, cognome;
            string codRegione = null;
            string codAzienda = null;
            string codTipoStruttura = null;
            string codStruttura = null;
            string pubblicaPrivata = null;
            int? idRuolo = null;

            List<vwRegione> listaRegioni = null;
            List<vwAzienda> listaAziende = null;
            List<TipologiaStruttura> listaTipiStruttura = null;
            List<vwStruttura> listaStrutture = null;
            List<vwReparto> listaReparti = null;

            listaTipiStruttura = repository.GetTipiStruttureAttive();

            if (cookie != null)
            {
                codRegione = cookie.Regione;
                codAzienda = cookie.Azienda;
                codTipoStruttura = cookie.TipoStruttura;
                codStruttura = cookie.Struttura;
                keyReparto = cookie.Reparto;
                nome = cookie.Nome;
                cancellate = cookie.Cancellate;
                cognome = cookie.Cognome;
                idRuolo = cookie.Ruolo;
                pubblicaPrivata = cookie.PubblicaPrivata;
            }
            else
            {
                codRegione = utente.UtenteStrutture[0].codRegione;
                codAzienda = utente.UtenteStrutture[0].codAzienda;
                codTipoStruttura = utente.UtenteStrutture[0].CodiceTipologiaStruttura;
                codStruttura = utente.UtenteStrutture[0].CodiceStruttura;
                keyReparto = utente.UtenteStrutture[0].KeyReparto;
            }

            listaRegioni = repository.GetRegioniUtente(utente.Username);
            listaAziende = repository.GetAziende(codRegione, false, null);
            listaStrutture = string.IsNullOrEmpty(codAzienda) ? new List<vwStruttura>() : repository.GetStruttureBuff(codRegione, codAzienda, codTipoStruttura, false, pubblicaPrivata, null, utente.Ruolo == Ruolo.Aziendale);
            listaReparti = string.IsNullOrEmpty(codStruttura) ? new List<vwReparto>() : repository.GetReparti(codRegione, codAzienda, codStruttura,false,false);


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
            ViewBag.Reparto = new SelectList(listaReparti, "KeyReparto", "Nome", keyReparto);
            ViewBag.EnableRegione = listaRegioni.Count == 1 ? "disabled" : "";
            ViewBag.EnableAzienda = listaAziende.Count == 1 ? "disabled" : "";
            ViewBag.EnableStruttura = listaStrutture.Count == 1 ? "disabled" : "";

            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Text = "No", Value = "false", Selected = cancellate != true });
            selectList.Add(new SelectListItem { Text = "Sì", Value = "true", Selected = cancellate == true });
            ViewBag.Cancellate = selectList;

            var selectPubblicaPrivata = new List<SelectListItem>();
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Pubbliche e private", Value = "", Selected = string.IsNullOrEmpty(pubblicaPrivata) });
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Solo pubbliche", Value = "1", Selected = pubblicaPrivata == "1" });
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Solo private", Value = "2", Selected = pubblicaPrivata == "2" });
            ViewBag.PubblicaPrivata = selectPubblicaPrivata;

            ViewBag.idRuolo = new SelectList(repository.GetRuoli((int)utente.Ruolo), "id", "nome", idRuolo);
        }

        public ActionResult BackSearch()
        {
            utente = (User)Session["user"];
            int idUtente = utente.UserId;
            CookieRicercaUtente cookie = new CookieRicercaUtente().GetCookie(Request);
            if (cookie != null)
            {
                int totaleUtenti = 0;
                List<Utente> utenti = repository.GetUtenti(((int)cookie.PaginaCorrente - 1) * utentiPerPagina, utentiPerPagina, utente.UserId, utente.Ruolo, cookie.Cognome, cookie.Nome, cookie.Ruolo, cookie.Cancellate, cookie.Regione, cookie.Azienda, cookie.TipoStruttura, cookie.Struttura, cookie.Reparto, out totaleUtenti);
                return ViewRisultati((int)cookie.PaginaCorrente, utenti, totaleUtenti, cookie);
            }
            return RedirectToAction("Index");
        }
        public ActionResult CambiaPagina(int? page)
        {
            utente = (User)Session["user"];
            CookieRicercaUtente cookie = new CookieRicercaUtente().GetCookie(Request);

            int totaleUtenti = 0;
            List<Utente> utenti = repository.GetUtenti(((int)page - 1) * utentiPerPagina, utentiPerPagina, utente.UserId, utente.Ruolo, cookie.Cognome, cookie.Nome, cookie.Ruolo, cookie.Cancellate, cookie.Regione, cookie.Azienda, cookie.TipoStruttura, cookie.Struttura, cookie.Reparto, out totaleUtenti);
            return ViewRisultati((int)page, utenti, totaleUtenti, cookie);
        }
    }
}