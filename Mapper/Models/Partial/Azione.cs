using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mapper.Models
{
    public partial class Azione
    {
        public enum Stato
        {
            Lavaggio = 1,
            Frizione = 2,
            Nessuna = 3,
            NessunaConGuanti = 4
        }
    }
}