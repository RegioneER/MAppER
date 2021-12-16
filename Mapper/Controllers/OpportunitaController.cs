using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mapper.Controllers;
using Mapper.Models;
using Mapper.Models.Repository;
using Mapper.Authentication;

namespace Mapper.Views
{
    public class OpportunitaController : BaseController
    {
        private MapperRepository repository = MapperRepository.GetMapperRepository();
        private User utente = new User();

        public ActionResult Create(int idScheda, int idOsservazione)
        {
            return ViewOpportunita("Create", idScheda, idOsservazione, null);
        }
        public ActionResult Edit(int idScheda, int idOsservazione, int? idOpportunita)
        {
            if (!idOpportunita.HasValue)
                return RedirectToAction("Create", new { idScheda = idScheda });
            else
                return ViewOpportunita("Edit", idScheda, idOsservazione, idOpportunita);
        }

        private ViewResult ViewOpportunita(string viewName, int idScheda, int idOsservazione, int? idOpportunita)
        {
            utente = (User)Session["user"];
            Scheda scheda = repository.GetScheda(idScheda);
            Osservazione osservazione = null;
            bool schedaOffline = false;

            //if (osservazione == null)
            //    throw new ErroreInterno("Osservazione non trovata");

            Opportunita opportunita = null;
            Bacteria bacteria = null;
            if (idOpportunita.HasValue)
            {
                opportunita = repository.GetOpportunita(idOpportunita.Value);
                if (opportunita == null)
                    throw new ErroreInterno("Opportunià non trovata");
                bacteria = opportunita.Bacteria;
            }
            else
            {
                opportunita = new Opportunita();
                opportunita.idOsservazione = idOsservazione;
                opportunita.idAzione = 0;
                opportunita.idIndicazione = 0;
                bacteria = new Bacteria();
                bacteria.code = "nessun batterio     ";
            }

            if (idOsservazione > 0)
            {
                osservazione = repository.GetOsservazione(idOsservazione);
                opportunita.Osservazione = osservazione;
                schedaOffline = opportunita.Osservazione.Scheda.Offline;
            }

            SelectList selectList = null;
            if (bacteria != null)
                selectList = new SelectList(repository.GetBacteri(), nameof(Bacteria.code), nameof(Bacteria.description_IT), bacteria.code);
            else
                selectList = new SelectList(repository.GetBacteri(), nameof(Bacteria.code), nameof(Bacteria.description_IT));

            Entities db = new Entities(true);
            var listaOperatori = (from s in db.Scheda
                                  from oss in db.Osservazione.Where(x => x.idScheda == s.id)
                                  from ope in db.Operatore.Where(x => x.id == oss.idOperatore)
                                  where s.id == idScheda
                                  orderby ope.nomeCategoria, oss.operatoreEsterno
                                  select new
                                  {
                                      id = (int)oss.id,
                                      nome = oss.operatoreEsterno.HasValue && oss.operatoreEsterno.Value ? ope.nomeCategoria + " (Esterno)" : ope.nomeCategoria
                                  }).ToList();

            ViewBag.IdScheda = idScheda;
            ViewBag.Offline = schedaOffline;
            ViewBag.id = opportunita.id;
            ViewBag.ListaOperatori = new SelectList(listaOperatori, "id", "nome", idOsservazione);
            ViewBag.Batterio = selectList;

            repository.UpdatePosizione(utente.Username, Request.RawUrl, null);
            return View(viewName, opportunita);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,idOsservazione,idAzione,idIndicazione,idBacteria")] Opportunita opportunita, int idScheda, bool Offline)
        {
            return Salva(opportunita, idScheda, Offline, ModelState);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,idOsservazione,idAzione,idIndicazione,idBacteria")] Opportunita opportunita, int idScheda, bool Offline)
        {
            return Salva(opportunita, idScheda, Offline, ModelState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Salva(Opportunita opportunita, int idScheda, bool Offline, ModelStateDictionary modelState)
        {
            utente = (User)Session["user"];
            Osservazione osservazione = repository.GetOsservazione(opportunita.idOsservazione);

            if (ModelState.IsValid)
            {
                repository.Modified(opportunita);

                if (!Offline)
                    opportunita.data = DateTime.Now;
                repository.SaveOpportunita(opportunita);
                repository.InsertUpdateLog(utente.Username, "Salva opportunità id=" + opportunita.id + "di osservazione id=" + opportunita.idOsservazione, Request.Url.PathAndQuery);
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
                if (osservazione == null)
                    return ViewOpportunita("Create", idScheda, (int)opportunita.idOsservazione, null);
                else
                    return ViewOpportunita("Edit", idScheda, (int)opportunita.idOsservazione, opportunita.id > 0 ? (int)opportunita.id : (int?)null);

            }

            return RedirectToAction("Create", new { idScheda = osservazione.idScheda, idOsservazione = osservazione.id });

            //return RedirectToAction("Edit", "Osservazione", new { idScheda = osservazione.idScheda, idOsservazione = opportunita.idOsservazione });
        }

        [HttpPost]
        public ActionResult Delete(int IDOsservazione, int IDOpportunita)
        {
            utente = (User)Session["user"];
            try
            {
                repository.DeleteOpportunita(IDOpportunita);
                repository.InsertUpdateLog(utente.Username, "Elimina opportunità id=" + IDOpportunita + "di osservazione id=" + IDOsservazione, Request.Url.PathAndQuery);
                Osservazione osservazione = repository.GetOsservazione(IDOsservazione);
                return Json(new { newurl = Url.Action("Edit", "Osservazione", new { osservazione.idScheda, IDOsservazione, type = Controllers.OsservazioneController.TipoAzione.DeleteOpportunita }) });
            }
            catch (Exception ex)
            {
                throw new ErroreInterno(String.Format("Errore elimina opportunità: {0}", ex.Message));
            }
        }

    }
}