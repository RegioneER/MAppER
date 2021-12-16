using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Schema;
using Mapper.Models;
using Mapper.Models.Repository;
using Newtonsoft.Json;
using Mapper.Authentication;

namespace Mapper.Controllers
{
    public class SchedaController : BaseController
    {
        private int GiorniSbloccaSchedaRegionale = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["GiorniSbloccaSchedaRegionale"].ToString());
        private int GiorniSbloccaSchedaAziendale = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["GiorniSbloccaSchedaAziendale"].ToString());
        private MapperRepository repository = MapperRepository.GetMapperRepository();
        private User utente = new User();

        public enum TipoAzione
        {
            Create, Edit, Delete, Consolidate, DeleteOsservazione, Unlock
        }

        public ActionResult Create(int? IDScheda)
        {
            return ViewScheda("Create", IDScheda);
        }
        public ActionResult Edit(int? IDScheda, int? Type)
        {
            if (!IDScheda.HasValue)
                return RedirectToAction("Create");
            else
            {
                if (Type.HasValue)
                {
                    switch (Type)
                    {
                        case (int)TipoAzione.Delete:
                            TempData["Alert"] = new Alert { Title = "Scheda eliminata con successo", AlertType = Alert.AlertTypeEnum.Success };
                            break;

                        case (int)TipoAzione.Consolidate:
                            TempData["Alert"] = new Alert { Title = "Scheda consolidata con successo", AlertType = Alert.AlertTypeEnum.Success };
                            break;

                        case (int)TipoAzione.Unlock:
                            TempData["Alert"] = new Alert { Title = "Scheda sbloccata con successo", AlertType = Alert.AlertTypeEnum.Success };
                            break;

                        case (int)TipoAzione.DeleteOsservazione:
                            TempData["Alert"] = new Alert { Title = "Osservazione eliminata con successo", AlertType = Alert.AlertTypeEnum.Success };
                            break;
                    }
                }

                return ViewScheda("Edit", IDScheda);
            }
        }

        private ViewResult ViewScheda(string viewName, int? IDScheda)
        {
            utente = (User)Session["user"];
            Scheda scheda = null;

            if (IDScheda.HasValue)
            {
                scheda = repository.GetScheda(IDScheda.Value);
                if (scheda == null)
                    throw new ErroreInterno("Scheda non presente");

                scheda.DataSchedaForDisplay = scheda.data.ToShortDateString();
                scheda.TimeSchedaForDisplay = scheda.data.ToShortTimeString();
            }
            else
            {
                scheda = new Scheda()
                {
                    id = 0,
                    data = DateTime.Now,
                    dataInserimento = DateTime.Now,
                    durataSessione = 0,
                    idReparto = 0,
                    idUtente = utente.UserId,
                    idStatoSessione = (int)StatoSessione.Stato.InLavorazione,
                    DataScheda = DateTime.Now.ToShortDateString(),
                    TimeScheda = DateTime.Now.ToShortTimeString(),
                    note = ""
                };
            }

            SetViewBag(scheda);
            repository.UpdatePosizione(utente.Username, Request.RawUrl, null);
            return View(viewName, scheda);
        }

        private void SetViewBag(Scheda scheda)
        {
            string codRegione = null;
            string codAzienda = null;
            string keyAzienda = null;
            string codTipoStruttura = null;
            string keyStrutturaData = null;
            string codStruttura = null;
            string keyReparto = scheda.idReparto > 0 ? scheda.Reparto.KeyReparto : null;
            bool mostraStorico = (scheda.id != 0);

            List<vwRegione> listaRegioni = null;
            List<vwAzienda> listaAziende = null;
            List<vwStruttura> listaStrutture = null;
            List<vwReparto> listaReparti = null;
            List<TipologiaStruttura> listaTipiStruttura = null;

            listaTipiStruttura = repository.GetTipiStruttureAttive();

            if (scheda.idReparto > 0)
            {
                vwReparto reparto = repository.GetReparto(scheda.idReparto, scheda.idWebServiceReparto);
                vwStruttura struttura = repository.GetStrutturaDataScheda(reparto.keyStruttura, scheda.data);
                codRegione = reparto.CodRegione;
                keyAzienda =  struttura.KeyAzienda;
                codAzienda = struttura.CodAzienda;
                                                  //codTipoStruttura = reparto.CodiceTipologiaStruttura;
                keyStrutturaData = struttura.KeyStrutturaData;
                codStruttura = struttura.KeyStruttura; 
            }
            else
            {
                codRegione = utente.UtenteStrutture[0].codRegione;
                keyAzienda = utente.UtenteStrutture[0].KeyAzienda;
                codAzienda = utente.UtenteStrutture[0].codAzienda;
                //codTipoStruttura = utente.UtenteStrutture[0].CodiceTipologiaStruttura;
                codStruttura = utente.UtenteStrutture[0].CodiceStruttura;
                keyStrutturaData = !string.IsNullOrEmpty(codStruttura) ? repository.GetStrutturaCorrente(codStruttura).KeyStrutturaData : "";

            }

            listaRegioni = repository.GetRegioniUtente(utente.Username);
            listaAziende = repository.GetAziende(codRegione, mostraStorico, null);
            listaStrutture = string.IsNullOrEmpty(codAzienda) ? new List<vwStruttura>() : repository.GetStruttureBuff(codRegione, codAzienda, codTipoStruttura, mostraStorico, null, scheda.data, utente.Ruolo == Ruolo.Aziendale);
            listaReparti = string.IsNullOrEmpty(codStruttura) ? new List<vwReparto>() : repository.GetReparti(codRegione, codAzienda, codStruttura, true,false);

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
            ViewBag.ddlReparto = new SelectList(listaReparti, "KeyReparto", "Nome", keyReparto);
            ViewBag.EnableRegione = listaRegioni.Count == 1 ? "disabled" : "";
            ViewBag.EnableAzienda = listaAziende.Count == 1 ? "disabled" : "";
            ViewBag.EnableStruttura = listaStrutture.Count == 1 ? "disabled" : "";
            ViewBag.EnableSbloccaScheda = scheda.idStatoSessione == (int)StatoSessione.Stato.Consolidata &&
                (
                    (utente.Ruolo == Ruolo.Aziendale && scheda.dataUltimaModificaStato.AddDays(GiorniSbloccaSchedaAziendale) >= DateTime.Now)
                    ||
                    (utente.Ruolo == Ruolo.Regionale && scheda.dataUltimaModificaStato.AddDays(GiorniSbloccaSchedaRegionale) >= DateTime.Now)
                );

            ViewBag.idScheda = scheda.id;
            ViewBag.keyReparto = scheda.idReparto > 0 ? scheda.Reparto.KeyReparto : null;
            ViewBag.idStatoSessione = scheda.idStatoSessione;
            ViewBag.DataOraInserimento = scheda.dataInserimento.ToString("dd/MM/yyyy HH:mm");
            ViewBag.DataInserimentoDate = scheda.dataInserimento.ToShortDateString();
            ViewBag.DataInserimentoTime = scheda.dataInserimento.ToShortTimeString();
            ViewBag.DataScheda = scheda.data.ToShortDateString();
            ViewBag.TimeScheda = scheda.data.ToShortTimeString();
            ViewBag.Durata = scheda.durataSessione;
            ViewBag.Cancellata = (scheda.idStatoSessione == (int)StatoSessione.Stato.Cancellata) ? "on" : "off";
            ViewBag.IdUtente = scheda.idUtente;

            var selectPubblicaPrivata = new List<SelectListItem>();
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Pubbliche e private", Value = "", Selected = true });
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Solo pubbliche", Value = "1" });
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Solo private", Value = "2" });
            ViewBag.PubblicaPrivata = selectPubblicaPrivata;

            if (scheda.id > 0)
            {
                ViewBag.Indicazioni = repository.GetIndicazioni();
                ViewBag.TabellaGrafico = repository.GetTabellaOsservazioni(scheda.id, null);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,KeyReparto,idStatoSessione,DataScheda,TimeScheda,durataSessione,idUtente,dataInserimento")] Scheda scheda, string Cancellata)
        {
            return Salva(scheda, Cancellata, ModelState);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,KeyReparto,idStatoSessione,DataScheda,TimeScheda,durataSessione,idUtente,dataInserimento")] Scheda scheda, string Cancellata)
        {
            return Salva(scheda, Cancellata, ModelState);
        }

        public ActionResult Salva(Scheda scheda, string cancellata, ModelStateDictionary modelState)
        {
            utente = (User)Session["user"];

            vwReparto reparto = repository.GetReparto(scheda.KeyReparto);
            scheda.idReparto = reparto.IdReparto;
            scheda.idWebServiceReparto = reparto.IdWebServiceReparto;
            if (modelState.IsValid)
            {

                repository.Modified(scheda);
                /* se lo statosessione è cancellato, ma il flag è stato tolto, riporto lo stato della scheda come parziale */
                scheda.idStatoSessione = (scheda.idStatoSessione == (int)StatoSessione.Stato.Cancellata && String.IsNullOrEmpty(cancellata) ? (int)StatoSessione.Stato.InLavorazione : scheda.idStatoSessione);
                scheda.note = "";

                if (scheda.DataScheda != null)
                {
                    scheda.data = DateTime.Parse(scheda.DataScheda + " " + scheda.TimeScheda, new CultureInfo("it-IT"));
                }
                /**/

                /* aggiorno la dataultimaModificaStato */
                if (scheda.id > 0)
                {
                    //Scheda oldScheda = repository.GetScheda(scheda.id);
                    //if (oldScheda.idStatoSessione != scheda.idStatoSessione)
                    scheda.dataUltimaModificaStato = DateTime.Now;
                }
                else
                    scheda.dataUltimaModificaStato = scheda.dataInserimento;
                /**/
                //scheda.Reparto = repository.GetReparto(scheda.idReparto);

                repository.SaveScheda(scheda);
                repository.InsertUpdateLog(utente.Username, "Salva scheda id=" + scheda.id, Request.Url.PathAndQuery);
                TempData["Alert"] = new Alert { Title = "Salvataggio eseguito con successo", AlertType = Alert.AlertTypeEnum.Success };
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

            return RedirectToAction("Edit", new { IDScheda = scheda.id });
        }

        [HttpPost]
        public ActionResult Consolidate(int IDScheda)
        {
            utente = (User)Session["user"];
            try
            {
                repository.SetStatoScheda(IDScheda, StatoSessione.Stato.Consolidata);
                repository.InsertUpdateLog(utente.Username, "Consolida scheda id=" + IDScheda, Request.Url.PathAndQuery);
                return Json(new { newurl = Url.Action("Edit", new { IDScheda = IDScheda, Type = (int?)TipoAzione.Consolidate }) });
            }
            catch (Exception ex)
            {
                throw new ErroreInterno(string.Format("Errore consolida scheda: {0}", ex.Message));
            }
        }

        [HttpPost]
        public ActionResult Sblocca(int IDScheda)
        {
            utente = (User)Session["user"];
            try
            {
                repository.SetStatoScheda(IDScheda, StatoSessione.Stato.InLavorazione);
                repository.InsertUpdateLog(utente.Username, "Sblocca scheda id=" + IDScheda, Request.Url.PathAndQuery);
                return Json(new { newurl = Url.Action("Edit", new { IDScheda = IDScheda, Type = (int?)TipoAzione.Unlock }) });
            }
            catch (Exception ex)
            {
                throw new ErroreInterno(string.Format("Errore consolida scheda: {0}", ex.Message));
            }
        }

        [HttpPost]
        public ActionResult Delete(int IDScheda)
        {
            utente = (User)Session["user"];
            try
            {
                repository.SetStatoScheda(IDScheda, StatoSessione.Stato.Cancellata);
                repository.InsertUpdateLog(utente.Username, "Cancella scheda id=" + IDScheda, Request.Url.PathAndQuery);
                return Json(new { newurl = Url.Action("Edit", new { IDScheda = IDScheda, Type = (int?)TipoAzione.Delete }) });
            }
            catch (Exception ex)
            {
                throw new ErroreInterno(string.Format("Errore cancella scheda: {0}", ex.Message));
            }
        }

        [HttpPost]
        public JsonResult CreateChart(int IDScheda)
        {
            List<object> iData = new List<object>();

            List<string> listaIndicazioni = (from t in repository.GetIndicazioni()
                                             select t.tipologia).ToList();

            List<fn_TabellaAdesioni_Result> result = (List<fn_TabellaAdesioni_Result>)repository.GetTabellaAdesioni(IDScheda, null);
            List<decimal?> listaAdesioni = (from t in result
                                            select t.PercAdesione).ToList();
            List<decimal?> listaNonAdesioni = (from t in result
                                               select t.PercNonAdesione).ToList();

            iData.Add(listaIndicazioni);
            iData.Add(listaAdesioni);
            iData.Add(listaNonAdesioni);

            return Json(iData, JsonRequestBehavior.AllowGet);
        }

    }
}