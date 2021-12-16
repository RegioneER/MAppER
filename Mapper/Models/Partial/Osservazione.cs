using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Mapper.Models
{
    [MetadataType(typeof(OsservazioneMetadata))]
    public partial class Osservazione
    {

    }

    internal sealed class OsservazioneMetadata
    {
        [Display(Name = "Operatore")]
        [Required]
        public int idOperatore { get; set; }

        [Display(Name = "Numero operatori")]
        [Required]
        [Range(1, 999,
        ErrorMessage = "Il valore deve essere compreso tra {1} e {2} ")]
        public int numOperatori { get; set; }

    }
}