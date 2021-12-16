using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mapper.Authentication;
using Mapper.Models;
using Mapper.Models.Repository;
using Newtonsoft.Json;

namespace Mapper.Controllers
{
    public class UtenteStrutturaController : BaseController
    {
        private MapperRepository repository = MapperRepository.GetMapperRepository();
        private User utente = new User();

        // GET: UtenteStruttura
        public ActionResult Create(int idUtente, int idRuolo)
        {
            UtenteStruttura us = new UtenteStruttura();
            us.idUtente = idUtente;
            us.idRuolo = idRuolo;
            us.dataDal = DateTime.Parse(DateTime.Now.ToShortDateString());
            SetViewBag(us);
            return View(us);
        }

        public ActionResult Edit(int? ID, int idUtente, int idRuolo)
        {
            if (!ID.HasValue || ID.Value == 0)
                return RedirectToAction("Create", new { idUtente = idUtente, idRuolo = idRuolo });
            else
            {
                if (String.IsNullOrEmpty((String)Session["UtenteStrutture" + idUtente]))
                {
                    TempData["Alert"] = new Alert { Title = "Sessione scaduta", AlertType = Alert.AlertTypeEnum.Warning };
                    return RedirectToAction("Edit", "Utente", new { idUtente = idUtente, idRuolo = idRuolo });
                }
                List<UtenteStruttura> strutture = JsonConvert.DeserializeObject<List<UtenteStruttura>>((string)Session["UtenteStrutture" + idUtente]);
                UtenteStruttura us = strutture.Find(x => x.ID == ID.Value);// repository.GetUtenteStruttura(ID.Value);
                us.idRuolo = idRuolo;
                SetViewBag(us);
                return View(us);
            }
        }

