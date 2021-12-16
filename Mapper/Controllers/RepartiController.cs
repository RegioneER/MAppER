using Mapper.Authentication;
using Mapper.Models;
using Mapper.Models.Cookies;
using Mapper.Models.Repository;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mapper.Controllers
{
    public class RepartiController : BaseController
    {
        // GET: Reparti
        private MapperRepository repository = MapperRepository.GetMapperRepository();
        private static User utente = new User();
        private int repartiPerPagina = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["RepartiPerPagina"].ToString());

        public ActionResult Index()
        {
            utente = (User)Session["user"];
            SetViewBag(null);
            repository.UpdatePosizione(utente.Username, Request.RawUrl, null);
            return View();
        }

        public ActionResult Cerca()
        {
            return RedirectToAction("BackSearch");
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Cerca(string codRegione, string codAzienda, string codTipoStruttura, string codStruttura, string keyReparto, bool? Cancellate, int? StatoSessione)
        {
            utente = (User)Session["user"];
            CookieRicercaReparto cookieR = new CookieRicercaReparto()
            {
                Regione = codRegione,
                Azienda = codAzienda,
                TipoStruttura = codTipoStruttura,
                Struttura = codStruttura,
                Reparto = keyReparto,
                Cancellate = Cancellate,
                StatoSessione = StatoSessione,
                PaginaCorrente = 1
            };
            cookieR.CreateCookie(Response, Request);

            codAzienda = codAzienda.Split('.')[0];
            int totaleReparti = 0;
            var reparti = repository.GetReparti(0, repartiPerPagina, utente.UserId, codRegione, codAzienda, codTipoStruttura, codStruttura, keyReparto, Cancellate, out totaleReparti);
            return ViewRisultati(1, reparti, totaleReparti, cookieR);
        }
        public ActionResult ChangePage(int page)
        {
            utente = (User)Session["user"];
            CookieRicercaReparto cookie = new CookieRicercaReparto().GetCookie(Request);
            cookie.PaginaCorrente = page;
            cookie.CreateCookie(Response, Request);
            string codAzienda = cookie.Azienda.Split('.')?[0];
            int totaleReparti = 0;
            List<vwReparto> reparti = repository.GetReparti(((int)page - 1) * repartiPerPagina, repartiPerPagina, utente.UserId, cookie.Regione, codAzienda, cookie.TipoStruttura, cookie.Struttura, cookie.Reparto, cookie.Cancellate, out totaleReparti);
            return ViewRisultati((int)page, reparti, totaleReparti, cookie);
        }

        private ViewResult ViewRisultati(int page, List<vwReparto> reparti, int totaleSchede, CookieRicercaReparto cookie)
        {
            utente = (User)Session["user"];

            SetViewBag(cookie);

            vwRepartoManager RepartoManager = new vwRepartoManager
            {
                TotaleReparti = totaleSchede,
                RepartiPerPagina = repartiPerPagina,
                CurrentPage = page,
                RepartiPaged = new StaticPagedList<vwReparto>(reparti, page, repartiPerPagina, totaleSchede)
            };

            ViewBag.CurrentPage = page;
            repository.UpdatePosizione(utente.Username, Request.RawUrl.Replace("/Cerca", "/BackSearch"), JsonConvert.SerializeObject(cookie));
            return View("Index", RepartoManager);
        }

        public ActionResult BackSearch()
        {
            utente = (User)Session["user"];
            int idUtente = utente.UserId;
            CookieRicercaReparto cookie = new CookieRicercaReparto().GetCookie(Request);
            if (cookie != null)
            {
                string codAzienda = cookie.Azienda.Split('.')?[0];
                int totaleReparti = 0;
                List<vwReparto> reparti = repository.GetReparti(((int)cookie.PaginaCorrente - 1) * repartiPerPagina, repartiPerPagina, utente.UserId, cookie.Regione, codAzienda, cookie.TipoStruttura, cookie.Struttura, cookie.Reparto, cookie.Cancellate, out totaleReparti);
                return ViewRisultati((int)cookie.PaginaCorrente, reparti, totaleReparti, cookie);
            }
            return RedirectToAction("Index");
        }

        public void SetViewBag(CookieRicercaReparto cookie)
        {
            string codRegione = null;
            string keyAzienda = null;
            string codAzienda = null;
            string codTipoStruttura = null;
            string codStruttura = null;
            string keyReparto = null;
            int? statoSessione = null;
            bool? cancellate = null;
            string pubblicaPrivata = null;

            List<vwRegione> listaRegioni = null;
            List<vwAzienda> listaAziende = null;
            List<TipologiaStruttura> listaTipiStruttura = null;
            List<vwStruttura> listaStrutture = null;
            List<vwReparto> listaReparti = null;

            utente = (User)Session["user"];
            listaTipiStruttura = repository.GetTipiStruttureAttive();

            if (cookie != null)
            {
                codRegione = cookie.Regione;
                keyAzienda = cookie.Azienda;
                codAzienda = cookie.Azienda.Split('.')?[0];
                codTipoStruttura = cookie.TipoStruttura;
                codStruttura = cookie.Struttura;
                keyReparto = cookie.Reparto;
                statoSessione = cookie.StatoSessione;
                cancellate = cookie.Cancellate;
                pubblicaPrivata = cookie.PubblicaPrivata;
            }
            else
            {
                codRegione = utente.UtenteStrutture[0].codRegione;
                keyAzienda = utente.UtenteStrutture[0].KeyAzienda;
                codAzienda = utente.UtenteStrutture[0].codAzienda;
                codTipoStruttura = utente.UtenteStrutture[0].CodiceTipologiaStruttura;
                codStruttura = utente.UtenteStrutture[0].CodiceStruttura;
                keyReparto = utente.UtenteStrutture[0].KeyReparto;
                cancellate = false;
            }

            listaRegioni = repository.GetRegioniUtente(utente.Username);
            listaAziende = repository.GetAziende(codRegione, false, null);
            listaStrutture = string.IsNullOrEmpty(codAzienda) ? new List<vwStruttura>() : repository.GetStruttureBuff(codRegione, codAzienda, codTipoStruttura, false, pubblicaPrivata, null, utente.Ruolo == Ruolo.Aziendale);
            listaReparti = string.IsNullOrEmpty(codStruttura) ? new List<vwReparto>() : repository.GetReparti(codRegione, codAzienda, codStruttura, false, false);

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
            if (utente.Ruolo == Ruolo.Osservatore)
            {
                listaReparti = (from r in listaReparti
                                from us in utente.UtenteStrutture
                                where r.CodRegione == us.codRegione && r.CodAzienda == us.codAzienda && r.IdStrutturaErogatrice == us.idStrutturaErogatrice && r.idWebServiceStruttura == us.idWebServiceStruttura && r.IdReparto == us.idReparto && r.IdWebServiceReparto == us.idWebServiceReparto
                                select r).Distinct().ToList();
            }

            ViewBag.Regione = new SelectList(listaRegioni, "CodRegione", "Denominazione", codRegione);
            ViewBag.Azienda = new SelectList(listaAziende, "KeyAzienda", "Denominazione", keyAzienda);
            ViewBag.TipoStruttura = new SelectList(listaTipiStruttura, "CodTipologia", "DescrizioneForDisplay", codTipoStruttura);
            ViewBag.Struttura = new SelectList(listaStrutture, "KeyStruttura", "Denominazione", codStruttura);
            ViewBag.Reparto = new SelectList(listaReparti, "KeyReparto", "NomeCompleto", keyReparto);
            ViewBag.EnableRegione = listaRegioni.Count == 1 ? "disabled" : "";
            ViewBag.EnableAzienda = listaAziende.Count == 1 ? "disabled" : "";
            ViewBag.EnableStruttura = listaStrutture.Count == 1 ? "disabled" : "";

            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Text = "No", Value = "false", Selected = cancellate == false });
            selectList.Add(new SelectListItem { Text = "Sì", Value = "true", Selected = cancellate == true });
            ViewBag.Cancellate = selectList;
            var selectPubblicaPrivata = new List<SelectListItem>();
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Pubbliche e private", Value = "", Selected = string.IsNullOrEmpty(pubblicaPrivata) });
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Solo pubbliche", Value = "1", Selected = pubblicaPrivata == "1" });
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Solo private", Value = "2", Selected = pubblicaPrivata == "2" });
            ViewBag.PubblicaPrivata = selectPubblicaPrivata;

            ViewBag.IdUtente = utente.UserId.ToString();  //utente.Ruolo == Ruolo.Osservatore ? utente.idUtente.ToString() : null;
        }
    }
}