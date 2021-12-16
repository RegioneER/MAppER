using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mapper.Models;
using Mapper.Models.Repository;
using Mapper.Authentication;

namespace Mapper.Controllers
{
    public class OsservazioneController : BaseController
    {
        private MapperRepository repository = MapperRepository.GetMapperRepository();
        private User utente = new User();
        public enum TipoAzione
        {
            Create, Edit, Delete, DeleteOpportunita
        }

        public ActionResult Create(int IDScheda, int? IDOsservazione)
        {
            return ViewOsservazione("Create", IDScheda, IDOsservazione);
        }
        public ActionResult Edit(int IDScheda, int? IDOsservazione, int? Type)
        {
            if (!IDOsservazione.HasValue)
                return RedirectToAction("Create");
            else
            {
                if (Type.HasValue)
                {
                    switch (Type)
                    {
                        case (int)TipoAzione.DeleteOpportunita:
                            TempData["Alert"] = new Alert { Title = "Opportunità eliminata con successo", AlertType = Alert.AlertTypeEnum.Success };
                            break;
                    }
                }

                return ViewOsservazione("Edit", IDScheda, IDOsservazione);
            }
        }

        private ViewResult ViewOsservazione(string viewName, int IDScheda, int? IDOsservazione)
        {
            utente = (User)Session["user"];
            Scheda scheda = repository.GetScheda(IDScheda);
            Osservazione osservazione = null;

            if (IDOsservazione.HasValue)
            {
                osservazione = repository.GetOsservazione(IDOsservazione.Value);
                if (osservazione == null)
                    throw new ErroreInterno("Osservazione non trovata");
                osservazione.Opportunita = repository.GetOpportunitaByOsservazione(IDOsservazione.Value);
                ViewBag.Indicazioni = repository.GetIndicazioni();
                ViewBag.TabellaGrafico = repository.GetTabellaOsservazioni(null, IDOsservazione.Value);
            }
            else
            {
                osservazione = new Osservazione();
                osservazione.id = 0;
                osservazione.idScheda = IDScheda;
                osservazione.numOperatori = 0;
                osservazione.idOperatore = 0;
                osservazione.operatoreEsterno = false;
            }
            osservazione.Scheda = repository.GetScheda(IDScheda);

            SetViewBag(scheda.id, scheda.idStatoSessione, osservazione);

            repository.UpdatePosizione(utente.Username, Request.RawUrl, null);
            return View(viewName, osservazione);
        }
        private void SetViewBag(int idScheda, int idStatoSessione, Osservazione osservazione)
        {
            SelectList listOperatori = null;
            int? idOperatore = osservazione == null ? (int?)null : osservazione.idOperatore;
            listOperatori = new SelectList(repository.GetOperatori(), nameof(Operatore.id), nameof(Operatore.nomeCategoria), idOperatore);

            ViewBag.id = osservazione.id;
            ViewBag.Operatore = listOperatori;
            ViewBag.Esterno = osservazione != null && osservazione.operatoreEsterno.HasValue ? "on" : "off";
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idScheda,idOperatore,numOperatori")] Osservazione osservazione, string Esterno)
        {
            return Salva(osservazione, Esterno, ModelState);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idScheda,idOperatore,numOperatori")] Osservazione osservazione, string Esterno)
        {
            return Salva(osservazione, Esterno, ModelState);
        }

        public ActionResult Salva(Osservazione osservazione, string Esterno, ModelStateDictionary modelState)
        {
            utente = (User)Session["user"];
            bool ckEsterno = Esterno == "on";
            if (ModelState.IsValid)
            {
                Scheda scheda = repository.GetScheda(osservazione.idScheda);
                if ((osservazione.id == 0 && scheda.Osservazione.Any(x => x.idOperatore == osservazione.idOperatore && x.operatoreEsterno == ckEsterno)) || (osservazione.id > 0 && scheda.Osservazione.Any(x => x.id != osservazione.id && x.idOperatore == osservazione.idOperatore && x.operatoreEsterno == ckEsterno)))
                {
                    Alert alert = new Alert();
                    alert.AlertType = Alert.AlertTypeEnum.Error;
                    alert.Title = "ATTENZIONE! Sono presenti degli errori! Nessun dato è stato salvato";
                    alert.Messages.Add("Operatore già presente nella scheda. Se necessario aprire il dettaglio e modificare la quantità");
                    TempData["Alert"] = alert;
                }
                else
                {
                    Osservazione osservazioneDB = null;
                    if (osservazione.id > 0)
                        osservazioneDB = repository.GetOsservazione(osservazione.id);
                    else
                        osservazioneDB = new Osservazione()
                        {
                            idScheda = osservazione.idScheda,
                            idOperatore = osservazione.idOperatore,
                        };

                    repository.Modified(osservazioneDB);

                    osservazioneDB.data = DateTime.Now;
                    osservazioneDB.operatoreEsterno = ckEsterno;
                    osservazioneDB.numOperatori = osservazione.numOperatori;
                    repository.SaveOsservazione(osservazioneDB);
                    repository.InsertUpdateLog(utente.Username, "Salva osservazione id=" + osservazioneDB.id + "di scheda id=" + osservazioneDB.idScheda, Request.Url.PathAndQuery);
                    TempData["Alert"] = new Alert { Title = "Salvataggio eseguito con successo", AlertType = Alert.AlertTypeEnum.Success };
                }
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

            return RedirectToAction("Edit", "Scheda", new { IDScheda = osservazione.idScheda });
            //return RedirectToAction("Edit", new { IDScheda = osservazione.idScheda, IDOsservazione = (int)osservazione.id });
            //return ViewOsservazione("Edit",osservazione.idScheda, (int)osservazione.id);
        }

        [HttpPost]
        public ActionResult Delete(int IDScheda, int IDOsservazione)
        {
            utente = (User)Session["user"];
            try
            {
                repository.DeleteOsservazioneOpportunita(IDOsservazione);
                repository.InsertUpdateLog(utente.Username, "Elimina osservazione id=" + IDOsservazione + "di scheda id=" + IDScheda, Request.Url.PathAndQuery);
                return Json(new { newurl = Url.Action("Edit", "Scheda", new { IDScheda = IDScheda, Type = (int?)Mapper.Controllers.SchedaController.TipoAzione.DeleteOsservazione }) });
            }
            catch (Exception ex)
            {
                throw new ErroreInterno(String.Format("Errore elimina osservazione: {0}", ex.Message));
            }
        }

        [HttpPost]
        public JsonResult CreateChart(int idOsservazione)
        {
            List<object> iData = new List<object>();

            List<string> listaIndicazioni = (from t in repository.GetIndicazioni()
                                             select t.tipologia).ToList();

            List<fn_TabellaAdesioni_Result> result = (List<fn_TabellaAdesioni_Result>)repository.GetTabellaAdesioni(null, idOsservazione);
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