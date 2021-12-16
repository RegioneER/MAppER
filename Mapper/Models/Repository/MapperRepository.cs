using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.UI.WebControls;
using Mapper.Controllers;

namespace Mapper.Models.Repository
{
    public class MapperRepository : IRepository
    {
        Entities db = null;

        public MapperRepository()
        {
            db = new Entities(true);
        }

        public Entities DBContext
        {
            get { return db; }
        }

        public static MapperRepository GetMapperRepository()
        {
            return new MapperRepository();
        }

        public List<vwRegione> GetRegioni()
        {
            return db.vwRegione.OrderBy(x => x.Denominazione).ToList();
        }

        public List<vwAreaDisciplina> GetAreeDisciplina()
        {
            return db.vwAreaDisciplina.OrderBy(x => x.Ordinale).ToList();
        }

        public vwAreaDisciplina GetAreaDisciplina(int idDisciplina)
        {
            return db.vwAreaDisciplina.FirstOrDefault(x => x.CodAreaDisciplina == idDisciplina);
        }

        public vwAzienda GetAzienda(string codAzienda, string codregione)
        {
            return db.vwAzienda.FirstOrDefault(x => x.CodRegione == codregione && x.CodAzienda == codAzienda && !x.DataFine.HasValue);
        }

        public vwAzienda GetAzienda(string codAzienda, string codregione, DateTime dataInizio)
        {
            return db.vwAzienda.FirstOrDefault(x => x.CodRegione == codregione && x.CodAzienda == codAzienda && x.DataInizio == dataInizio);
        }

        public List<vwAzienda> GetAziende()
        {
            return db.vwAzienda.Where(x => !x.DataFine.HasValue).OrderBy(x => x.Denominazione).ToList();
        }

        public List<vwStruttura> GetStrutture()
        {
            return db.vwStruttura.Where(x => !x.DataFine.HasValue && !x.DataChiusura.HasValue).OrderBy(x => x.Denominazione).ToList();
        }
        public UtenteDaCensire GetUtenteDaCensire(int idUtente)
        {
            return db.UtenteDaCensire.Find(idUtente);
        }
        public bool CFPresenteUtenteDaCensire(string cf)
        {
            UtenteDaCensire U = db.UtenteDaCensire.FirstOrDefault(x => x.CodiceFiscale == cf);
            return U != null ? true : false;
        }

        public bool EmailPresenteUtenteDaCensire(string email)
        {
            UtenteDaCensire U = db.UtenteDaCensire.FirstOrDefault(x => x.Email == email);
            return U != null ? true : false;
        }

        public List<UtenteDaCensire> GetUtentiDaCensire()
        {
            return db.UtenteDaCensire.OrderBy(x => x.DataCandidatura).ToList();
        }

        public List<UtenteDaCensire> GetUtentiDaCensire(int skip, int take, int idUtente, string cognome, string nome, int? idRuolo, bool? pubblico, DateTime? DataDal, DateTime? DataAl, string codRegione, string codAzienda, string keyStruttura, int idStato, string codicefiscale, out int utentitotali)
        {
            Utente utente = db.Utente.Find(idUtente);

            List<String> regioni = new List<String>();
            List<String> aziende = new List<string>();
            List<String> strutture = new List<string>();
            List<String> listaReparti = new List<String>();
            foreach (UtenteStruttura us in utente.struttureAttive)
            {
                if (utente.idRuolo == (int)Ruolo.Regionale && string.IsNullOrEmpty(codRegione))
                    regioni.Add(us.codRegione);
                if (utente.idRuolo == (int)Ruolo.Aziendale)
                {
                    if (string.IsNullOrEmpty(codAzienda))
                        aziende.Add(us.codAzienda);
                    strutture.AddRange((from s in GetStruttureBuff(us.codRegione, us.codAzienda, null, true, null, null, true)
                                        select s.KeyStruttura).ToList());
                }
                aziende.Add(us.codAzienda);
                if (utente.idRuolo == (int)Ruolo.ReferenteStruttura && string.IsNullOrEmpty(keyStruttura))
                    strutture.Add(us.CodiceStruttura);
            };

            IQueryable<UtenteDaCensire> utentidacensire = db.UtenteDaCensire.AsQueryable();

            if (!string.IsNullOrEmpty(codRegione))
                utentidacensire = utentidacensire.Where(x => x.CodRegione == codRegione);
            else
                utentidacensire = utentidacensire.Where(x => regioni.Count == 0 || regioni.Contains(x.CodRegione));

            if (!string.IsNullOrEmpty(codAzienda))
                utentidacensire = utentidacensire.Where(x => x.CodAzienda == codAzienda);
            else
                utentidacensire = utentidacensire.Where(x => aziende.Count == 0 || aziende.Contains(x.CodAzienda));

            if (!string.IsNullOrEmpty(keyStruttura))
                utentidacensire = utentidacensire.Where(x => x.KeyStruttura == keyStruttura);
            else
                utentidacensire = utentidacensire.Where(x => strutture.Count == 0 || strutture.Contains(x.KeyStruttura));

            if (!string.IsNullOrEmpty(cognome))
                utentidacensire = utentidacensire.Where(x => x.Cognome.Contains(cognome));

            if (!string.IsNullOrEmpty(nome))
                utentidacensire = utentidacensire.Where(x => x.Nome.Contains(nome));

            if (!string.IsNullOrEmpty(codicefiscale))
                utentidacensire = utentidacensire.Where(x => x.CodiceFiscale == codicefiscale);

            if (DataDal.HasValue)
                utentidacensire = utentidacensire.Where(x => x.DataCandidatura >= DataDal.Value);

            if (DataAl.HasValue)
                utentidacensire = utentidacensire.Where(x => x.DataCandidatura <= DataAl.Value);

            utentidacensire = utentidacensire.Where(x => x.IdStato == idStato);

            if (pubblico.HasValue)
                utentidacensire = utentidacensire.Where(x => x.Pubblico == pubblico);

            if (idRuolo.HasValue)
                utentidacensire = utentidacensire.Where(x => x.IdRuolo == idRuolo.Value);

            utentidacensire = utentidacensire.Where(x => x.CodiceFiscale != null);

            var result = utentidacensire.ToList();
            utentitotali = result.Count();
            if (skip > 0 && take > 0)
                result = result.Skip(skip).Take(take).ToList();

            return result;

        }

