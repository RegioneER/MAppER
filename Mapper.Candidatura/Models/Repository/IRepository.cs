using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapper.Candidatura.Models;

namespace Mapper.Candidatura.Models.Repository
{
    public interface IRepository
    {
        List<vwRegione> GetRegioni();
        List<vwAzienda> GetAziende(string codRegione);
        List<vwStruttura> GetvwStrutture(string codRegione, string codAzienda, string codTipoStruttura);
        List<TipologiaStruttura> GetTipiStruttureAttive(); 
        

    }
}
