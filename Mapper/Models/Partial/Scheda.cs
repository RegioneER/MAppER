using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Mapper.Models;
using Mapper.Models.Repository;
using PagedList;
using System.Data;
using System.Web.Mvc;

namespace Mapper.Models
{
    [MetadataType(typeof(SchedaMetadata))]
    public partial class Scheda
    {
        private MapperRepository repository = MapperRepository.GetMapperRepository();

        private int totOpportunita = 0;
        private decimal adesioni = 0;
        private decimal nonadesioni = 0;

        public vwReparto Reparto
        {
            get
            {
                return repository.GetReparto(idReparto, idWebServiceReparto);
            }
            set { }
        }

        public int TotaleOpportunita
        {
            get
            {
                totOpportunita = repository.CountOpportunita(id);
                return totOpportunita;
            }
        }
        public int TotaleSoggetti
        {
            get
            {
                return repository.CountSoggetti(id);
            }
        }
        public decimal Adesioni
        {
            get
            {
                if (totOpportunita > 0)
                    adesioni = repository.CountAdesioni(id) * 100 / totOpportunita;
                return adesioni;
            }
        }
        public decimal NonAdesioni
        {
            get
            {
                if (totOpportunita > 0)
                    nonadesioni = 100 - adesioni;
                //    result = repository.CountNonAdesioni(id) * 100 / totOpportunita;
                return nonadesioni;
            }
        }
        public bool Offline
        {
            get
            {
                return data != dataInserimento;
            }
        }

        [Display(Name = "Reparto")]
        [Required]
        public string KeyReparto { get; set; }

        public string DataScheda { get; set; }
        [Display(Name = "Data")]
        [Required]
        [RegularExpression(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$", ErrorMessage = "Formato non valido")]
        public string DataSchedaForDisplay
        {
            get
            {
                return DataScheda;
            }
            set
            {
                DataScheda = value;
            }
        }
        public string TimeScheda { get; set; }
        [Display(Name = "Ora")]
        [Required]
        [RegularExpression(@"([01]?[0-9]|2[0-3]):[0-5][0-9]", ErrorMessage = "Ora non valida")]
        public string TimeSchedaForDisplay
        {
            get
            {
                return TimeScheda;
            }
            set
            {
                TimeScheda = value;
            }
        }

        public string AnagraficaAttuale
        {
            get
            {
                vwStruttura struttura = repository.GetStrutturaCorrente(Reparto.keyStruttura);
                vwAzienda azienda = repository.GetAzienda(struttura.CodAzienda, struttura.CodRegione);
                return $"{azienda.Denominazione}-{struttura.Denominazione}-{Reparto.Nome}";
            }
        }

        public string AnagraficaDataScheda
        {
            get
            {
                vwStruttura struttura = repository.GetStrutturaDataScheda(Reparto.keyStruttura, data);
                vwAzienda azienda = repository.GetAzienda(struttura.CodAzienda, struttura.CodRegione, struttura.DataInizioAzienda);
                return $"{azienda?.Denominazione}-{struttura?.Denominazione}-{Reparto.Nome}";
            }
        }
    }

    internal sealed class SchedaMetadata
    {
        [Required]
        public int idReparto { get; set; }
        [Required]
        public int idWebServiceReparto { get; set; }

        [Display(Name = "Durata sessione")]
        [Required]
        [Range(0, 99999,
        ErrorMessage = "Il valore deve essere compreso tra {1} e {2} ")]
        public int durataSessione { get; set; }

        [Required]
        public DateTime data { get; set; }
    }

    public class SchedaManager
    {
        public double SchedePerPagina { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount
        {
            get { return Convert.ToInt32(Math.Ceiling(TotaleSchede / SchedePerPagina)); }
        }
        public int TotaleSchede { get; set; }
        public StaticPagedList<Scheda> SchedePaged { get; set; }
    }
}