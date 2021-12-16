using Mapper.Candidatura.Models.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mapper.Candidatura.Models
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
        [Display(Name = "Regione")]
        [Required]
        public string CodRegione { get; set; }

        [RegularExpression(@"^[-_a-zA-Z0-9]+(\.[-_a-zA-Z0-9]+)*@[-_a-zA-Z0-9]+(\.[-_a-zA-Z0-9]+)+$", ErrorMessage = "Il formato della Email non è corretto")]
        [Required, MaxLength(255)]
        public string Email { get; set; }
        [Required, MaxLength(50)]
        public string Nome { get; set; }
        [Required, MaxLength(50)]
        public string Cognome { get; set; }
        [Display(Name = "Codice fiscale")]
        [CodiceFiscaleConvalida]
        [Required]
        public string CodiceFiscale { get; set; }
        [Required]
        public string CodAzienda { get; set; }
        [Required]
        public int IdRuolo { get; set; }
    }

    public class CodiceFiscaleConvalida : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (String.IsNullOrEmpty((string)value) || ControllaCodiceFiscale((string)value))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Codice fiscale non valido");
                }
            }
            catch (Exception)
            {
                return new ValidationResult("Codice fiscale non valido");
            }
        }

        public static bool ControllaCodiceFiscale(string CodiceFiscale)
        {

            bool result = false;
            const int caratteri = 16;
            if (CodiceFiscale == null)
                return result;

            // se il codice fiscale non è di 16 caratteri il controllo
            // è già finito prima ancora di cominciare

            if (CodiceFiscale.Length != caratteri)
                return result;

            // stringa per controllo e calcolo omocodia
            const string omocodici = "LMNPQRSTUV";
            // per il calcolo del check digit e la conversione in numero
            const string listaControllo = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int[] listaPari = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            int[] listaDispari = { 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23 };

            CodiceFiscale = CodiceFiscale.ToUpper();
            char[] cCodice = CodiceFiscale.ToCharArray();

            // check della correttezza formale del codice fiscale
            // elimino dalla stringa gli eventuali caratteri utilizzati negli
            // spazi riservati ai 7 che sono diventati carattere in caso di omocodia
            for (int k = 6; k < 15; k++)
            {
                if ((k == 8) || (k == 11))
                    continue;
                int x = (omocodici.IndexOf(cCodice[k]));
                if (x != -1)
                    cCodice[k] = x.ToString().ToCharArray()[0];
            }

            result = true;

            if (result)
            {
                int somma = 0;
                // ripristino il codice fiscale originario 
                // grazie a Lino Barreca che mi ha segnalato l'errore
                cCodice = CodiceFiscale.ToCharArray();
                for (int i = 0; i < 15; i++)
                {
                    char c = cCodice[i];
                    int x = "0123456789".IndexOf(c);
                    if (x != -1)
                        c = listaControllo.Substring(x, 1).ToCharArray()[0];
                    x = listaControllo.IndexOf(c);
                    // i modulo 2 = 0 è dispari perchè iniziamo da 0
                    if ((i % 2) == 0)
                        x = listaDispari[x];
                    else
                        x = listaPari[x];
                    somma += x;
                }

                result = (listaControllo.Substring(somma % 26, 1) == CodiceFiscale.Substring(15, 1));
            }
            return result;
        }
    }

}