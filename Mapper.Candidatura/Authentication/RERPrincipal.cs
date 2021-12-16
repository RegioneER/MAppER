using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mapper.Candidatura.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Web;

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
                SyslogEvent.WriteLog(string.Format("CFLEG(NULL) Nessun codice fiscale rilevato"), "LoginFailure");
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
                            SyslogEvent.WriteLog(string.Format("CFLEG({0} Utente non autorizzato ad accedere all'applicazione. TrustLevel {1}.)", CodiceFiscale, TrustLevel), "LoginFailure");
                            return false;
                    }
                }
                else
                {
                    SyslogEvent.WriteLog(string.Format("CFLEG({0} Utente non autorizzato ad accedere all'applicazione. Nessun TrustLevel rilevato.)", CodiceFiscale), "LoginFailure");
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
                                SyslogEvent.WriteLog(string.Format("CFLEG({0} Utente non autorizzato ad accedere all'applicazione. PolicyLevel {1}.)", CodiceFiscale, PolicyLevel), "LoginFailure");
                                return false;
                        }
                    }
                    else
                    {

                        SyslogEvent.WriteLog(string.Format("CFLEG({0} Utente non autorizzato ad accedere all'applicazione. Nessun PolicyLevel rilevato.)", CodiceFiscale), "LoginFailure");
                        return false;
                    }
                }
            }
            Login();
            return true;
        }

        private void Login()
        {
            SyslogEvent.WriteLog(string.Format("CFLEG({0} SpidCode:{1} Utente loggato)", CodiceFiscale, SpidCode), "Login_Complete");
        }
    }
}