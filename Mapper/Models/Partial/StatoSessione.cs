using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mapper.Models
{
    public partial class StatoSessione
    {
        public enum Stato
        {
            InLavorazione = 2,
            Consolidata = 3,
            Cancellata = 4
        }
    }
}