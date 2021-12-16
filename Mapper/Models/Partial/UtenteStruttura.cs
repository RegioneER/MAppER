using Mapper.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Mapper.Models
{
    [MetadataType(typeof(UtenteStrutturaMetadata))]
    public partial class UtenteStruttura
    {
        private MapperRepository repository = MapperRepository.GetMapperRepository();

        [Display(Name = "Data dal")]
        [Required]
        [RegularExpression(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$", ErrorMessage = "Formato data non valido")]
        public string DataDalForDisplay
        {
            get
            {
                return dataDal.ToShortDateString();
            }
        }
        [Display(Name = "Data al")]
        [RegularExpression(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$", ErrorMessage = "Formato data non valido")]
        public string DataAlForDisplay
        {
            get
            {
                string s = null;
                if (dataAl.HasValue)
                    s = dataAl.Value.ToShortDateString();
                return s;
            }
        }
        public int idRuolo { get; set; }

        [Display(Name = "Regione")]
        public string NomeRegione
        {
            get
            {
                string nomeRegione = "";
                if (!String.IsNullOrEmpty(codRegione))
                    nomeRegione = repository.GetRegione(codRegione).Denominazione;

                return nomeRegione;
            }
        }
        [Display(Name = "Azienda")]
        public string NomeAzienda
        {
            get
            {
                string nomeAzienda = "";
                if (!String.IsNullOrEmpty(codAzienda))
                    nomeAzienda = repository.GetAzienda(codAzienda, codRegione).Denominazione;

                return nomeAzienda;
            }
        }
        public string KeyAzienda
        {
            get
            {
                string keyAzienda = "";
                if (!String.IsNullOrEmpty(codAzienda))
                    keyAzienda = repository.GetAzienda(codAzienda, codRegione).KeyAzienda;

                return keyAzienda;
            }
        }
        [Display(Name = "Struttura")]
        [Required]
        public string KeyStrutturaForDisplay { get; set; }
        public string CodiceStruttura
        {
            get
            {
                return (idStrutturaErogatrice.HasValue && idWebServiceStruttura.HasValue) ? idStrutturaErogatrice.Value.ToString() + "." + idWebServiceStruttura.Value.ToString() : null;
            }
        }
        public string CodiceTipologiaStruttura
        {
            get
            {
                string tipologia = "";
                if (idStrutturaErogatrice.HasValue && idWebServiceStruttura.HasValue)
                    tipologia = repository.GetStruttura(idStrutturaErogatrice.Value, idWebServiceStruttura.Value).CodiceTipologiaStruttura;

                return tipologia;

            }
        }
        [Display(Name = "Reparto")]
        [Required]
        public string KeyRepartoForDisplay { get; set; }
        public string KeyReparto
        {
            get
            {
                return (idReparto.HasValue && idWebServiceReparto.HasValue) ? idReparto.Value.ToString() + "." + idWebServiceReparto.Value.ToString(): null;
            }
        }
        [Display(Name = "Reparto")]
        public string NomeReparto
        {
            get
            {
                string nomeReparto = "";
                if (!string.IsNullOrEmpty(KeyReparto))
                    nomeReparto = repository.GetReparto(KeyReparto).Nome;

                return nomeReparto;
            }
        }
        [Display(Name = "Struttura")]
        public string NomeStruttura
        {
            get
            {
                vwStruttura struttura = null;
                if (idStrutturaErogatrice.HasValue && idWebServiceStruttura.HasValue)
                    struttura = repository.GetStruttura(idStrutturaErogatrice.Value, idWebServiceStruttura.Value);
                return struttura == null ? "" : struttura.Denominazione;
            }
        }
        public bool PuoEssereModificato { get; set; }
       
    }
    internal sealed class UtenteStrutturaMetadata
    {
       [JsonIgnore]
        vwRegione Regione { get; set; }
        [JsonIgnore]
        vwAzienda Azienda { get; set; }
        [JsonIgnore]
        Utente Utente { get; set; }
        [JsonIgnore]
        TipologiaStruttura TipologiaStruttura { get; set; }
        [JsonIgnore]
        vwReparto Reparto { get; set; }
    }


}