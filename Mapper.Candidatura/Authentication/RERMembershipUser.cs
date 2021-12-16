
namespace Mapper.Candidatura.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Security;


    public class RERMembershipUser : MembershipUser
    {
        #region User Properties  

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CodiceFiscale { get; set; }
        public string PolicyLevel { get; set; }
        public string TrustLevel { get; set; }
        public string AuthenticationMethod { get; set; }
        public string SpidCode { get; set; }
        public ICollection<Role> Roles { get; set; }

        #endregion

        public RERMembershipUser(User user) : base("RERMembership", user.Username, user.UserId, user.Email, string.Empty, string.Empty, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now)
        {
            UserId = user.UserId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Roles = user.Roles;
            CodiceFiscale = user.CodiceFiscale;
            PolicyLevel = user.PolicyLevel;
            TrustLevel = user.TrustLevel;
            AuthenticationMethod = user.AuthenticationMethod;
            SpidCode = user.SpidCode;
        }
    }
}
