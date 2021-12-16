using Mapper.Models;
using Mapper.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mapper.Authentication;
using Newtonsoft.Json;

namespace Mapper.Controllers
{
    public class UtenteController : BaseController
    {
        private MapperRepository repository = MapperRepository.GetMapperRepository();
        private User utente = new User();

        // GET: Utente
        public ActionResult Edit(int idUtente, int? idRuolo, bool? aggiorna)
        {
            utente = (User)Session["user"];
            Utente utentedettaglio = repository.GetUtente(idUtente);
            if (idRuolo.HasValue)
                utentedettaglio.idRuolo = idRuolo.Value;

            if (aggiorna == null || !aggiorna.Value)
            {
                List<UtenteStruttura> struttureProvvisorie = new List<UtenteStruttura>();
                foreach (UtenteStruttura us in utentedettaglio.UtenteStruttura)
                {
                    if (utente.Ruolo == Ruolo.Regionale)
                        us.PuoEssereModificato = utente.UtenteStrutture.Any(x => x.codRegione == us.codRegione);
                    else if (utente.Ruolo == Ruolo.Aziendale)
                        us.PuoEssereModificato = utente.UtenteStrutture.Any(x => x.codRegione == us.codRegione && x.codAzienda == us.codAzienda);
                    else if (utente.Ruolo == Ruolo.ReferenteStruttura)
                        us.PuoEssereModificato = utente.UtenteStrutture.Any(x => x.codRegione == us.codRegione && x.codAzienda == us.codAzienda && x.idStrutturaErogatrice == us.idStrutturaErogatrice && x.idWebServiceStruttura == us.idWebServiceStruttura);

                    struttureProvvisorie.Add(us);
                }
                utentedettaglio.struttureProvvisorie = struttureProvvisorie;

                Session.Remove("UtenteStrutture" + idUtente);
                string str = JsonConvert.SerializeObject(struttureProvvisorie);
                Session.Add("UtenteStrutture" + idUtente, str);

                repository.UpdatePosizione(utente.Username, Request.RawUrl, null);
            }
            else
            {
                TempData["Alert"] = new Alert { Title = "Necessario cliccare il tasto Salva per rendere effettive le modifiche", AlertType = Alert.AlertTypeEnum.Info };
                utentedettaglio.struttureProvvisorie = JsonConvert.DeserializeObject<List<UtenteStruttura>>((string)Session["UtenteStrutture" + idUtente]);
            }

            SetViewBag(utentedettaglio);
            return View(utentedettaglio);
        }

        private void SetViewBag(Utente utentedettaglio)
        {
            utente = (User)Session["user"];

            ViewBag.cancellato = utentedettaglio.cancellato;
            ViewBag.Profilo = new SelectList(repository.GetRuoli((int)utente.Ruolo), "id", "nome", utentedettaglio.idRuolo);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,cognome,nome,email,username,codicefiscale,cancellato,idruolo")] Utente u)
        {
            Alert alert = new Alert();
            alert.AlertType = Alert.AlertTypeEnum.Error;
            alert.Title = "ATTENZIONE! Sono presenti degli errori! Nessun dato è stato salvato";

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(u.CodiceFiscale) && repository.CodiceFiscaleAssociatoAdUtente(u.id, u.CodiceFiscale))
                {
                    alert.Messages.Add($"Il codice fiscale {u.CodiceFiscale} è associato ad un altro utente attivo");
                    TempData["Alert"] = alert;
                    return RedirectToAction("Edit", new { idUtente = u.id });
                }

                List<UtenteStruttura> struttureProvvisorie = JsonConvert.DeserializeObject<List<UtenteStruttura>>((string)Session["UtenteStrutture" + u.id]);
                if (u.idRuolo != (int)Ruolo.NonAssociato && (struttureProvvisorie == null || struttureProvvisorie.Count == 0))
                {
                    alert.Messages.Add("Necessario aggiungere un'associazione");
                    TempData["Alert"] = alert;
                    return RedirectToAction("Edit", new { idUtente = u.id });
                }

                Salva(u, struttureProvvisorie);

                TempData["Alert"] = new Alert { Title = "Salvataggio eseguito con successo", AlertType = Alert.AlertTypeEnum.Success };
            }
            else
            {

                foreach (var val in ModelState.Values)
                {
                    foreach (var err in val.Errors)
                    {
                        alert.Messages.Add(err.ErrorMessage);
                    }
                }
                TempData["Alert"] = alert;
            }

            return RedirectToAction("Edit", new { idUtente = u.id });
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit2([Bind(Include = "id,cognome,nome,email,username,codicefiscale,cancellato,idruolo")] Utente u)
        {
            Alert alert = new Alert();
            alert.AlertType = Alert.AlertTypeEnum.Error;
            alert.Title = "ATTENZIONE! Sono presenti degli errori! Nessun dato è stato salvato";

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(u.CodiceFiscale) && repository.CodiceFiscaleAssociatoAdUtente(u.id, u.CodiceFiscale))
                {
                    alert.Messages.Add($"Il codice fiscale {u.CodiceFiscale} è associato ad un altro utente attivo");
                    TempData["Alert"] = alert;
                    return RedirectToAction("Edit", new { idUtente = u.id });
                }

