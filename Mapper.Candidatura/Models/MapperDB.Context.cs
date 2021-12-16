﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mapper.Candidatura.Models
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
    
        public virtual DbSet<vwAreaDisciplina> vwAreaDisciplina { get; set; }
        public virtual DbSet<vwAzienda> vwAzienda { get; set; }
        public virtual DbSet<vwRegione> vwRegione { get; set; }
        public virtual DbSet<vwReparto> vwReparto { get; set; }
        public virtual DbSet<vwStruttura> vwStruttura { get; set; }
        public virtual DbSet<StatoCandidatura> StatoCandidatura { get; set; }
        public virtual DbSet<Ruoli> Ruoli { get; set; }
        public virtual DbSet<UtenteDaCensire> UtenteDaCensire { get; set; }
        public virtual DbSet<Utente> Utente { get; set; }
        public virtual DbSet<UtenteStruttura> UtenteStruttura { get; set; }
        public virtual DbSet<TipologiaStruttura> TipologiaStruttura { get; set; }
    
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
    }
}