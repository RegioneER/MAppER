using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mapper.Models
{
    public class NewsCruscottoViewModel
    {
        public int ID { get; set; }
        public DateTime? Data { get; set; }
        public string DataLibera { get; set; }
        public string Titolo { get; set; }
        public string Descrizione { get; set; }
        public bool? IsVisibile { get; set; }
        public bool? IsNew { get; set; }
        public bool? IsInEvidenza { get; set; }
        public DateTime? VisibileDal { get; set; }
        public DateTime? VisibileAl { get; set; }
        public List<AllegatoNewsViewModel> Allegati { get; set; }
        public ApplicazioneNewsViewModel Applicazione { get; set; }
    }
}