        public string GetNomeReparto(int idreparto)
        {
            if (idreparto != 0)
            {
                return db.vwReparto.Find(idreparto).Nome;
            }
            else
            {
                return idreparto.ToString();
            }
        }

        public vwStruttura GetStruttura(int idStruttura, int idWebService)
        {
            return db.vwStruttura.Where(x => x.IdStrutturaErogatrice == idStruttura && x.idWebServiceStruttura == idWebService).OrderBy(x => x.DataFine).FirstOrDefault();

        }

        public List<vwStruttura> GetvwStrutture(string codRegione, string codAzienda, string codTipoStruttura)
        {
            return db.vwStruttura.Where(x => x.CodRegione == codRegione
                                            && x.CodAzienda == codAzienda
                                            && (String.IsNullOrEmpty(codTipoStruttura) || x.CodiceTipologiaStruttura == codTipoStruttura)
                                            && (!x.DataFine.HasValue || (x.DataFine.HasValue && x.DataFine.Value > DateTime.Now))
                                          ).ToList();
        }

        public vwStruttura GetStrutturaCorrente(string keyStruttura)
        {
            return db.vwStruttura.FirstOrDefault(x => x.KeyStruttura == keyStruttura && !x.DataFine.HasValue && !x.DataChiusura.HasValue);
        }

        public vwStruttura GetStrutturaDataScheda(string keyStruttura, DateTime dataScheda)
        {
            return db.vwStruttura.Where(x => x.KeyStruttura == keyStruttura && x.DataRiferimentoStruttura <= dataScheda).OrderByDescending(x => x.DataRiferimentoStruttura).First();
        }

        public vwRegione GetRegione(string codRegione)
        {
            return db.vwRegione.First(x => x.CodRegione == codRegione);
        }
        public List<vwRegione> GetRegioniUtente(string userName)
        {
            var regioni = (from u in db.Utente
                           from us in db.UtenteStruttura.Where(x => x.idUtente == u.id)
                           from r in db.vwRegione.Where(x => x.CodRegione == us.codRegione)
                           where u.username == userName
                           select new
                           {
                               codice = r.CodRegione,
                               nome = r.Denominazione
                           }).Distinct();

            List<vwRegione> listaRegioni = new List<vwRegione>();
            foreach (var item in regioni)
            {
                listaRegioni.Add(new vwRegione()
                {
                    CodRegione = item.codice,
                    Denominazione = item.nome

                });
            }

            return listaRegioni;
        }

        public List<vwAzienda> GetAziende(string codRegione)
        {
            return db.vwAzienda.Where(x => x.CodRegione == codRegione && !x.DataFine.HasValue).OrderBy(x => x.Denominazione).ToList();
        }

        public List<vwAzienda> GetAziende(string codRegione, bool mostraCessate, DateTime? dataScheda)
        {
            return db.vwAzienda.Where(x => x.CodRegione == codRegione
                && (mostraCessate || !x.DataFine.HasValue)
                && (!dataScheda.HasValue || (x.DataInizio <= dataScheda.Value && (!x.DataFine.HasValue || x.DataFine.Value >= dataScheda)))
                ).OrderBy(x => x.Denominazione).ToList();
        }
        //public List<vwAzienda> GetAziendeUtente(string userName, string codRegione)
        //{
        //    var aziende = (from u in db.Utente
        //                   from us in db.UtenteStruttura.Where(x => x.idUtente == u.id)
        //                   from a in db.vwAzienda.Where(x => x.CodAzienda == us.codAzienda && !x.DataFine.HasValue)
        //                   where u.username == userName
        //                   && a.CodRegione == codRegione
        //                   select new
        //                   {
        //                       a.CodAzienda,
        //                       a.KeyAzienda,
        //                       a.Denominazione
        //                   }).OrderBy(x => x.Denominazione).Distinct();

        //    List<vwAzienda> listaAzienda = new List<vwAzienda>();
        //    foreach (var item in aziende)
        //    {
        //        listaAzienda.Add(new vwAzienda()
        //        {
        //            KeyAzienda = item.KeyAzienda,
        //            CodAzienda = item.CodAzienda,
        //            Denominazione = item.Denominazione
        //        });
        //    }

        //    return listaAzienda;
        //}
        //public List<vwAzienda> GetAziendeUtente(string userName, string codRegione, bool mostraCessate)
        //{
        //    var aziende = (from u in db.Utente
        //                   from us in db.UtenteStruttura.Where(x => x.idUtente == u.id)
        //                   from a in db.vwAzienda.Where(x => x.CodAzienda == us.codAzienda)
        //                   where u.username == userName
        //                   && a.CodRegione == codRegione
        //                   && (mostraCessate == true || !a.DataFine.HasValue)
        //                   select new
        //                   {
        //                       a.KeyAzienda,
        //                       a.CodAzienda,
        //                       a.Denominazione
        //                   }).OrderBy(x => x.Denominazione).Distinct();

        //    List<vwAzienda> listaAzienda = new List<vwAzienda>();
        //    foreach (var item in aziende)
        //    {
        //        listaAzienda.Add(new vwAzienda()
        //        {
        //            KeyAzienda = item.KeyAzienda,
        //            CodAzienda = item.CodAzienda,
        //            Denominazione = item.Denominazione
        //        });
        //    }

        //    return listaAzienda;
        //}

