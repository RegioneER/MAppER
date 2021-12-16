using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Configuration;


namespace Mapper.Models
{
    public partial class Entities : DbContext
    {
        public Entities(bool usaStringaConnessione)
            : base(ConfigurationManager.ConnectionStrings["MapperDB"].ConnectionString)
        {
        }
    }
}