using Mapper.Candidatura.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Mapper.Candidatura.Models
{
    [MetadataType(typeof(UtenteStrutturaMetadata))]
    public partial class UtenteStruttura
    {
        private MapperRepository repository = MapperRepository.GetMapperRepository();

        private vwStruttura _Struttura;
        public vwStruttura Struttura
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CodiceStruttura))
                    throw new NullReferenceException("Il parametro CodiceStruttura non deve essere null");

                if (_Struttura == null)
                    _Struttura = repository.DBContext.vwStruttura.Find(CodiceStruttura);

                return _Struttura;
            }
        }       

        [Display(Name = "Data dal")]
        [Required]
        [RegularExpression(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$", ErrorMessage = "Formato data non valido")]
        public string DataDalForDisplay
        {
            get
            {
                string s = null;
                s = dataDal.ToShortDateString();
                return s;
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
        public string CodiceStruttura
        {
            get
            {

                return string.Format("{0}.{1}.{2}.{3}.{4}.{5}", codRegione, codAzienda, Struttura.CodMin, Struttura.SubCodMin, Struttura.CodiceTipologiaStruttura, idWebServiceStruttura);
            }
        }
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