        //public List<vwStruttura> GetStruttureBuff(string codRegione, string codAzienda)
        //{
        //    return (from s in db.vwStruttura
        //                //from ts in db.vwTipologiaStruttura.Where(x => x.CodTipologia == s.CodiceTipologiaStruttura)
        //            where s.CodRegione == codRegione
        //                  && s.CodAzienda == codAzienda
        //                  && !s.DataChiusura.HasValue && !s.DataFine.HasValue
        //            //    && ts.IsAttivo == true
        //            orderby s.Denominazione
        //            select s).ToList();
        //}

        //public List<vwStruttura> GetStruttureBuff(string codRegione, string codAzienda, string codTipoStruttura)
        //{
        //    return (from s in db.vwStruttura
        //            where s.CodRegione == codRegione
        //                  && s.CodAzienda == codAzienda
        //                  && (String.IsNullOrEmpty(codTipoStruttura) || s.CodiceTipologiaStruttura == codTipoStruttura)
        //                  && !s.DataChiusura.HasValue && !s.DataFine.HasValue
        //            orderby s.Denominazione
        //            select s).ToList();
        //}

        public List<vwStruttura> GetStruttureBuff(string codRegione, string codAzienda, string codTipoStruttura, bool mostraCessate, string pubblicaPrivata, DateTime? dataScheda, bool inConvenzione)
        {
            return (from s in db.vwStruttura
                    where s.CodRegione == codRegione
                      && s.CodAzienda == codAzienda
                      && (String.IsNullOrEmpty(codTipoStruttura) || s.CodiceTipologiaStruttura == codTipoStruttura)
                      && (String.IsNullOrEmpty(pubblicaPrivata) || (s.PubblicoPrivato == pubblicaPrivata))
                      && (
                            (mostraCessate && !dataScheda.HasValue && s.DataFine.HasValue) // && s.DataChiusura.HasValue)
                            ||
                            (!dataScheda.HasValue && !s.DataFine.HasValue && !s.DataChiusura.HasValue)
                            ||
                            (dataScheda.HasValue && (s.DataRiferimentoStruttura <= dataScheda.Value && (!s.DataFine.HasValue || s.DataFine.Value >= dataScheda)))
                      )
                      && (!inConvenzione || s.InConvenzione == inConvenzione)
                    orderby s.Denominazione
                    select s).ToList();
        }

        //public List<vwStruttura> GetStruttureBuffUtente(string userName, string codRegione, string codAzienda)
        //{
        //    var strutture = (from u in db.Utente
        //                     from us in db.UtenteStruttura.Where(x => x.idUtente == u.id)
        //                     from s in db.vwStruttura.Where(x => x.CodRegione == us.codRegione
        //                            && x.CodAzienda == us.codAzienda
        //                            && x.IdStrutturaErogatrice == us.idStrutturaErogatrice
        //                            && x.idWebServiceStruttura == us.idWebServiceStruttura
        //                            && !x.DataChiusura.HasValue && !x.DataFine.HasValue
        //                        )
        //                         //from ts in db.vwTipologiaStruttura.Where(x => x.CodTipologia == s.CodiceTipologiaStruttura)
        //                     where u.username == userName
        //                            && s.CodRegione == codRegione
        //                            && s.CodAzienda == codAzienda
        //                     //&& ts.IsAttivo == true
        //                     select new
        //                     {
        //                         s.CodRegione,
        //                         s.KeyAzienda,
        //                         s.CodAzienda,
        //                         s.IdStrutturaErogatrice,
        //                         s.idWebServiceStruttura,
        //                         s.KeyStruttura,
        //                         s.Denominazione
        //                     }).OrderBy(x => x.Denominazione).Distinct();

        //    List<vwStruttura> listaStrutture = new List<vwStruttura>();
        //    foreach (var item in strutture)
        //    {
        //        listaStrutture.Add(new vwStruttura()
        //        {
        //            CodRegione = item.CodRegione,
        //            KeyAzienda = item.KeyAzienda,
        //            CodAzienda = item.CodAzienda,
        //            IdStrutturaErogatrice = item.IdStrutturaErogatrice,
        //            idWebServiceStruttura = item.idWebServiceStruttura,
        //            KeyStruttura = item.KeyStruttura,
        //            Denominazione = item.Denominazione
        //        });
        //    }

