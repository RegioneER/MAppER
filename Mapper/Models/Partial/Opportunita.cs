using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Mapper.Models
{
    [MetadataType(typeof(OpportunitaMetadata))]
    public partial class Opportunita : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (idAzione == 0)
                yield return new ValidationResult("Azione è obbligatoria", new[] { "idAzione" });

            if (idIndicazione == 0)
                yield return new ValidationResult("Indicazione è obbligatoria", new[] { "idIndicazione" });
        }

        public string imgAzione
        {
            get
            {
                switch (idAzione)
                {
                    case 1:
                        return "emoticon-green.png";
                    case 2:
                        return "emoticon-green.png";
                    case 3:
                        return "emoticon-red.png";
                    case 4:
                        return "emoticon-red.png";
                }
                return null;
            }
        }
        public string imgIndicazione
        {
            get
            {
                switch (idIndicazione)
                {
                    case 1:
                        return "pre_contatto_icon.png";
                    case 2:
                        return "pre_asepsi_icon.png";
                    case 3:
                        return "dp-fluido_icon.png";
                    case 4:
                        return "dp-paziente_icon.png";
                    case 5:
                        return "dp-ambiente_icon.png";
                }
                return null;
            }
        }
    }

    internal sealed class OpportunitaMetadata
    {
        [Display(Name = "Microrganismo")]
        [Required]
        public int idBacteria { get; set; }
        [Display(Name="Operatore")]
        [Required]
        [Range(1,int.MaxValue, ErrorMessage ="Necessario selezionare un operatore")]
        public int idOsservazione { get; set; }
    }
}