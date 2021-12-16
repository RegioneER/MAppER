using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mapper.Candidatura.Models.Exceptions
{
    public class ErroreInterno : Exception
    {
        public ErroreInterno(string message) : base(message)
        {

        }
    }
  
}