        private void SetViewBag(UtenteStruttura us)
        {
            utente = (User)Session["user"];

            string codRegione = null;
            string codAzienda = null;
            string codTipoStruttura = null;
            string codStruttura = null;
            string keyReparto = null;
            List<vwRegione> listaRegioni = null;
            List<vwAzienda> listaAziende = null;
            List<TipologiaStruttura> listaTipiStruttura = null;
            List<vwStruttura> listaStrutture = null;
            List<vwReparto> listaReparti = null;

            listaTipiStruttura = repository.GetTipiStruttureAttive();

            codRegione = utente.UtenteStrutture[0].codRegione;
            codAzienda = utente.UtenteStrutture[0].codAzienda;
            codTipoStruttura = utente.UtenteStrutture[0].CodiceTipologiaStruttura;
            codStruttura = utente.UtenteStrutture[0].CodiceStruttura;
            keyReparto = utente.UtenteStrutture[0].KeyReparto;

            /* setto le proprietà della struttura in modifica */
            if (!String.IsNullOrEmpty(us.codRegione))
                codRegione = us.codRegione;
            if (!String.IsNullOrEmpty(us.codAzienda))
                codAzienda = us.codAzienda;
            //if (!String.IsNullOrEmpty(us.CodiceTipologiaStruttura))
            //    codTipoStruttura = us.CodiceTipologiaStruttura;
            if (!String.IsNullOrEmpty(us.CodiceStruttura))
                codStruttura = us.CodiceStruttura;
            keyReparto = us.KeyReparto;

            listaRegioni = repository.GetRegioniUtente(utente.Username);
            listaAziende = repository.GetAziende(codRegione, false, null);
            listaStrutture = string.IsNullOrEmpty(codAzienda) ? new List<vwStruttura>() : repository.GetStruttureBuff(codRegione, codAzienda, codTipoStruttura, false, null, null, utente.Ruolo == Ruolo.Aziendale);
            listaReparti = string.IsNullOrEmpty(codStruttura) ? new List<vwReparto>() : repository.GetReparti(codRegione, codAzienda, codStruttura, false, false);


            if (utente.Ruolo == Ruolo.Aziendale || utente.Ruolo == Ruolo.ReferenteStruttura || utente.Ruolo == Ruolo.Osservatore)
            {
                listaAziende = (from a in listaAziende
                                from uus in utente.UtenteStrutture
                                where a.CodRegione == uus.codRegione && a.CodAzienda == uus.codAzienda
                                select a).Distinct().ToList();
            }
            if (utente.Ruolo == Ruolo.ReferenteStruttura || utente.Ruolo == Ruolo.Osservatore)
            {
                listaStrutture = (from s in listaStrutture
                                  from uus in utente.UtenteStrutture
                                  where s.CodRegione == uus.codRegione && s.CodAzienda == uus.codAzienda && s.IdStrutturaErogatrice == uus.idStrutturaErogatrice && s.idWebServiceStruttura == uus.idWebServiceStruttura
                                  select s).Distinct().ToList();
            }
            if (utente.Ruolo == Ruolo.Osservatore)
            {
                listaReparti = (from r in listaReparti
                                from uus in utente.UtenteStrutture
                                where r.CodRegione == uus.codRegione && r.CodAzienda == uus.codAzienda && r.IdStrutturaErogatrice == uus.idStrutturaErogatrice && r.idWebServiceStruttura == uus.idWebServiceStruttura && r.IdReparto == uus.idReparto && r.IdWebServiceReparto == uus.idWebServiceReparto
                                select r).Distinct().ToList();
            }

            Utente u = repository.GetUtente(us.idUtente);
            ViewBag.NomeUtente = String.Format("{0} {1}", u.cognome, u.nome);

            ViewBag.Regione = new SelectList(listaRegioni, "CodRegione", "Denominazione", codRegione);
            ViewBag.Azienda = new SelectList(listaAziende, "CodAzienda", "Denominazione", codAzienda);
            ViewBag.TipoStruttura = new SelectList(listaTipiStruttura, "CodTipologia", "DescrizioneForDisplay", codTipoStruttura);
            ViewBag.Struttura = new SelectList(listaStrutture, "KeyStruttura", "Denominazione", codStruttura);
            ViewBag.ddlReparto = new SelectList(listaReparti, "KeyReparto", "Nome", keyReparto);

            ViewBag.EnableRegione = listaRegioni.Count == 1 ? "disabled" : "";
            ViewBag.EnableAzienda = listaAziende.Count == 1 ? "disabled" : "";
            ViewBag.EnableStruttura = listaStrutture.Count == 1 ? "disabled" : "";

            var selectPubblicaPrivata = new List<SelectListItem>();
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Pubbliche e private", Value = "", Selected = true });
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Solo pubbliche", Value = "1" });
            selectPubblicaPrivata.Add(new SelectListItem { Text = "Solo private", Value = "2" });
            ViewBag.PubblicaPrivata = selectPubblicaPrivata;
        }