        //    return listaStrutture;
        //}
        public List<vwReparto> GetRepartiByTipoStruttura(string codRegione, string codAzienda, string codTipoStruttura, int? idDisciplinaArea, bool mostraTutti)
        {
            IQueryable<vwReparto> reparti = db.vwReparto.AsQueryable();

            if (!String.IsNullOrEmpty(codRegione))
                reparti = reparti.Where(x => x.CodRegione == codRegione);

            if (!String.IsNullOrEmpty(codAzienda))
                reparti = reparti.Where(x => x.CodAzienda == codAzienda);

            if (!String.IsNullOrEmpty(codTipoStruttura))
                reparti = reparti.Where(x => x.CodiceTipologiaStruttura == codTipoStruttura);

            if (idDisciplinaArea.HasValue)
                reparti = reparti.Where(x => x.codAreaDisciplina == idDisciplinaArea);

            if (!mostraTutti)
                reparti = reparti.Where(x => !x.isStorico.Value && !x.Cancellato.Value);

            return reparti.Distinct().OrderBy(x => x.Nome).ToList();
        }
        public List<vwReparto> GetReparti(string codRegione, string codAzienda, string codStruttura, bool forStorico, bool mostraTutti)
        {
            return db.vwReparto.Where(x => x.CodRegione == codRegione
                                    && x.CodAzienda == codAzienda
                                    && codStruttura == x.keyStruttura
                                    && (mostraTutti || (!x.isStorico.Value && !x.Cancellato.Value))
                                   ).Distinct().OrderBy(x => x.Nome).ToList();

            //return db.vwReparto.Where(x => x.CodRegione == codRegione
            //                        && x.CodAzienda == codAzienda
            //                        && (forStorico ? (codStruttura == x.keyStrutturaData) : (!x.isStorico.Value && codStruttura == x.keyStruttura))
            //                        && (mostraTutti || (!x.isStorico.Value && !x.Cancellato.Value))
            //                       ).Distinct().OrderBy(x => x.Nome).ToList();
        }
        public List<vwReparto> GetRepartiUtente(string userName, string codRegione, string codAzienda, string codStruttura)
        {
            var reparti = (from u in db.Utente
                           from us in db.UtenteStruttura.Where(x => x.idUtente == u.id)
                           from s in db.vwReparto.Where(x => x.IdReparto == us.idReparto && x.IdWebServiceReparto == us.idWebServiceReparto)
                           where u.username == userName && s.IdReparto == us.idReparto
                              && (codRegione == "" || s.CodRegione == codRegione)
                              && (codAzienda == "" || s.CodAzienda == codAzienda)
                              && (codStruttura == "" || codStruttura == s.keyStruttura)
                           select new
                           {
                               s.IdReparto,
                               s.CodRegione,
                               s.CodAzienda,
                               s.IdStrutturaErogatrice,
                               s.idWebServiceStruttura,
                               s.Nome
                           }).OrderBy(x => x.Nome).Distinct();

            List<vwReparto> listaReparti = new List<vwReparto>();
            foreach (var item in reparti)
            {
                listaReparti.Add(new vwReparto()
                {
                    IdReparto = item.IdReparto,
                    CodRegione = item.CodRegione,
                    CodAzienda = item.CodAzienda,
                    IdStrutturaErogatrice = item.IdStrutturaErogatrice,
                    idWebServiceStruttura = item.idWebServiceStruttura,
                    Nome = item.Nome
                });
            }

            return listaReparti;
        }

        public List<StatoSessione> GetStatiSessione()
        {
            return db.StatoSessione.Where(x => x.id > 1 && x.id < 5).ToList();
        }

        public List<TipologiaStruttura> GetTipiStruttureAttive()
        {
            return db.TipologiaStruttura.Where(x => x.IsAttivo == true).OrderBy(x => x.Ordinale).ToList();
        }

        //metodo usato nella pagina di ricerca dei reparti per uniformarlo a quello delle altre ricerche
        public List<vwReparto> GetReparti(int skip, int take, int idUtente, string codRegione, string codAzienda, string codTipoStruttura, string codStruttura, string keyReparto, bool? cancellato, out int totaleReparti)
        {
            Utente utente = db.Utente.Find(idUtente);

            List<String> regioni = new List<String>();
            List<String> aziende = new List<string>();
            List<String> strutture = new List<string>();
            List<String> listaReparti = new List<String>();
            foreach (UtenteStruttura us in utente.struttureAttive)
            {
                if (utente.idRuolo == (int)Ruolo.Regionale && string.IsNullOrEmpty(codRegione))
                    regioni.Add(us.codRegione);
                if (utente.idRuolo == (int)Ruolo.Aziendale)
                {
                    if (string.IsNullOrEmpty(codAzienda))
                        aziende.Add(us.codAzienda);
                    strutture.AddRange((from s in GetStruttureBuff(us.codRegione, us.codAzienda, null, true, null, null, true)
                                        select s.KeyStruttura).ToList());
                }
                if (utente.idRuolo == (int)Ruolo.ReferenteStruttura && string.IsNullOrEmpty(codStruttura))
                    strutture.Add(us.CodiceStruttura);
                if (utente.idRuolo == (int)Ruolo.Osservatore && string.IsNullOrEmpty(keyReparto))
                    listaReparti.Add(us.KeyReparto);
            };

            IQueryable<vwReparto> reparti = db.vwReparto.AsQueryable();

            if (!string.IsNullOrEmpty(keyReparto))
            {
                reparti = db.vwReparto.Where(x => x.KeyReparto == keyReparto);
            }
            else
            {
                if (!string.IsNullOrEmpty(codRegione))
                    reparti = reparti.Where(x => x.CodRegione == codRegione);
                else
                    reparti = reparti.Where(x => regioni.Count == 0 || regioni.Contains(x.CodRegione));

                if (!string.IsNullOrEmpty(codAzienda))
                    reparti = reparti.Where(x => x.CodAzienda == codAzienda);
                else
                    reparti = reparti.Where(x => aziende.Count == 0 || aziende.Contains(x.CodAzienda));

                if (!string.IsNullOrEmpty(codTipoStruttura))
                    reparti = reparti.Where(x => x.CodiceTipologiaStruttura == codTipoStruttura);

                if (!string.IsNullOrEmpty(codStruttura))
                    reparti = reparti.Where(x => codStruttura == x.keyStruttura);
                else
                    reparti = reparti.Where(x => strutture.Count == 0 || strutture.Contains(x.keyStruttura));

                if (cancellato.HasValue)
                {
                    reparti = reparti.Where(x => x.Cancellato == cancellato.Value);
                }
            }

            reparti = reparti.Where(x => !x.isStorico.Value);

            var result = reparti.OrderBy(x => x.Nome).ToList();
            totaleReparti = result.Count();
            result = result.Skip(skip).Take(take).ToList();

            return result;
        }


