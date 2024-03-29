﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mapper.Models
{
    public class AllegatoNewsViewModel
    {
        public Guid? GUID { get; set; }
        public int ID { get; set; }
        public int? IDNotizia { get; set; }
        public string Descrizione { get; set; }
        public int? Dimensione { get; set; }
        public string Formato { get; set; }
        public string NomeFile { get; set; }
        public DateTime? DataOra { get; set; }
    }
}