        public ActionResult SalvaAssociazione(UtenteStruttura us, string keyStruttura, string keyReparto)
        {
            Alert alert = new Alert();
            alert.AlertType = Alert.AlertTypeEnum.Error;
            alert.Title = "ATTENZIONE! Sono presenti degli errori!";



            //if (ModelState.IsValid)
            //{
            if (String.IsNullOrEmpty(us.codRegione))
            {
                TempData["Alert"] = new Alert { Title = "Campo Regione obbligatorio", AlertType = Alert.AlertTypeEnum.Warning };
                return RedirectToAction("Edit", "UtenteStruttura", new { ID = us.ID, idUtente = us.idUtente, idRuolo = us.idRuolo });
            }
            if (String.IsNullOrEmpty(us.codAzienda))
            {
                TempData["Alert"] = new Alert { Title = "Campo Azienda obbligatorio", AlertType = Alert.AlertTypeEnum.Warning };
                return RedirectToAction("Edit", "UtenteStruttura", new { ID = us.ID, idUtente = us.idUtente, idRuolo = us.idRuolo });
            }
            if (!string.IsNullOrEmpty(keyStruttura))
            {
                vwStruttura struttura = repository.GetStrutturaCorrente(keyStruttura);
                us.idStrutturaErogatrice = struttura.IdStrutturaErogatrice;
                us.idWebServiceStruttura = struttura.idWebServiceStruttura;
            }
            if (!string.IsNullOrEmpty(keyReparto))
            {
                vwReparto reparto = repository.GetReparto(keyReparto);
                us.idReparto = reparto.IdReparto;
                us.idWebServiceReparto = reparto.IdWebServiceReparto;
            }
            if (String.IsNullOrEmpty((String)Session["UtenteStrutture" + us.idUtente]))
            {
                TempData["Alert"] = new Alert { Title = "Sessione scaduta", AlertType = Alert.AlertTypeEnum.Warning };
                return RedirectToAction("Edit", "Utente", new { idUtente = us.idUtente, idRuolo = us.idRuolo });
            }
            List<UtenteStruttura> strutture = JsonConvert.DeserializeObject<List<UtenteStruttura>>((string)Session["UtenteStrutture" + us.idUtente]);
            if (strutture == null)
                strutture = new List<UtenteStruttura>();

            if (us.idRuolo == (int)Ruolo.Osservatore)
            {
                if (!us.idReparto.HasValue)
                    alert.Messages.Add("Campo Reparto obbligatorio");
                if (strutture.Any(x => x.ID != us.ID && x.idReparto == us.idReparto && x.idWebServiceReparto == us.idWebServiceReparto))
                    alert.Messages.Add("Associazione già presente");
            }
            else if (us.idRuolo == (int)Ruolo.ReferenteStruttura)
            {
                if (String.IsNullOrEmpty(keyStruttura))
                    alert.Messages.Add("Campo Struttura obbligatorio");
                if (strutture.Any(x => x.ID != us.ID && x.idStrutturaErogatrice == us.idStrutturaErogatrice && x.idWebServiceStruttura == us.idWebServiceStruttura))
                    alert.Messages.Add("Associazione già presente");
            }
            else if (us.idRuolo == (int)Ruolo.Aziendale)
            {
                if (strutture.Any(x => x.ID != us.ID && x.codAzienda == us.codAzienda))
                    alert.Messages.Add("Associazione già presente");
            }
            else if (us.idRuolo == (int)Ruolo.Regionale)
            {
                if (strutture.Any(x => x.ID != us.ID && x.codRegione == us.codRegione))
                    alert.Messages.Add("Associazione già presente");
            }
            if (us.dataAl.HasValue)
            {
                if (us.dataDal > us.dataAl.Value)
                    alert.Messages.Add("Il campo Data dal deve essere minore del campo Data al");
            }
            if (alert.Messages.Count > 0)
            {
                TempData["Alert"] = alert;
                return RedirectToAction("Edit", "UtenteStruttura", new { ID = us.ID, idUtente = us.idUtente, idRuolo = us.idRuolo });
            }

            int ndx = strutture.IndexOf(strutture.Find(x => x.ID == us.ID));
            if (ndx >= 0)
                strutture[ndx] = us;
            else
            {
                us.ID = !strutture.Any(x => x.ID < 0) ? -1 : strutture.Where(x => x.ID < 0).Min(x => x.ID) - 1;
                strutture.Add(us);
            }

            string str = JsonConvert.SerializeObject(strutture);
            Session["UtenteStrutture" + us.idUtente] = str;
            return RedirectToAction("Edit", "Utente", new { idUtente = us.idUtente, idRuolo = us.idRuolo, aggiorna = true });
            //}
            //else
            //{
            //    foreach (var val in ModelState.Values)
            //    {
            //        foreach (var err in val.Errors)
            //        {
            //            alert.Messages.Add(err.ErrorMessage);
            //        }
            //    }
            //    TempData["Alert"] = alert;
            //    return RedirectToAction("Edit", "UtenteStruttura", new { ID = us.ID, idUtente = us.idUtente, idRuolo = us.idRuolo });
            //}
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UtenteStruttura us, string CodRegioneForDisplay, string CodAziendaForDisplay, string KeyStrutturaForDisplay, string KeyRepartoForDisplay)
        {
            return SalvaAssociazione(us, KeyStrutturaForDisplay, KeyRepartoForDisplay);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UtenteStruttura us, string KeyStrutturaForDisplay, string KeyRepartoForDisplay)
        {

            return SalvaAssociazione(us, KeyStrutturaForDisplay, KeyRepartoForDisplay);
        }
    }
}