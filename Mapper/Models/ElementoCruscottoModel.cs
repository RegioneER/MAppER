using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mapper.Models
{
    public class ElementoCruscottoModel
    {
        public string CodStatoTrattamento { get; internal set; }

        public string DescStatoTrattamento { get; internal set; }
        public int Conteggio { get; internal set; }
    }
}