        public List<Scheda> GetSchede(int skip, int take, int idUtente, string codRegione, string codAzienda, string codTipoStruttura, string keyStruttura, string keyReparto, int? statoSessione, DateTime? dataInizio, DateTime? dataFine, bool? mostraCancellate, int? idScheda, out int totaleSchede)
        {
            Utente utente = db.Utente.Find(idUtente);

            List<String> regioni = new List<String>();
            List<String> aziende = new List<String>();
            List<String> strutture = new List<String>();
            List<String> reparti = new List<String>();
            foreach (UtenteStruttura us in utente.UtenteStruttura)
            {
                if (utente.idRuolo == (int)Ruolo.Regionale && string.IsNullOrEmpty(codRegione))
                    regioni.Add(us.codRegione);
                if (utente.idRuolo == (int)Ruolo.Aziendale)
                {
                    if (string.IsNullOrEmpty(codAzienda))
                        aziende.Add(us.codAzienda);
                    strutture.AddRange((from s in GetStruttureBuff(us.codRegione, us.codAzienda, null, true, null, null, true)
                                        select s.KeyStruttura).ToList());
                }
                if (utente.idRuolo == (int)Ruolo.ReferenteStruttura && string.IsNullOrEmpty(keyStruttura))
                    strutture.Add(us.CodiceStruttura);
                if (utente.idRuolo == (int)Ruolo.Osservatore && string.IsNullOrEmpty(keyReparto))
                    reparti.Add(us.KeyReparto);
            };

            DateTime? dataOraFine = dataFine;
            if (dataOraFine.HasValue)
                dataOraFine = dataOraFine.Value.AddDays(1).AddMilliseconds(-1);

            var result = (from s in db.Scheda
                          from r in db.vwReparto.Where(x => x.IdReparto == s.idReparto)
                          where
                              ((codRegione == "" && (regioni.Count == 0 || regioni.Contains(r.CodRegione))) || r.CodRegione == codRegione)
                              && ((codAzienda == "" && (aziende.Count == 0 || aziende.Contains(r.CodAzienda))) || r.CodAzienda == codAzienda)
                              && (codTipoStruttura == "" || r.CodiceTipologiaStruttura == codTipoStruttura)
                              && ((keyStruttura == "" && (strutture.Count == 0 || strutture.Contains(r.keyStruttura))) || r.keyStruttura == keyStruttura)
                              && ((keyReparto == "" && (reparti.Count == 0 || reparti.Contains(r.KeyReparto))) || r.KeyReparto == keyReparto)
                              && (utente.idRuolo != (int)Ruolo.Osservatore || (utente.idRuolo == (int)Ruolo.Osservatore && s.idUtente == idUtente))
                              && (
                                    !statoSessione.HasValue || (statoSessione.HasValue && s.idStatoSessione == statoSessione)
                                  //!statoSessione.HasValue && (mostraCancellate.HasValue && mostraCancellate == true)
                                  //|| (!statoSessione.HasValue && (!mostraCancellate.HasValue || mostraCancellate == false) && s.idStatoSessione != (int)StatoSessione.Stato.Cancellata)
                                  //|| (statoSessione.HasValue && s.idStatoSessione == statoSessione)

                                  )
                              && (!dataInizio.HasValue || dataInizio.Value <= s.data)
                              && (!dataOraFine.HasValue || dataOraFine.Value >= s.data)
                              && (!idScheda.HasValue || idScheda.Value == s.id)
                          select new
                          {
                              s.id,
                              s.durataSessione,
                              s.idReparto,
                              s.idWebServiceReparto,
                              s.idUtente,
                              s.idStatoSessione,
                              s.data,
                              s.dataInserimento
                          });

            totaleSchede = result.Distinct().Count();

            var resultOrder = result.Distinct().OrderByDescending(x => x.data).Skip(skip).Take(take).ToList();

            List<Scheda> schede = new List<Scheda>();
            foreach (var item in resultOrder)
            {
                schede.Add(new Scheda()
                {
                    id = item.id,
                    durataSessione = item.durataSessione,
                    idReparto = item.idReparto,
                    idWebServiceReparto = item.idWebServiceReparto,
                    idUtente = item.idUtente,
                    idStatoSessione = item.idStatoSessione,
                    data = item.data,
                    dataInserimento = item.dataInserimento,

                    Reparto = db.vwReparto.First(x => x.IdReparto == item.idReparto && x.IdWebServiceReparto == item.idWebServiceReparto),
                    StatoSessione = db.StatoSessione.Find(item.idStatoSessione),
                    Utente = db.Utente.Find(item.idUtente)
                });
            }


            return schede;
        }