                List<UtenteStruttura> struttureProvvisorie = JsonConvert.DeserializeObject<List<UtenteStruttura>>((string)Session["UtenteStrutture" + u.id]);

                Salva(u, struttureProvvisorie);

                return RedirectToAction("Create", "UtenteStruttura", new { idUtente = u.id, idRuolo = u.idRuolo });
            }
            else
            {

                foreach (var val in ModelState.Values)
                {
                    foreach (var err in val.Errors)
                    {
                        alert.Messages.Add(err.ErrorMessage);
                    }
                }
                TempData["Alert"] = alert;
            }

            return RedirectToAction("Edit", new { idUtente = u.id });
        }

        private void Salva(Utente u, List<UtenteStruttura> struttureProvvisorie)
        {
            if (string.IsNullOrEmpty(u.username))
                u.username = u.CodiceFiscale;

            /*se visualizzo l'utente vuol dire che è attivato da idm. Perciò in questo modo non rischio di disattivarlo*/
            u.attivato = true;

            repository.Modified(u);
            u.UtenteStruttura.Clear();
            if (struttureProvvisorie != null && struttureProvvisorie.Count > 0)
            {
                foreach (UtenteStruttura us in struttureProvvisorie)
                {
                    if (u.idRuolo == (int)Ruolo.Regionale)
                    {
                        us.codAzienda = null;
                        us.idStrutturaErogatrice = null;
                        us.idWebServiceStruttura = null;
                        us.idReparto = null;
                        us.idWebServiceReparto = null;
                    }
                    else if (u.idRuolo == (int)Ruolo.Aziendale)
                    {
                        us.idStrutturaErogatrice = null;
                        us.idWebServiceStruttura = null;
                        us.idReparto = null;
                        us.idWebServiceReparto = null;
                    }
                    else if (u.idRuolo == (int)Ruolo.ReferenteStruttura)
                    {
                        us.idReparto = null;
                        us.idWebServiceReparto = null;
                    }
                    else if (u.idRuolo == (int)Ruolo.Osservatore)
                    {
                    }
                    else if (u.idRuolo == (int)Ruolo.NonAssociato)
                    {
                        us.codRegione = null;
                        us.codAzienda = null;
                        us.idStrutturaErogatrice = null;
                        us.idWebServiceStruttura = null;
                        us.idReparto = null;
                        us.idWebServiceReparto = null;
                    }

                    u.UtenteStruttura.Add(new UtenteStruttura()
                    {
                        ID = us.ID,
                        idUtente = us.idUtente,
                        codRegione = us.codRegione,
                        codAzienda = us.codAzienda,
                        idStrutturaErogatrice = us.idStrutturaErogatrice,
                        idWebServiceStruttura = us.idWebServiceStruttura,
                        idReparto = us.idReparto,
                        idWebServiceReparto = us.idWebServiceReparto,
                        dataDal = us.dataDal,
                        dataAl = us.dataAl
                    }
                    );
                }
            }
            repository.SaveUtente(u);
        }

        public ActionResult RefreshTabella(int id, int idRuolo)
        {
            Utente utentedettaglio = repository.GetUtente(id);

            if (idRuolo == utentedettaglio.idRuolo)
            {
                List<UtenteStruttura> struttureProvvisorie = new List<UtenteStruttura>();
                foreach (UtenteStruttura us in utentedettaglio.UtenteStruttura)
                {
                    struttureProvvisorie.Add(us);
                }
                utentedettaglio.struttureProvvisorie = struttureProvvisorie;
            }
            else
            {
                utentedettaglio.idRuolo = idRuolo;
                utentedettaglio.struttureProvvisorie = null;
            }

            Session.Remove("UtenteStrutture" + id);
            string str = JsonConvert.SerializeObject(utentedettaglio.struttureProvvisorie);
            Session.Add("UtenteStrutture" + id, str);

            return PartialView("Partial/_partialUtenteStrutture", utentedettaglio);
        }

        public ActionResult CancellaAssociazione(int idUtente, int id)
        {
            List<UtenteStruttura> struttureProvvisorie = JsonConvert.DeserializeObject<List<UtenteStruttura>>((string)Session["UtenteStrutture" + idUtente]);
            Utente utentedettaglio = repository.GetUtente(idUtente);

            UtenteStruttura us = struttureProvvisorie.Find(x => x.ID == id);
            DateTime dt = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy"));

            if (id < 0 || us.dataDal == dt)
                /*qualora fosse una nuova struttura appena associata non ha senso salvarla*/
                struttureProvvisorie.Remove(us);
            else
            {
                /*è una struttura già salvata in precedenza*/
                int ndx = struttureProvvisorie.IndexOf(us);
                us.dataAl = dt;
                struttureProvvisorie[ndx] = us;
            }
            utentedettaglio.struttureProvvisorie = struttureProvvisorie;

            Session.Remove("UtenteStrutture" + idUtente);
            string str = JsonConvert.SerializeObject(struttureProvvisorie);
            Session.Add("UtenteStrutture" + idUtente, str);
            
            return PartialView("Partial/_partialUtenteStrutture", utentedettaglio);
        }

    }
}