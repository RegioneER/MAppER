using Mapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Mapper.Authentication
{

    public class RERPrincipal : IPrincipal
    {
        #region Identity Properties  

        public virtual int UserId { get; set; }
        public virtual string UserName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string CodiceFiscale { get; set; }
        public virtual string PolicyLevel { get; set; }
        public virtual string TrustLevel { get; set; }
        public virtual string AuthenticationMethod { get; set; }
        public virtual string SpidCode { get; set; }
        public virtual string[] Roles { get; set; }
        #endregion

        public virtual IIdentity Identity
        {
            get; private set;
        }

        public virtual bool IsInRole(string role)
        {
            if (Roles.Any(r => role.Contains(r)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public RERPrincipal()
        {
            //Identity = new GenericIdentity("Test");
        }

        public RERPrincipal(string username)
        {
            Identity = new GenericIdentity(username);
        }

        public virtual bool IsAbilitato()
        {
            if (string.IsNullOrWhiteSpace(CodiceFiscale))
            {
                ApplicationLogger.LogEvent("Mapper", HttpContext.Current.ApplicationInstance, new Exception("CFLEG(NULL) Nessun codice fiscale rilevato - LoginFailure"));
                return false;
            }
            if (string.IsNullOrWhiteSpace(SpidCode)) //Non è un'utenza SPID
            {

                if (!string.IsNullOrEmpty(TrustLevel))
                {
                    switch (TrustLevel.ToLower())
                    {
                        case "alto":
                            break;
                        case "medio":
                        case "basso":
                            ApplicationLogger.LogEvent("Mapper", HttpContext.Current.ApplicationInstance, new Exception($"CFLEG({CodiceFiscale} Utente non autorizzato ad accedere all'applicazione. TrustLevel {TrustLevel}.) - LoginFailure"));
                            return false;
                    }
                }
                else
                {
                    ApplicationLogger.LogEvent("Mapper", HttpContext.Current.ApplicationInstance, new Exception($"CFLEG({CodiceFiscale} Utente non autorizzato ad accedere all'applicazione. Nessun TrustLevel rilevato.) - LoginFailure"));
                    return false;
                }

                if (string.IsNullOrEmpty(AuthenticationMethod) || !AuthenticationMethod.Equals("smartcard", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(PolicyLevel))
                    {
                        switch (PolicyLevel.ToLower())
                        {
                            case "alto":
                            case "medio":
                                break;
                            case "basso":
                                ApplicationLogger.LogEvent("Mapper", HttpContext.Current.ApplicationInstance, new Exception($"CFLEG({CodiceFiscale} Utente non autorizzato ad accedere all'applicazione. PolicyLevel {PolicyLevel}.) - LoginFailure"));
                                return false;
                        }
                    }
                    else
                    {
                        ApplicationLogger.LogEvent("Mapper", HttpContext.Current.ApplicationInstance, new Exception($"CFLEG({CodiceFiscale} Utente non autorizzato ad accedere all'applicazione. Nessun PolicyLevel rilevato.) - LoginFailure"));
                        return false;
                    }
                }
            }
            Login();
            return true;
        }

        private void Login()
        {
            ApplicationLogger.LogEvent("Mapper", HttpContext.Current.ApplicationInstance, new Exception($"CFLEG({CodiceFiscale} SpidCode:{SpidCode} Utente loggato) - Login_Complete"));
        }
    }
}