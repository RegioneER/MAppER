using Mapper.Models;
using Mapper.Models.Cookies;
using Mapper.Models.Repository;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mapper.Authentication;

namespace Mapper.Controllers
{

    public class SchedeController : BaseController
    {
        private MapperRepository repository = MapperRepository.GetMapperRepository();
        private static User utente = new User();
        private int schedePerPagina = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["SchedePerPagina"].ToString());
        private string urlDefault = System.Configuration.ConfigurationManager.AppSettings["UrlDefault"];

        // GET: Schede
        public ActionResult Index()
        {
            utente = (User)Session["user"];
            var request = HttpContext;
            if (utente != null)
            {
                SetViewBag(null);

                repository.UpdatePosizione(utente.Username, Request.RawUrl, null);

                return View();
            }
            return null;
        }

        public ActionResult Logout()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Abandon();
            utente = (User)Session["user"];
            return Redirect(urlDefault);
        }

        public ActionResult Start()
        {
            utente = (User)Session["user"];
            if (utente != null)
            {
                UltimaPosizione posizione = repository.GetUltimaPosizione(utente.Username);
                if (posizione != null)
                {
                    if (!string.IsNullOrEmpty(posizione.datiRicerca))
                    {
                        CookieRicerca cookie = JsonConvert.DeserializeObject<CookieRicerca>(posizione.datiRicerca);
                        cookie.CreateCookie(Response, Request);
                    }
                    string urlRedirect = urlDefault;
                    urlRedirect = string.Format("{0}{1}", urlRedirect, posizione.url);
#if (DEBUG)
                    urlRedirect = string.Format("http://localhost{0}", posizione.url);
#endif
                    return Redirect(urlRedirect);
                }

                return RedirectToAction("Index");
            }

            return null;
        }

        public ActionResult Search()
        {
            return RedirectToAction("BackSearch");
        }

        private ViewResult ViewRisultati(int page, List<Scheda> schede, int totaleSchede, CookieRicerca cookie)
        {
            utente = (User)Session["user"];

            SetViewBag(cookie);

            SchedaManager schedaManager = new SchedaManager
            {
                TotaleSchede = totaleSchede,
                SchedePerPagina = schedePerPagina,
                CurrentPage = page,
                SchedePaged = new StaticPagedList<Scheda>(schede, page, schedePerPagina, totaleSchede)
            };

            ViewBag.CurrentPage = page;

            repository.UpdatePosizione(utente.Username, Request.RawUrl.Replace("/Search", "/BackSearch"), JsonConvert.SerializeObject(cookie));
            return View("Index", schedaManager);
        }

        private void SetViewBag(CookieRicerca cookie)
        {
            string codRegione = null;
            string keyAzienda = null;
            string codAzienda = null;
            string codTipoStruttura = null;
            string keyStrutturaData = null;
            string codStruttura = null;
            string keyReparto = null;
            int? statoSessione = null;
            bool? cancellate = null;
            string pubblicaPrivata = null;
            bool mostraStorico = false;

            List<vwRegione> listaRegioni = null;
            List<vwAzienda> listaAziende = null;
            List<TipologiaStruttura> listaTipiStruttura = null;
            List<vwStruttura> listaStrutture = null;
            List<vwReparto> listaReparti = null;

            listaTipiStruttura = repository.GetTipiStruttureAttive();

            if (cookie != null)
            {
                codRegione = cookie.Regione;
                keyAzienda = cookie.Azienda;
                codAzienda = cookie.Azienda.Split('.')?[0];
                keyStrutturaData = cookie.Struttura;
                codStruttura = !string.IsNullOrEmpty(cookie.Struttura) ? $"{cookie.Struttura.Split('.')?[0]}.{cookie.Struttura.Split('.')?[1]}" : cookie.Struttura;
                codTipoStruttura = cookie.TipoStruttura;
                keyReparto = cookie.Reparto;
                statoSessione = cookie.StatoSessione;
                cancellate = cookie.Cancellate;
                pubblicaPrivata = cookie.PubblicaPrivata;
                mostraStorico = cookie.MostraStorico.HasValue ? cookie.MostraStorico.Value : false;

                ViewBag.DataDal = cookie.DataDal;
                ViewBag.DataAl = cookie.DataAl;
                ViewBag.idScheda = cookie.idScheda;
            }
            else
            {
                codRegione = utente.UtenteStrutture[0].codRegione;
                keyAzienda = utente.UtenteStrutture[0].KeyAzienda;
                codAzienda = utente.UtenteStrutture[0].codAzienda;
                codStruttura = utente.UtenteStrutture[0].CodiceStruttura;
                keyStrutturaData = !string.IsNullOrEmpty(codStruttura) ? repository.GetStrutturaCorrente(codStruttura).KeyStrutturaData: "";
                keyReparto = utente.UtenteStrutture[0].KeyReparto;
            }

            listaRegioni = repository.GetRegioniUtente(utente.Username);
            listaAziende = repository.GetAziende(codRegione, mostraStorico, null);
            listaStrutture = string.IsNullOrEmpty(codAzienda) ? new List<vwStruttura>() : repository.GetStruttureBuff(codRegione, codAzienda, codTipoStruttura, mostraStorico, pubblicaPrivata, null, utente.Ruolo == Ruolo.Aziendale);
            listaReparti = string.IsNullOrEmpty(codStruttura) ? new List<vwReparto>() : repository.GetReparti(codRegione, codAzienda, codStruttura, true, false);

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
            ViewBag.Struttura = new SelectList(listaStrutture, "KeyStrutturaData", "Denominazione", keyStrutturaData);
            ViewBag.Reparto = new SelectList(listaReparti, "KeyReparto", "Nome", keyReparto);
            ViewBag.EnableRegione = listaRegioni.Count == 1 ? "disabled" : "";
            ViewBag.EnableAzienda = listaAziende.Count == 1 ? "disabled" : "";
            ViewBag.EnableStruttura = ((utente.Ruolo == Ruolo.Osservatore || utente.Ruolo == Ruolo.ReferenteStruttura) && listaAziende.Count == 1 && listaStrutture.Count == 1) ? "disabled" : "";
            ViewBag.StatoSessione = new SelectList(repository.GetStatiSessione(), "id", "DescrizionePubblica", statoSessione);
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Text = "No", Value = "false", Selected = cancellate != true });
            selectList.Add(new SelectListItem { Text = "Sì", Value = "true", Selected = cancellate == true });
            ViewBag.Cancellate = selectList;

            var selectPubblicaPrivata = new List<SelectListItem>();
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Pubbliche e private", Value = "", Selected = string.IsNullOrEmpty(pubblicaPrivata) });
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Solo pubbliche", Value = "1", Selected = pubblicaPrivata == "1" });
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Solo private", Value = "2", Selected = pubblicaPrivata == "2" });
            ViewBag.PubblicaPrivata = selectPubblicaPrivata;
            ViewBag.MostraStorico = mostraStorico;
            ViewBag.IdUtente = utente.UserId.ToString();
        }

        public ActionResult ChangePage(int page)
        {
            CookieRicerca cookie = new CookieRicerca().GetCookie(Request);
            cookie.PaginaCorrente = page;
            cookie.CreateCookie(Response, Request);
            string codAzienda = cookie.Azienda.Split('.')?[0];
            string codStruttura = !string.IsNullOrEmpty(cookie.Struttura) ? $"{cookie.Struttura.Split('.')?[0]}.{cookie.Struttura.Split('.')?[1]}" : cookie.Struttura;
            int totaleSchede = 0;
            List<Scheda> schede = repository.GetSchede(((int)page - 1) * schedePerPagina, schedePerPagina, (int)cookie.IdUtente, cookie.Regione, codAzienda, cookie.TipoStruttura, codStruttura, cookie.Reparto, cookie.StatoSessione, cookie.DataDal, cookie.DataAl, cookie.Cancellate, cookie.idScheda, out totaleSchede);
            return ViewRisultati((int)page, schede, totaleSchede, cookie);
        }

        public ActionResult BackSearch()
        {
            utente = (User)Session["user"];
            int idUtente = utente.UserId;
            CookieRicerca cookie = new CookieRicerca().GetCookie(Request);
            if (cookie != null)
            {
                if (cookie.IdUtente.HasValue) idUtente = cookie.IdUtente.Value;
                int totaleSchede = 0;
                string codAzienda = cookie.Azienda.Split('.')?[0];
                string codStruttura = !string.IsNullOrEmpty(cookie.Struttura) ? $"{cookie.Struttura.Split('.')?[0]}.{cookie.Struttura.Split('.')?[1]}" : cookie.Struttura;

                List<Scheda> schede = repository.GetSchede(((int)cookie.PaginaCorrente - 1) * schedePerPagina, schedePerPagina, idUtente, cookie.Regione, codAzienda, cookie.TipoStruttura, codStruttura, cookie.Reparto, cookie.StatoSessione, cookie.DataDal, cookie.DataAl, cookie.Cancellate, cookie.idScheda, out totaleSchede);
                return ViewRisultati((int)cookie.PaginaCorrente, schede, totaleSchede, cookie);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// cerca i risultati dei filtri
        /// </summary>
        /// <param name="Reparto"></param>
        /// <param name="StatoSessione"></param>
        /// <param name="DataDal"></param>
        /// <param name="DataAl"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Search(int IdUtente, string codRegione, string codAzienda, string codTipoStruttura, string codStruttura, string keyReparto, int? StatoSessione, bool? Cancellate, DateTime? DataDal, DateTime? DataAl, int? idScheda, string ckStorico, string PubblicaPrivata)
        {
            CookieRicerca cookieR = new CookieRicerca()
            {
                IdUtente = IdUtente,
                Regione = codRegione,
                Azienda = codAzienda,
                Struttura = codStruttura,
                TipoStruttura = codTipoStruttura,
                Reparto = keyReparto,
                DataDal = DataDal,
                DataAl = DataAl,
                Cancellate = Cancellate,
                idScheda = idScheda,
                StatoSessione = StatoSessione,
                PaginaCorrente = 1,
                MostraStorico = ckStorico == "on" ? true : false,
                PubblicaPrivata = PubblicaPrivata
            };
            cookieR.CreateCookie(Response, Request);

            codAzienda = codAzienda.Split('.')?[0];
            if (!string.IsNullOrEmpty(codStruttura))
                codStruttura = $"{codStruttura.Split('.')[0]}.{codStruttura.Split('.')[1]}";

            int totaleSchede = 0;
            List<Scheda> schede = repository.GetSchede(0, schedePerPagina, IdUtente, codRegione, codAzienda, codTipoStruttura, codStruttura, keyReparto, StatoSessione, DataDal, DataAl, Cancellate, idScheda, out totaleSchede);
            return ViewRisultati(1, schede, totaleSchede, cookieR);
        }

        public JsonResult GetAziende(string codRegione, bool mostraCessate, string dataScheda)
        {
            utente = (User)Session["user"];
            DateTime? dataRiferimento = DateTime.TryParse(dataScheda, out DateTime dt) ? dt : (DateTime?)null;
            List<vwAzienda> listaAziende = repository.GetAziende(codRegione, mostraCessate, dataRiferimento);

            if (utente.Ruolo == Ruolo.Aziendale || utente.Ruolo == Ruolo.ReferenteStruttura || utente.Ruolo == Ruolo.Osservatore)
            {
                listaAziende = (from a in listaAziende
                                from us in utente.UtenteStrutture
                                where a.CodRegione == us.codRegione && a.CodAzienda == us.codAzienda
                                select a).ToList();
            }

            var result = (from azienda in listaAziende
                          select new
                          {
                              codice = azienda.KeyAzienda,
                              id = azienda.CodAzienda,
                              nome = azienda.Denominazione
                          }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStrutture(string codRegione, string codAzienda, string codTipoStruttura, bool mostraCessate, string pubblicaPrivata, string dataScheda, bool forStorico)
        {
            DateTime? dataRiferimento = DateTime.TryParse(dataScheda, out DateTime dt) ? dt : (DateTime?)null;
            List<vwStruttura> listaStrutture = repository.GetStruttureBuff(codRegione, codAzienda.Split('.')?[0], codTipoStruttura, mostraCessate, pubblicaPrivata, dataRiferimento, utente.Ruolo == Ruolo.Aziendale);

            if (utente.Ruolo == Ruolo.ReferenteStruttura || utente.Ruolo == Ruolo.Osservatore)
            {
                listaStrutture = (from s in listaStrutture
                                  from us in utente.UtenteStrutture
                                  where s.CodRegione == us.codRegione && s.CodAzienda == us.codAzienda && s.IdStrutturaErogatrice == us.idStrutturaErogatrice && s.idWebServiceStruttura == us.idWebServiceStruttura
                                  select s).ToList();
            }

            var result = (from struttura in listaStrutture
                          select new
                          {
                              codice = forStorico ? struttura.KeyStrutturaData : struttura.KeyStruttura,
                              nome = struttura.Denominazione
                          }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReparti(string codRegione, string codAzienda, string codTipoStruttura, string codStruttura, bool forStorico, bool mostraTutti)
        {
            List<vwReparto> listaReparti = null;
            codAzienda = codAzienda.Split('.')[0];
            codStruttura = !string.IsNullOrEmpty(codStruttura) ? $"{codStruttura.Split('.')?[0]}.{codStruttura.Split('.')?[1]}" : codStruttura;

            if (String.IsNullOrEmpty(codStruttura))
            {
                if (!string.IsNullOrEmpty(codTipoStruttura))
                {
                    TipologiaStruttura tipologia = repository.GetTipiStruttureAttive().FirstOrDefault(x => x.CodTipologia == codTipoStruttura);
                    listaReparti = repository.GetRepartiByTipoStruttura(codRegione, codAzienda, codTipoStruttura, tipologia.CodAreaDisciplina, mostraTutti);
                }
                else
                    listaReparti = new List<vwReparto>();
            }
            else
                listaReparti = repository.GetReparti(codRegione, codAzienda, codStruttura, forStorico, mostraTutti);

            if (utente.Ruolo == Ruolo.Osservatore)
            {
                listaReparti = (from r in listaReparti
                                from us in utente.UtenteStrutture.Where(x => x.codRegione == r.CodRegione && x.codAzienda == r.CodAzienda && x.idStrutturaErogatrice == r.IdStrutturaErogatrice && x.idWebServiceStruttura == r.idWebServiceStruttura && x.idReparto == r.IdReparto && x.idWebServiceReparto == r.IdWebServiceReparto)
                                select r).Distinct().ToList();
            }

            var result = from reparto in listaReparti
                         select new
                         {
                             codice = reparto.KeyReparto,
                             nome = reparto.NomeCompleto
                         };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Esporta(int IdUtente, string codRegione, string codAzienda, string codTipoStruttura, string codStruttura, string keyReparto, int? StatoSessione, bool? Cancellate, DateTime? DataDal, DateTime? DataAl, int? idScheda)
        {
            utente = (User)Session["user"];
            codRegione = string.IsNullOrEmpty(codRegione) ? null : codRegione;
            codAzienda = string.IsNullOrEmpty(codAzienda) ? null : codAzienda.Split('.')?[0];
            codTipoStruttura = string.IsNullOrEmpty(codTipoStruttura) ? null : codTipoStruttura;
            codStruttura = string.IsNullOrEmpty(codStruttura) ? null : $"{codStruttura.Split('.')[0]}.{codStruttura.Split('.')[1]}";
            keyReparto = string.IsNullOrEmpty(keyReparto) ? null : keyReparto;

            List<EsportaOpportunita_Result> righe = repository.GetEsportaOpportunita(IdUtente, codRegione, codAzienda, codTipoStruttura, codStruttura, keyReparto, StatoSessione, Cancellate, DataDal, DataAl, idScheda);

            if (righe != null && righe.Count > 0)
            {
                MemoryStream memoryStream = CreaFile(righe);
                return File(memoryStream.GetBuffer(), System.Net.Mime.MediaTypeNames.Application.Octet, "ElencoOpportunita.csv");
            }

            throw new ErroreInterno("Non ci sono dati da esportare");
        }

        public MemoryStream CreaFile(List<EsportaOpportunita_Result> righe)
        {
            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream, encoding);
            /* intestazione csv */
            string csv = "Id scheda;DataOra scheda;DataOra inserimento;Codice Regione;Nome Regione;Codice azienda;Nome azienda;Codice tipologia struttura;"
                    + "Nome tipologia struttura;Codice ministeriale struttura;Nome struttura;Indirizzo struttura;Nome area disciplina;Codice disciplina;"
                    + "Nome discipina;Nome Reparto;Stato sessione scheda;Durata sessione Scheda;Numero operatori;Codice categoria operatore;Nome categoria operatore;"
                    + "Classe operatore;Operatore esterno;DataOra opportunità;Codice tipologia azione;Tipologia azione;Codice tipologia indicazione;Tipologia indicazione;"
                    + "IdUtente;Ruolo utente";

            tw.WriteLine(csv);

            /*Contenuti*/
            foreach (EsportaOpportunita_Result riga in righe)
            {
                csv = "";
                csv += String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21};{22};{23};{24};{25};{26};{27};{28};{29}",
                        riga.IdScheda, riga.dataScheda.ToString(), riga.dataInserimentoScheda.ToString(), riga.codiceRegione, riga.nomeRegione, riga.codiceAzienda, riga.nomeAzienda,
                        riga.CodiceTipologiaStruttura, riga.nomeTipologiaStruttura, riga.codMinStruttura, riga.nomeStruttura, riga.indirizzoStruttura, riga.nomeDisciplinaArea,
                        riga.codiceDisciplina, riga.nomeDisciplina, riga.nomeReparto, riga.statoSessioneScheda, riga.durataSessione, riga.numeroOperatori, riga.codiceCategoriaOperatori,
                        riga.nomeCategoriaOperatore, riga.classeOperatore, riga.operatoreEsterno, riga.dataOraOpportunità, riga.codiceTipologiaAzione, riga.tipologiaAzione,
                        riga.codiceTipologiaIndicazione, riga.TipologiaIndicazione, riga.idUtente, riga.ruoloUtente);

                tw.WriteLine(csv);
            }
            tw.Flush();
            tw.Close();

            return memoryStream;
        }




    }
}