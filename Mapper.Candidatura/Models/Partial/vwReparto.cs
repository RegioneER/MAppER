using Mapper.Candidatura.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Mapper.Candidatura.Models
{
    [MetadataType(typeof(vwRepartoMetadata))]
    public partial class vwReparto
    {
        public enum TipoStato { WebService = 1, Modificato = 2, Custom = 3 }

        private MapperRepository repository = MapperRepository.GetMapperRepository();

        public bool CancellatoForDisplay
        {
            get
            {
                return !Cancellato.HasValue ? false : Cancellato.Value;
            }
            set
            {
                CancellatoForDisplay = value;
            }
        }
    }

    internal sealed class vwRepartoMetadata
    {
        [Display(Name = "Nome")]
        [Required]
        public string nome { get; set; }
        [Display(Name = "Disciplina")]
        [Required]
        public int idDisciplinaArea { get; set; }
        [Display(Name = "Regione")]
        [Required]
        public string codRegione { get; set; }
        [Display(Name = "Azienda")]
        [Required]
        public string codAzienda { get; set; }
    }

}