        public List<Utente> GetUtenti(int skip, int take, int idUtente, Ruolo ruoloUtenteLoggato, string cognome, string nome, int? idRuolo, bool? Cancellate, string codRegione, string codAzienda, string codTipoStruttura, string codStruttura, string keyReparto, out int totaleUtenti)
        {

            Utente utente = db.Utente.Find(idUtente);

            List<String> regioni = new List<String>();
            List<String> aziende = new List<string>();
            List<String> strutture = new List<string>();
            List<String> listaReparti = new List<String>();
            foreach (UtenteStruttura us in utente.struttureAttive)
            {
                if (utente.idRuolo == (int)Ruolo.Regionale && string.IsNullOrEmpty(codRegione))
                    regioni.Add(us.codRegione);
                if (utente.idRuolo == (int)Ruolo.Aziendale)
                {
                    if (string.IsNullOrEmpty(codAzienda))
                        aziende.Add(us.codAzienda);
                    strutture.AddRange((from s in GetStruttureBuff(us.codRegione, us.codAzienda, null, true, null, null, true)
                                        select s.KeyStruttura).ToList());
                }
                if (utente.idRuolo == (int)Ruolo.ReferenteStruttura && string.IsNullOrEmpty(codStruttura))
                    strutture.Add(us.CodiceStruttura);
                if (utente.idRuolo == (int)Ruolo.Osservatore && string.IsNullOrEmpty(keyReparto))
                    listaReparti.Add(us.KeyReparto);
            };

            List<Utente> elencoUtenti = new List<Utente>();
            var UserJoin = from u in db.Utente
                           join us in db.UtenteStruttura on u.id equals us.idUtente into utenteUtenteStruttura
                           from uus in utenteUtenteStruttura.DefaultIfEmpty()
                           select new { u, uus };

            UserJoin = UserJoin.Where(x => x.u.attivato);

            if (!string.IsNullOrEmpty(cognome))
                UserJoin = UserJoin.Where(x => x.u.cognome.Contains(cognome));

            if (!string.IsNullOrEmpty(nome))
                UserJoin = UserJoin.Where(x => x.u.nome.Contains(nome));

            if (idRuolo.HasValue)
            {
                UserJoin = UserJoin.Where(x => x.u.idRuolo == idRuolo);
            }
            else
            {
                if (ruoloUtenteLoggato == Ruolo.Regionale)
                {
                    UserJoin = UserJoin.Where(x => x.u.idRuolo != (int)Ruolo.Regionale);
                }
                else if (ruoloUtenteLoggato == Ruolo.Aziendale)
                {
                    UserJoin = UserJoin.Where(x => x.u.idRuolo != (int)Ruolo.Regionale && x.u.idRuolo != (int)Ruolo.Aziendale);
                }
                else if (ruoloUtenteLoggato == Ruolo.ReferenteStruttura)
                {
                    UserJoin = UserJoin.Where(x => x.u.idRuolo != (int)Ruolo.Regionale && x.u.idRuolo != (int)Ruolo.Aziendale && x.u.idRuolo != (int)Ruolo.ReferenteStruttura);
                }
            }

            if (Cancellate.HasValue)
            {
                UserJoin = UserJoin.Where(x => x.u.cancellato == Cancellate.Value);
            }

            if (!string.IsNullOrEmpty(codRegione))
                UserJoin = UserJoin.Where(x => x.uus.codRegione == codRegione || (x.u.idRuolo == (int)Ruolo.NonAssociato));
            else
                UserJoin = UserJoin.Where(x => regioni.Count == 0 || regioni.Contains(x.uus.codRegione) || (x.u.idRuolo == (int)Ruolo.NonAssociato));

            if (!string.IsNullOrEmpty(codAzienda))
                UserJoin = UserJoin.Where(x => x.uus.codAzienda == codAzienda || (x.u.idRuolo == (int)Ruolo.NonAssociato));
            else
                UserJoin = UserJoin.Where(x => aziende.Count == 0 || aziende.Contains(x.uus.codAzienda) || (x.u.idRuolo == (int)Ruolo.NonAssociato));

            if (!string.IsNullOrEmpty(codStruttura))
            {
                int idStruttura = Convert.ToInt32(codStruttura.Split('.')[0]);
                int idWs = Convert.ToInt32(codStruttura.Split('.')[1]);
                UserJoin = UserJoin.Where(x => x.uus.idStrutturaErogatrice == idStruttura && x.uus.idWebServiceStruttura == idWs || (x.u.idRuolo == (int)Ruolo.NonAssociato));
            }
            else
                UserJoin = UserJoin.Where(x => strutture.Count == 0 || strutture.Contains(x.uus.idStrutturaErogatrice + "." + x.uus.idWebServiceStruttura) || (x.u.idRuolo == (int)Ruolo.NonAssociato));

            if (!string.IsNullOrEmpty(keyReparto))
            {
                int idReparto = Convert.ToInt32(keyReparto.Split('.')[0]);
                int idWs = Convert.ToInt32(codStruttura.Split('.')[1]);
                UserJoin = UserJoin.Where(x => x.uus.idReparto == idReparto && x.uus.idWebServiceReparto == idWs);
            }
            else
                UserJoin = UserJoin.Where(x => listaReparti.Count == 0 || listaReparti.Contains(x.uus.idReparto + "." + x.uus.idWebServiceReparto));


            totaleUtenti = UserJoin.Select(x => x.u).Distinct().Count();
            elencoUtenti = UserJoin.Select(x => x.u).Distinct().OrderBy(x => x.nome).OrderBy(x => x.cognome).Skip(skip).Take(take).ToList();

            return elencoUtenti;
        }

        public int CountOpportunita(int idScheda)
        {
            int count = (from os in db.Osservazione
                         from op in db.Opportunita.Where(op => os.id == op.idOsservazione)
                         where os.idScheda == idScheda
                         select op.id).Count();

            return count;
        }
        public int CountSoggetti(int idScheda)
        {
            var result = (from o in db.Osservazione
                          where o.idScheda == idScheda
                          select o.numOperatori);

            int count = 0;
            count = result.ToList().Sum();

            return count;
        }

        public int CountAdesioni(int idScheda)
        {
            int count = (from os in db.Osservazione
                         from op in db.Opportunita.Where(op => os.id == op.idOsservazione)
                         where os.idScheda == idScheda && (op.idAzione == 1 || op.idAzione == 2)
                         select os.id).Count();

            return count;
        }

        public int CountAdesioni(int idScheda, int idIndicazione)
        {
            int count = (from os in db.Osservazione
                         from op in db.Opportunita.Where(op => os.id == op.idOsservazione)
                         where os.idScheda == idScheda && op.idIndicazione == idIndicazione && (op.idAzione == 1 || op.idAzione == 2)
                         select os.id).Count();

            return count;
        }

        public int CountNonAdesioni(int idScheda)
        {
            int count = (from os in db.Osservazione
                         from op in db.Opportunita.Where(op => os.id == op.idOsservazione)
                         where os.idScheda == idScheda && (op.idAzione == 3 || op.idAzione == 4)
                         select os.id).Count();

            return count;
        }
        public int CountNonAdesioni(int idScheda, int idIndicazione)
        {
            int count = (from os in db.Osservazione
                         from op in db.Opportunita.Where(op => os.id == op.idOsservazione)
                         where os.idScheda == idScheda && op.idIndicazione == idIndicazione && (op.idAzione == 3 || op.idAzione == 4)
                         select os.id).Count();

            return count;
        }

        public Scheda GetScheda(int idScheda)
        {
            return db.Scheda.Find(idScheda);
        }

        public vwReparto GetReparto(int idReparto, int idWsReparto)
        {
            return db.vwReparto.FirstOrDefault(x => x.IdReparto == idReparto && x.IdWebServiceReparto == idWsReparto && !x.isStorico.Value);
        }

