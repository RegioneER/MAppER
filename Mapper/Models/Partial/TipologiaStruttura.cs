using Mapper.Models.Repository;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mapper.Models
{
    public partial class TipologiaStruttura
    {
        private MapperRepository repository = MapperRepository.GetMapperRepository();

        public string DescrizioneForDisplay
        {
            get
            {
                return string.Format("[{0}] {1}", CodTipologia, Descrizione);
            }
        }

    }

}