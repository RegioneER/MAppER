using Mapper.Models.Repository;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mapper.Models
{
    [MetadataType(typeof(UtenteDaCensireMetadata))]
    public partial class UtenteDaCensire
    {
        public enum TipoStato { In_Attesa = 0, Approvata = 1, Rifiutata = 2 }

        private MapperRepository repository = MapperRepository.GetMapperRepository();
      public string NomeRegione
        {
            get
            {
                string _nomeRegione = "";
                if (!String.IsNullOrEmpty(CodRegione))
                {
                    vwRegione regione = repository.GetRegione(CodRegione);
                    if (regione != null)
                        _nomeRegione = regione.Denominazione;
                }

                return _nomeRegione;
            }
        }
        public string NomeAzienda
        {
            get
            {
                string _nomeAzienda = "";
                if (!String.IsNullOrEmpty(CodAzienda))
                {
                    vwAzienda azienda = repository.GetAzienda(CodAzienda, CodRegione);
                    if (azienda != null)
                        _nomeAzienda = azienda.Denominazione;
                }

                return _nomeAzienda;
            }
        }

        public string NomeStruttura
        {
            get
            {
                vwStruttura struttura = repository.GetStrutturaCorrente(KeyStruttura);
                return struttura == null ? "" : struttura.Denominazione;
            }
        }
        public string NomeRuolo
        {
            get
            {
                return ((Ruolo)IdRuolo).ToString();
            }
        }
        public string NomeStato
        {
            get
            {
                switch (IdStato)
                {
                    case 0: return "In attesa di approvazione";
                    case 1: return "Approvata";
                    case 2: return "Rifiutato";
                    default: return "Stato sconosciuto";
                }
            }
        }

    }
    internal sealed class UtenteDaCensireMetadata
    {
        [RegularExpression(@"^[-_a-zA-Z0-9]+(\.[-_a-zA-Z0-9]+)*@[-_a-zA-Z0-9]+(\.[-_a-zA-Z0-9]+)+$", ErrorMessage = "Il formato della Email non è corretto")]
        [Required, MaxLength(255)]
        public string Email { get; set; }
        [Required, MaxLength(50)]
        public string Nome { get; set; }
        [Required, MaxLength(50)]
        public string Cognome { get; set; }
        [Required]
        public string CodiceFiscale { get; set; }
        [Display(Name="Struttura")]
        [Required]
        public string KeyStruttura { get; set; }
    }

    public class UtenteDaCensireManager
    {
        public double UtentiDaCensirePerPagina { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount
        {
            get { return Convert.ToInt32(Math.Ceiling(TotaleUtentiDaCensire / UtentiDaCensirePerPagina)); }
        }
        public int TotaleUtentiDaCensire { get; set; }
        public StaticPagedList<UtenteDaCensire> UtentiDaCensirePaged { get; set; }
    }
}