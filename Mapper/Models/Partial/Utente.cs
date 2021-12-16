using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.ComponentModel.DataAnnotations;
using Mapper.Models.Repository;

namespace Mapper.Models
{
    [MetadataType(typeof(UtenteMetadata))]
    public partial class Utente
    {
        public List<UtenteStruttura> struttureProvvisorie { get; set; }
        public List<UtenteStruttura> struttureAttive
        {
            get
            {
                return this.UtenteStruttura.Where(x => x.dataDal <= DateTime.Now && (!x.dataAl.HasValue || x.dataAl >= DateTime.Now)).ToList();
            }
        }
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

    public class CodiceFiscaleConvalida : ValidationAttribute
    {
        private MapperRepository repository = MapperRepository.GetMapperRepository();

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

        public bool ControllaCodiceFiscale(string CodiceFiscale)
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

    public class UtenteManager
    {
        public double UtentiPerPagina { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount
        {
            get { return Convert.ToInt32(Math.Ceiling(TotaleUtenti / UtentiPerPagina)); }
        }
        public int TotaleUtenti { get; set; }
        public StaticPagedList<Utente> UtentiPaged { get; set; }

    }

}