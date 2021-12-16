using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Mapper.Candidatura.Models
{
    [MetadataType(typeof(UtenteMetadata))]
    public partial class Utente
    {
        public List<UtenteStruttura> struttureProvvisorie { get; set; }
    }

    internal sealed class UtenteMetadata
    {
        [Display(Name = "Codice fiscale")]
        [CodiceFiscaleConvalida]
        public int CodiceFiscale { get; set; }
        [Display(Name = "Profilo")]
        [Required]
        public int idRuolo { get; set; }
    }

   
}