using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mapper.Models
{
    public partial class Indicazione
    {
        public enum Stato
        {
            PreContattoPaziente = 1,
            PreManovraAsepsi = 2,
            DopoContattoFluido = 3,
            DopoContattoPaziente = 4,
            DopoContattoAmbiente = 5
        }
    }
}