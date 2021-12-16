﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mapper.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Azione> Azione { get; set; }
        public virtual DbSet<Bacteria> Bacteria { get; set; }
        public virtual DbSet<Indicazione> Indicazione { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<LogType> LogType { get; set; }
        public virtual DbSet<Opportunita> Opportunita { get; set; }
        public virtual DbSet<Osservazione> Osservazione { get; set; }
        public virtual DbSet<Ruoli> Ruoli { get; set; }
        public virtual DbSet<StatoCandidatura> StatoCandidatura { get; set; }
        public virtual DbSet<StatoSessione> StatoSessione { get; set; }
        public virtual DbSet<UltimaPosizione> UltimaPosizione { get; set; }
        public virtual DbSet<vwRegione> vwRegione { get; set; }
        public virtual DbSet<vwTipoAnagrafeRegionale> vwTipoAnagrafeRegionale { get; set; }
        public virtual DbSet<vwAreaDisciplina> vwAreaDisciplina { get; set; }
        public virtual DbSet<Scheda> Scheda { get; set; }
        public virtual DbSet<Operatore> Operatore { get; set; }
        public virtual DbSet<vwAzienda> vwAzienda { get; set; }
        public virtual DbSet<Utente> Utente { get; set; }
        public virtual DbSet<UtenteDaCensire> UtenteDaCensire { get; set; }
        public virtual DbSet<vwReparto> vwReparto { get; set; }
        public virtual DbSet<vwStruttura> vwStruttura { get; set; }
        public virtual DbSet<TipologiaStruttura> TipologiaStruttura { get; set; }
        public virtual DbSet<UtenteStruttura> UtenteStruttura { get; set; }
    
        [DbFunction("Entities", "fn_TabellaOsservazioni")]
        public virtual IQueryable<fn_TabellaOsservazioni_Result> fn_TabellaOsservazioni(Nullable<int> idScheda, Nullable<int> idOsservazione)
        {
            var idSchedaParameter = idScheda.HasValue ?
                new ObjectParameter("idScheda", idScheda) :
                new ObjectParameter("idScheda", typeof(int));
    
            var idOsservazioneParameter = idOsservazione.HasValue ?
                new ObjectParameter("idOsservazione", idOsservazione) :
                new ObjectParameter("idOsservazione", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_TabellaOsservazioni_Result>("[Entities].[fn_TabellaOsservazioni](@idScheda, @idOsservazione)", idSchedaParameter, idOsservazioneParameter);
        }
    
        [DbFunction("Entities", "fn_TabellaAdesioni")]
        public virtual IQueryable<fn_TabellaAdesioni_Result> fn_TabellaAdesioni(Nullable<int> idScheda, Nullable<int> idOsservazione)
        {
            var idSchedaParameter = idScheda.HasValue ?
                new ObjectParameter("idScheda", idScheda) :
                new ObjectParameter("idScheda", typeof(int));
    
            var idOsservazioneParameter = idOsservazione.HasValue ?
                new ObjectParameter("idOsservazione", idOsservazione) :
                new ObjectParameter("idOsservazione", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_TabellaAdesioni_Result>("[Entities].[fn_TabellaAdesioni](@idScheda, @idOsservazione)", idSchedaParameter, idOsservazioneParameter);
        }
    
        public virtual int Reparto_Salva(Nullable<int> idReparto, Nullable<int> idWebServiceReparto, Nullable<int> idStrutturaErogatrice, Nullable<System.DateTime> dataInizioStrutturaErogatrice, Nullable<System.DateTime> dataInizio, Nullable<System.DateTime> dataFine, Nullable<int> idWebServiceStruttura, string codDisciplina, string progressivoDivisione, string nome, string descrizione, Nullable<int> codAreaDisciplina, Nullable<bool> cancellato)
        {
            var idRepartoParameter = idReparto.HasValue ?
                new ObjectParameter("IdReparto", idReparto) :
                new ObjectParameter("IdReparto", typeof(int));
    
            var idWebServiceRepartoParameter = idWebServiceReparto.HasValue ?
                new ObjectParameter("IdWebServiceReparto", idWebServiceReparto) :
                new ObjectParameter("IdWebServiceReparto", typeof(int));
    
            var idStrutturaErogatriceParameter = idStrutturaErogatrice.HasValue ?
                new ObjectParameter("IdStrutturaErogatrice", idStrutturaErogatrice) :
                new ObjectParameter("IdStrutturaErogatrice", typeof(int));
    
            var dataInizioStrutturaErogatriceParameter = dataInizioStrutturaErogatrice.HasValue ?
                new ObjectParameter("DataInizioStrutturaErogatrice", dataInizioStrutturaErogatrice) :
                new ObjectParameter("DataInizioStrutturaErogatrice", typeof(System.DateTime));
    
            var dataInizioParameter = dataInizio.HasValue ?
                new ObjectParameter("DataInizio", dataInizio) :
                new ObjectParameter("DataInizio", typeof(System.DateTime));
    
            var dataFineParameter = dataFine.HasValue ?
                new ObjectParameter("DataFine", dataFine) :
                new ObjectParameter("DataFine", typeof(System.DateTime));
    
            var idWebServiceStrutturaParameter = idWebServiceStruttura.HasValue ?
                new ObjectParameter("IdWebServiceStruttura", idWebServiceStruttura) :
                new ObjectParameter("IdWebServiceStruttura", typeof(int));
    
            var codDisciplinaParameter = codDisciplina != null ?
                new ObjectParameter("CodDisciplina", codDisciplina) :
                new ObjectParameter("CodDisciplina", typeof(string));
    
            var progressivoDivisioneParameter = progressivoDivisione != null ?
                new ObjectParameter("ProgressivoDivisione", progressivoDivisione) :
                new ObjectParameter("ProgressivoDivisione", typeof(string));
    
            var nomeParameter = nome != null ?
                new ObjectParameter("Nome", nome) :
                new ObjectParameter("Nome", typeof(string));
    
            var descrizioneParameter = descrizione != null ?
                new ObjectParameter("Descrizione", descrizione) :
                new ObjectParameter("Descrizione", typeof(string));
    
            var codAreaDisciplinaParameter = codAreaDisciplina.HasValue ?
                new ObjectParameter("CodAreaDisciplina", codAreaDisciplina) :
                new ObjectParameter("CodAreaDisciplina", typeof(int));
    
            var cancellatoParameter = cancellato.HasValue ?
                new ObjectParameter("Cancellato", cancellato) :
                new ObjectParameter("Cancellato", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Reparto_Salva", idRepartoParameter, idWebServiceRepartoParameter, idStrutturaErogatriceParameter, dataInizioStrutturaErogatriceParameter, dataInizioParameter, dataFineParameter, idWebServiceStrutturaParameter, codDisciplinaParameter, progressivoDivisioneParameter, nomeParameter, descrizioneParameter, codAreaDisciplinaParameter, cancellatoParameter);
        }
    
        public virtual ObjectResult<EsportaOpportunita_Result> EsportaOpportunita(string codRegione, string codAzienda, string codTipoStruttura, string keyStruttura, string keyReparto, Nullable<int> statoSessione, Nullable<System.DateTime> dataDal, Nullable<System.DateTime> dataAl, Nullable<bool> cancellate, Nullable<int> idScheda)
        {
            var codRegioneParameter = codRegione != null ?
                new ObjectParameter("codRegione", codRegione) :
                new ObjectParameter("codRegione", typeof(string));
    
            var codAziendaParameter = codAzienda != null ?
                new ObjectParameter("codAzienda", codAzienda) :
                new ObjectParameter("codAzienda", typeof(string));
    
            var codTipoStrutturaParameter = codTipoStruttura != null ?
                new ObjectParameter("codTipoStruttura", codTipoStruttura) :
                new ObjectParameter("codTipoStruttura", typeof(string));
    
            var keyStrutturaParameter = keyStruttura != null ?
                new ObjectParameter("keyStruttura", keyStruttura) :
                new ObjectParameter("keyStruttura", typeof(string));
    
            var keyRepartoParameter = keyReparto != null ?
                new ObjectParameter("keyReparto", keyReparto) :
                new ObjectParameter("keyReparto", typeof(string));
    
            var statoSessioneParameter = statoSessione.HasValue ?
                new ObjectParameter("statoSessione", statoSessione) :
                new ObjectParameter("statoSessione", typeof(int));
    
            var dataDalParameter = dataDal.HasValue ?
                new ObjectParameter("dataDal", dataDal) :
                new ObjectParameter("dataDal", typeof(System.DateTime));
    
            var dataAlParameter = dataAl.HasValue ?
                new ObjectParameter("dataAl", dataAl) :
                new ObjectParameter("dataAl", typeof(System.DateTime));
    
            var cancellateParameter = cancellate.HasValue ?
                new ObjectParameter("cancellate", cancellate) :
                new ObjectParameter("cancellate", typeof(bool));
    
            var idSchedaParameter = idScheda.HasValue ?
                new ObjectParameter("idScheda", idScheda) :
                new ObjectParameter("idScheda", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<EsportaOpportunita_Result>("EsportaOpportunita", codRegioneParameter, codAziendaParameter, codTipoStrutturaParameter, keyStrutturaParameter, keyRepartoParameter, statoSessioneParameter, dataDalParameter, dataAlParameter, cancellateParameter, idSchedaParameter);
        }
    }
}
