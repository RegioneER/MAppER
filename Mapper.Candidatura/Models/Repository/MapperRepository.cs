using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;

using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Web;
using System.Web.UI.WebControls;
using Mapper.Candidatura.Models;
using Mapper.Candidatura.Models.Exceptions;
using Microsoft.Ajax.Utilities;

namespace Mapper.Candidatura.Models.Repository
{
    public class MapperRepository : IRepository
    {
        private Entities db = null;

        public Entities DBContext
        {
            get { return db; }
        }

        public MapperRepository()
        {
            db = new Entities(true);
        }

        public static MapperRepository GetMapperRepository()
        {
            return new MapperRepository();
        }

        public List<vwRegione> GetRegioni()
        {
            return db.vwRegione.OrderBy(x => x.Denominazione).ToList();
        }

        public vwAzienda GetAzienda(string codAzienda, string codregione)
        {
            return db.vwAzienda.Find(codAzienda, codregione);
        }      

        public vwStruttura GetStruttura(string codRegione, string codAzienda, string codMinStruttura, string subCodMinStruttura, string codiceTipologiaStruttura, int? idWebServiceStruttura)
        {
            return db.vwStruttura.FirstOrDefault(x => x.CodRegione == codRegione && x.CodAzienda == codAzienda
                    && x.CodMin == codMinStruttura && x.SubCodMin == subCodMinStruttura && x.CodiceTipologiaStruttura == codiceTipologiaStruttura
                    && x.idWebServiceStruttura == idWebServiceStruttura.Value);
        }

        public vwRegione GetRegione(string codRegione)
        {
            return db.vwRegione.Find(codRegione);
        }       

        public List<vwAzienda> GetAziende(string codRegione)
        {
            return db.vwAzienda.Where(x => x.CodRegione == codRegione && (!x.DataFine.HasValue || (x.DataFine.HasValue && x.DataFine.Value > DateTime.Now))).OrderBy(x => x.Denominazione).ToList();
        }
public List<vwAzienda> GetAziendePubbliche(string codRegione)
        {

            var res = from a in db.vwAzienda
                      join s in db.vwStruttura on a.CodAzienda equals s.CodAzienda
                      where !(s.isPrivata ?? true)
                      select a;

            return res.Where(x => x.CodRegione == codRegione && (!x.DataFine.HasValue || (x.DataFine.HasValue && x.DataFine.Value > DateTime.Now))).OrderBy(x => x.Denominazione).Distinct().ToList();
        }

        public List<vwStruttura> GetvwStrutture(string codRegione, string codAzienda, string codTipoStruttura)
        {
            return db.vwStruttura.Where(x => x.CodRegione == codRegione
                                            && x.CodAzienda == codAzienda
                                            && (String.IsNullOrEmpty(codTipoStruttura) || x.CodiceTipologiaStruttura == codTipoStruttura)
                                            && (!x.DataFine.HasValue || (x.DataFine.HasValue && x.DataFine.Value > DateTime.Now))
                                          ).ToList();
        }       
      public List<vwStruttura> GetvwStrutturaPubbliche(string codRegione, string codAzienda, string codTipoStruttura)
        {
            return GetvwStrutture(codRegione, codAzienda, codTipoStruttura).Where(x => !(x.isPrivata ?? true)).ToList();
        }  

        public List<TipologiaStruttura> GetTipiStruttureAttive()
        {
            var res = from a in db.TipologiaStruttura
                      join s in db.vwStruttura on a.CodTipologia equals s.CodiceTipologiaStruttura
                      where a.IsAttivo && (!s.DataFine.HasValue || (s.DataFine.HasValue && s.DataFine.Value > DateTime.Now))
                      select a;

            return res.Distinct().OrderBy(x => x.Ordinale).ToList();
        }

        public List<TipologiaStruttura> GetTipiStruttureAttivePubbliche(string codRegione)
        {
            var res = from a in db.TipologiaStruttura
                      join s in db.vwStruttura on a.CodTipologia equals s.CodiceTipologiaStruttura
                      where !(s.isPrivata ?? true) && a.IsAttivo && (!s.DataFine.HasValue || (s.DataFine.HasValue && s.DataFine.Value > DateTime.Now)) && s.CodRegione == codRegione
                      select a;

            return res.Distinct().OrderBy(x => x.Ordinale).ToList();
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
    }
}