        public vwReparto GetReparto(string keyReparto)
        {
            return db.vwReparto.FirstOrDefault(x => x.KeyReparto == keyReparto && !x.isStorico.Value);
        }

        public vwReparto GetReparto(int idReparto)
        {
            return db.vwReparto.FirstOrDefault(x => x.IdReparto == idReparto && !x.isStorico.Value);
        }

        public void Modified(Scheda scheda)
        {
            db.Entry(scheda).State = EntityState.Modified;
        }

        public long SaveScheda(Scheda scheda)
        {
            if (scheda.id > 0)
            {
                //db..Attach(scheda);
                //db.ObjectStateManager.ChangeObjectState(scheda, EntityState.Modified);
                //db.Entry(scheda).State = EntityState.Modified;
            }
            else
                db.Scheda.Add(scheda);

            db.SaveChanges();
            return scheda.id;
        }
        public long SaveReparto(vwReparto reparto)
        {
            try
            {
                return db.Reparto_Salva(reparto.IdReparto, reparto.IdWebServiceReparto, reparto.IdStrutturaErogatrice, reparto.DataRiferimentoStruttura, reparto.DataInizio, reparto.DataFine, reparto.idWebServiceStruttura, reparto.CodDisciplina, reparto.ProgressivoDivisione, reparto.Nome, reparto.Descrizione, reparto.codAreaDisciplina, reparto.Cancellato);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }
        public void Modified(UtenteDaCensire utente)
        {
            db.Entry(utente).State = EntityState.Modified;
        }
        public long SaveUtenteDaCensire(UtenteDaCensire u)
        {
            try
            {
                if (u.Id > 0)
                { }
                else
                    db.UtenteDaCensire.Add(u);
                db.SaveChanges();
                return u.Id;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }
        public void Modified(Utente utente)
        {
            db.Entry(utente).State = EntityState.Modified;
        }
        public long SaveUtente(Utente utente)
        {
            try
            {
                db.UtenteStruttura.RemoveRange(db.UtenteStruttura.Where(x => x.idUtente == utente.id).ToList());

                foreach (UtenteStruttura us in utente.UtenteStruttura)
                {
                    db.UtenteStruttura.Add(us);
                }

                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();

                return utente.id;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

        }

        public Osservazione GetOsservazione(long id)
        {
            return db.Osservazione.Find(id);
        }

        public List<Operatore> GetOperatori()
        {
            return db.Operatore.OrderBy(x => x.codCategoria).ToList();
        }

        public void Modified(Osservazione osservazione)
        {
            db.Entry(osservazione).State = EntityState.Modified;
        }

        public long SaveOsservazione(Osservazione osservazione)
        {
            if (osservazione.id > 0)
            { }
            else
                db.Osservazione.Add(osservazione);
            db.SaveChanges();

            return osservazione.id;
        }

        public void Modified(Opportunita opportunita)
        {
            db.Entry(opportunita).State = EntityState.Modified;
        }
        public long SaveOpportunita(Opportunita opportunita)
        {
            if (opportunita.id > 0)
            { }
            else
                db.Opportunita.Add(opportunita);
            db.SaveChanges();

            return opportunita.id;
        }

        public Utente GetUtente(string userName)
        {
            Utente utente = db.Utente.Where(x => x.username == userName && x.attivato).FirstOrDefault();
            return utente;
        }

        public Utente GetUtente(int ID)
        {
            Utente utente = db.Utente.Find(ID);
            if (!utente.attivato)
                throw new ErroreInterno("Utente non attivo");
            return utente;
        }

        public bool CodiceFiscaleAssociatoAdUtente(int idUtente, string codiceFiscale)
        {
            return db.Utente.Any(x => x.attivato && !x.cancellato && x.CodiceFiscale.Equals(codiceFiscale, StringComparison.InvariantCultureIgnoreCase) && x.id != idUtente);
        }

        public UtenteStruttura GetUtenteStruttura(int ID)
        {
            return db.UtenteStruttura.Find(ID);
        }

        public void SetStatoScheda(int idScheda, StatoSessione.Stato stato)
        {
            Scheda scheda = db.Scheda.Find(idScheda);
            scheda.idStatoSessione = (int)stato;
            scheda.dataUltimaModificaStato = DateTime.Now;
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();
        }

        public void DeleteOsservazioneOpportunita(int idOsservazione)
        {
            try
            {
                DeleteOpportunitaByIdOsservazione(idOsservazione);
                DeleteOsservazione(idOsservazione);
            }
            catch (Exception ex)
            {
                throw new ErroreInterno("Impossibile eliminare");
            }
        }

        public void DeleteOsservazione(int idOsservazione)
        {
            db.Osservazione.Remove(db.Osservazione.Find(idOsservazione));
            db.SaveChanges();
        }

        public void DeleteOpportunitaByIdOsservazione(int idOsservazione)
        {
            db.Opportunita.RemoveRange(db.Opportunita.Where(x => x.idOsservazione == idOsservazione));
            db.SaveChanges();
        }

        public void DeleteOpportunita(int idOpportunita)
        {
            db.Opportunita.Remove(db.Opportunita.Find(idOpportunita));
            db.SaveChanges();
        }

        public List<Bacteria> GetBacteri()
        {
            return db.Bacteria.OrderBy(x => x.ordinale).ToList();
        }

        public Bacteria GetBacteria(string code)
        {
            return db.Bacteria.Find(code);
        }

        public List<Indicazione> GetIndicazioni()
        {
            return db.Indicazione.OrderBy(x => x.ordinale).ToList();
        }

        public Opportunita GetOpportunita(long id)
        {
            return db.Opportunita.Find(id);
        }

        public List<Opportunita> GetOpportunitaByOsservazione(long idOsservazione)
        {
            return db.Opportunita.Where(x => x.idOsservazione == idOsservazione).ToList();
        }

        public IEnumerable<fn_TabellaOsservazioni_Result> GetTabellaOsservazioni(int? idScheda, int? idOsservazione)
        {
            List<fn_TabellaOsservazioni_Result> tabella = db.fn_TabellaOsservazioni(idScheda, idOsservazione).ToList();
            tabella.Add(new fn_TabellaOsservazioni_Result()
            {
                Azione = "Totale",
                Prima_contatto_paziente = tabella.Sum(x => x.Prima_contatto_paziente),
                Prima_di_manovra_di_asepsi = tabella.Sum(x => x.Prima_di_manovra_di_asepsi),
                Dopo_contatto_fluido = tabella.Sum(x => x.Dopo_contatto_fluido),
                Dopo_contatto_paziente = tabella.Sum(x => x.Dopo_contatto_paziente),
                Dopo_contatto_ambiente_ = tabella.Sum(x => x.Dopo_contatto_ambiente_)
            });
            return tabella;
        }
        public IEnumerable<fn_TabellaAdesioni_Result> GetTabellaAdesioni(int? idScheda, int? idOsservazione)
        {
            List<fn_TabellaAdesioni_Result> tabella = db.fn_TabellaAdesioni(idScheda, idOsservazione).ToList();
            return tabella;
        }

        public void UpdatePosizione(string username, string url, string datiRicerca)
        {
            UltimaPosizione posizione = db.UltimaPosizione.FirstOrDefault(x => x.username == username);

            if (posizione == null)
            {
                posizione = new UltimaPosizione
                {
                    data = DateTime.Now,
                    username = username,
                    url = url,
                    datiRicerca = datiRicerca
                };
                db.UltimaPosizione.Add(posizione);
            }
            else
            {
                posizione.data = DateTime.Now;
                posizione.url = url;
                if (String.IsNullOrEmpty(datiRicerca))
                    posizione.datiRicerca = datiRicerca;
                db.Entry(posizione).State = EntityState.Modified;
            }

            db.SaveChanges();
        }

        public UltimaPosizione GetUltimaPosizione(string username)
        {
            return db.UltimaPosizione.FirstOrDefault(x => x.username == username);
        }

        public void InsertUpdateLog(string username, string message, string url)
        {
            Log log = new Log
            {
                type = 4,
                data = DateTime.Now,
                user = username,
                message = message,
                request = url
            };
            db.Log.Add(log);
            db.SaveChanges();
        }
        public void SetRepartoCancellato(string keyReparto)
        {
            vwReparto r = db.vwReparto.First(x => x.KeyReparto == keyReparto);
            db.Reparto_Salva(r.IdReparto, r.IdWebServiceReparto, r.IdStrutturaErogatrice, r.DataRiferimentoStruttura, r.DataInizio, r.DataFine, r.idWebServiceStruttura, r.CodDisciplina, r.ProgressivoDivisione, r.Nome, r.Descrizione, r.codAreaDisciplina, true);

        }
        public List<Ruoli> GetRuoli(int ruoloPartenza)
        {
            Ruoli ruolo = db.Ruoli.Find(ruoloPartenza);
            return db.Ruoli.Where(x => x.ordinale > ruolo.ordinale).OrderBy(x => x.ordinale).ToList();
        }

        public long SaveUtenteStruttura(UtenteStruttura utenteStruttura)
        {

            if (utenteStruttura.ID > 0)
            {
                UtenteStruttura u = db.UtenteStruttura.Find(utenteStruttura.ID);
                u.dataDal = utenteStruttura.dataDal;
                u.dataAl = utenteStruttura.dataAl;
                db.Entry(u).State = EntityState.Modified;
            }
            else
                db.UtenteStruttura.Add(utenteStruttura);

            db.SaveChanges();

            return utenteStruttura.ID;
        }

        public void DeleteUtenteStruttura(int id)
        {
            db.UtenteStruttura.Remove(db.UtenteStruttura.Find(id));
            db.SaveChanges();
        }

        public List<EsportaOpportunita_Result> GetEsportaOpportunita(int IdUtente, string codRegione, string codAzienda, string codTipoStruttura, string keyStruttura, string keyReparto, int? StatoSessione, bool? Cancellate, DateTime? DataDal, DateTime? DataAl, int? idScheda)
        {
            Utente utente = db.Utente.Find(IdUtente);

            List<string> regioni = new List<string>();
            List<string> aziende = new List<string>();
            List<string> strutture = new List<string>();
            List<string> reparti = new List<string>();
            foreach (UtenteStruttura us in utente.UtenteStruttura)
            {
                if (utente.idRuolo == (int)Ruolo.Regionale && string.IsNullOrEmpty(codRegione)) regioni.Add(us.codRegione);
                if (utente.idRuolo == (int)Ruolo.Aziendale && string.IsNullOrEmpty(codAzienda)) aziende.Add(us.codAzienda);
                if (utente.idRuolo == (int)Ruolo.ReferenteStruttura && string.IsNullOrEmpty(keyStruttura)) strutture.Add(us.CodiceStruttura);
                if (utente.idRuolo == (int)Ruolo.Osservatore && string.IsNullOrEmpty(keyReparto)) reparti.Add(us.KeyReparto);
            };

            return db.EsportaOpportunita(codRegione, codAzienda, codTipoStruttura, keyStruttura, keyReparto, StatoSessione, DataDal, DataAl, Cancellate, idScheda)
                .Where(x => (regioni.Count == 0 || regioni.Contains(x.codiceRegione))
                             && (aziende.Count == 0 || aziende.Contains(x.codiceAzienda))
                             && (strutture.Count == 0 || strutture.Contains(x.KeyStruttura))
                             && (reparti.Count == 0 || reparti.Contains(x.KeyReparto))
                                     ).ToList();
        }

        public List<StatoCandidatura> GetStatoCandidatura()
        {
            return db.StatoCandidatura.Where(x => x.Codice < 3).ToList();
        }

    }
}