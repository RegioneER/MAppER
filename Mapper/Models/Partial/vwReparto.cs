using Mapper.Models.Repository;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Mapper.Models
{
    [MetadataType(typeof(vwRepartoMetadata))]
    public partial class vwReparto
    {
        public enum TipoStato { WebService = 1, Modificato = 2, Custom = 3 }
        private MapperRepository repository = MapperRepository.GetMapperRepository();

        [Display(Name = "Struttura")]
        [Required]
        public string CodiceStruttura
        {
            get
            {
                return $"{IdStrutturaErogatrice}.{idWebServiceStruttura}";
            }
        }

        public string NomeRegione
        {
            get
            {
                return repository.GetRegione(CodRegione).Denominazione;
            }
        }
        public string NomeAzienda
        {
            get
            {
                return repository.GetAzienda(CodAzienda, CodRegione).Denominazione;
            }
        }
        public string NomeAreaDisciplina
        {
            get
            {
                return codAreaDisciplina.HasValue ? repository.GetAreaDisciplina(codAreaDisciplina.Value).Nome : "";
            }
        }
        public string NomeStruttura
        {
            get
            {
                vwStruttura struttura = repository.GetStruttura(IdStrutturaErogatrice, idWebServiceStruttura);
                return struttura != null ? struttura.Denominazione : "";
            }
        }
}

    internal sealed class vwRepartoMetadata
    {
        [Display(Name = "id")]
        public int IdReparto { get; set; }
        [Display(Name = "Nome")]
        [Required]
        public string Nome { get; set; }
        [Display(Name = "Disciplina")]
        [Required]
        public int codAreaDisciplina { get; set; }
        [Display(Name = "Regione")]
        [Required]
        public string CodRegione { get; set; }
        [Display(Name = "Azienda")]
        [Required]
        public string CodAzienda { get; set; }
        [Display(Name = "Struttura")]
        [Required]
        public string keyStruttura { get; set; }
    }


    public class vwRepartoManager
    {
        public double RepartiPerPagina { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount
        {
            get { return Convert.ToInt32(Math.Ceiling(TotaleReparti / RepartiPerPagina)); }
        }
        public int TotaleReparti { get; set; }
        public StaticPagedList<vwReparto> RepartiPaged { get; set; }

    }
}