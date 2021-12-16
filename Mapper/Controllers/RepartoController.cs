using Mapper.Authentication;
using Mapper.Models;
using Mapper.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mapper.Controllers
{
    public class RepartoController : BaseController
    {
        // GET: Reparto
        private MapperRepository repository = MapperRepository.GetMapperRepository();
        private User utente = new User();

        public enum TipoAzione
        {
            Create, Edit, Delete
        }

        public ActionResult Create(string keyReparto)
        {
            return ViewReparto(TipoAzione.Create, keyReparto);
        }
        public ActionResult Edit(string keyReparto)
        {
            if (!string.IsNullOrEmpty(keyReparto))
            {
                return ViewReparto(TipoAzione.Edit, keyReparto);
            }
            else
            {
                return RedirectToAction("Create");
            }
        }

        private ViewResult ViewReparto(TipoAzione viewName, string keyReparto)
        {
            utente = (User)Session["user"];
            vwReparto reparto = null;

            if (!string.IsNullOrEmpty(keyReparto))
            {
                reparto = repository.GetReparto(keyReparto);

                if (reparto == null)
                    throw new ErroreInterno("Reparto non presente");
            }
            else
            {
                reparto = new vwReparto()
                {
                    IdReparto = 0,
                    IdWebServiceReparto = 2,
                    DataInizio = DateTime.Now,
                    Cancellato = false,
                    Stato = 3
                };
            }

            SetViewBag(ref reparto);
            repository.UpdatePosizione(utente.Username, Request.RawUrl, null);
            return View(viewName.ToString(), reparto);
        }

        public void SetViewBag(ref vwReparto reparto)
        {
            if (reparto.IdReparto == 0)
            {
                utente = (User)Session["user"];

                string codRegione = null;
                string keyAzienda = null;
                string codAzienda = null;
                string codTipoStruttura = null;
                string codStruttura = null;

                List<vwRegione> listaRegioni = null;
                List<vwAzienda> listaAziende = null;
                List<TipologiaStruttura> listaTipiStruttura = null;
                List<vwStruttura> listaStrutture = null;

                listaTipiStruttura = repository.GetTipiStruttureAttive();

                codRegione = utente.UtenteStrutture[0].codRegione;
                keyAzienda = utente.UtenteStrutture[0].KeyAzienda;
                codAzienda = utente.UtenteStrutture[0].codAzienda;
                codTipoStruttura = utente.UtenteStrutture[0].CodiceTipologiaStruttura;
                codStruttura = utente.UtenteStrutture[0].CodiceStruttura;

                listaRegioni = repository.GetRegioniUtente(utente.Username);
                listaAziende = repository.GetAziende(codRegione, false, null);
                listaStrutture = string.IsNullOrEmpty(codAzienda) ? new List<vwStruttura>() : repository.GetStruttureBuff(codRegione, codAzienda, null, false, null, null, utente.Ruolo == Ruolo.Aziendale);

                reparto.CodRegione = codRegione;
                reparto.CodAzienda = codAzienda;
                reparto.CodiceTipologiaStruttura = codTipoStruttura;

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
                ViewBag.Azienda = new SelectList(listaAziende, "KeyAzienda", "Denominazione", keyAzienda);
                ViewBag.TipoStruttura = new SelectList(listaTipiStruttura, "CodTipologia", "DescrizioneForDisplay", codTipoStruttura);
                ViewBag.Struttura = new SelectList(listaStrutture, "KeyStruttura", "Denominazione", codStruttura);
                ViewBag.EnableRegione = listaRegioni.Count == 1 ? "disabled" : "";
                ViewBag.EnableAzienda = listaAziende.Count == 1 ? "disabled" : "";
                ViewBag.EnableStruttura = listaStrutture.Count == 1 ? "disabled" : "";
            }

            var listadiscipline = MapperRepository.GetMapperRepository().GetAreeDisciplina();
            ViewBag.Disciplina = new SelectList(listadiscipline, "CodAreaDisciplina", "Nome", reparto.codAreaDisciplina);
            ViewBag.IDReparto = reparto.IdReparto;
            ViewBag.Cancella = reparto.Cancellato;
            var selectPubblicaPrivata = new List<SelectListItem>();
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Pubbliche e private", Value = "", Selected = true });
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Solo pubbliche", Value = "1" });
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Solo private", Value = "2" });
            ViewBag.PubblicaPrivata = selectPubblicaPrivata;
        }

        public ActionResult Salva(vwReparto reparto, ModelStateDictionary modelState)
        {
            utente = (User)Session["user"];

            if (reparto.IdWebServiceReparto == 1)
                modelState.Remove("codAreaDisciplina");

            if (modelState.IsValid)
            {
                //if (reparto.IdReparto > 0)
                //{
                //    vwReparto repartoDaSalvare = repository.GetReparto(reparto.IdReparto);
                //    repartoDaSalvare.Nome = reparto.Nome;
                //    repartoDaSalvare.Descrizione = reparto.Descrizione;
                //    repartoDaSalvare.codAreaDisciplina = reparto.codAreaDisciplina;
                //    repartoDaSalvare.Cancellato = reparto.Cancellato;
                //    repository.Modified(repartoDaSalvare);

                //    repository.SaveReparto(repartoDaSalvare);
                //}
                //else

                if (reparto.IdWebServiceReparto == 1)
                    if (reparto.NomeOriginale.Equals(reparto.Nome, StringComparison.InvariantCultureIgnoreCase))
                        reparto.Nome = null;

                vwStruttura struttura = repository.GetStrutturaCorrente(reparto.keyStruttura);
                reparto.IdStrutturaErogatrice = struttura.IdStrutturaErogatrice;
                reparto.idWebServiceStruttura = struttura.idWebServiceStruttura;
                reparto.DataRiferimentoStruttura = struttura.DataRiferimentoStruttura;

                repository.SaveReparto(reparto);

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

            return RedirectToAction("BackSearch", "Reparti");
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(vwReparto reparto, bool? Cancella)
        {
            reparto.Cancellato = Cancella.HasValue ? Cancella.Value : false;

            return Salva(reparto, ModelState);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(vwReparto reparto, bool? Cancella)
        {
            reparto.Cancellato = Cancella.HasValue ? Cancella.Value : false;

            return Salva(reparto, ModelState);
        }

        public ActionResult Delete(string keyReparto)
        {
            repository.SetRepartoCancellato(keyReparto);
            TempData["Alert"] = new Alert { Title = "Reparto eliminato con successo", AlertType = Alert.AlertTypeEnum.Success };
            return RedirectToAction("Edit", new { keyReparto });
        }
    }
}