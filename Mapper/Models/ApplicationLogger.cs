using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mapper.Models
{
    public static class ApplicationLogger
    {
        public static Guid LogEvent(string applicationName, HttpApplication httpApplication, Exception exception)
        {
            if (exception.Message == "Invalid_Viewstate_Client_Disconnected")
                return Guid.Empty;
           
            if (httpApplication.Request.Url.ToString().IndexOf("get_aspx_ver.aspx") > 0)
                return Guid.Empty;


            //TODO: applicare qui il proprio log (file di testo, db ecc)
            return new Guid();
        }

    }
}