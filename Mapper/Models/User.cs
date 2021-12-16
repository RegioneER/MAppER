using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Web.SessionState;
using System.Net.Http.Headers;
using System.Collections.Specialized;
using Mapper.Models.Repository;
using Mapper.Models;
using Mapper.Authentication;
using PagedList;

namespace Mapper.Models
{
    [Serializable]
    public enum Ruolo
    {
        Osservatore = 1,
        Aziendale = 2,
        Regionale = 3,
        ReferenteStruttura = 4,
        NonAssociato = 5,
        Amministratore = 6
    }
    [Serializable]
    public class User : IPrincipal
    {


        #region Attributes
        private GenericIdentity _identity; // astrae lo username
        private string _username; // [PK][Identity]
        private Ruolo _ruolo;
        private string _nomeProfilo;
        private bool _isAttivo;
        private string _nomeUtente;
        private string _cognomeUtente;
        private string _email;
        private int _idUtente;
        private string _codiceFiscale;

        #endregion

        #region Properties

        public User Corrente
        {
            get
            {
                HttpContext httpContext = HttpContext.Current;
                if (httpContext == null)
                    return null;

                HttpRequest httpRequest = httpContext.Request;
                HttpSessionState httpSession = httpContext.Session;

#if DEBUG
                //string nomeUtenteAutenticato = @"RERSDM\Flamigni_S"; //REGIONALE
                //string nomeUtenteAutenticato = @"RERSDM\Martini_R";   //OSSERVATORE
                //string nomeUtenteAutenticato = @"RERSDM\Scardigli_A";  //AZIENDALE
                string nomeUtenteAutenticato = @"RERSDM\Spinelli_F";  //Ref.struttura
                //string nomeUtenteAutenticato = @"RERSDM\Aielli_G";  //NON ASSOCIATO

#else
                string nomeUtenteAutenticato = string.IsNullOrEmpty(httpRequest.Headers["Username"]) // NON è attiva l'integrazione con AccessManager...
                                                     ? httpContext.User.Identity.Name // ... allora "Windows authentication" ...
                                                     : string.Format(@"{0}\{1}", httpRequest.Headers["Domain"], httpRequest.Headers["Username"]); // ... altrimenti IAM
                
#endif
                User utenteSessione = null;
                try
                {
                    utenteSessione = (User)httpSession["user"];
                    if (utenteSessione != null)
                    {
                        HttpContext.Current.Response.ClearHeaders();
                        HttpContext.Current.Response.AddHeader("Username", utenteSessione._username);
                        HttpContext.Current.Response.AddHeader("Nome", utenteSessione.NomeUtente);
                        HttpContext.Current.Response.AddHeader("Cognome", utenteSessione.CognomeUtente);
                        HttpContext.Current.Response.AddHeader("Ruolo", ((int)utenteSessione.Ruolo).ToString());
                    }
                }
                catch
                {
                    utenteSessione = null;
                }

                if (utenteSessione == null || (utenteSessione != null && !utenteSessione.Identity.Name.Equals(nomeUtenteAutenticato, StringComparison.InvariantCultureIgnoreCase)))
                {
                    utenteSessione = UserManager.Autentica(nomeUtenteAutenticato);
                    if (httpSession != null)
                        httpSession.Add("user", utenteSessione);

                    HttpCookie cookie = new HttpCookie("user");
                    var nvc = new NameValueCollection();
                    nvc["id"] = utenteSessione.idUtente.ToString();
                    nvc["username"] = utenteSessione.Username;
                    nvc["role"] = utenteSessione.Ruolo.ToString();
                    cookie.Expires = DateTime.Now.AddDays(1);

                    //cookie.Secure = true;
                    cookie.Values.Add(nvc);
                    HttpContext.Current.Response.ClearHeaders();
                    HttpContext.Current.Response.Cookies.Add(cookie);
                    HttpContext.Current.Response.AddHeader("Username", utenteSessione._username);
                    HttpContext.Current.Response.AddHeader("Nome", utenteSessione.NomeUtente);
                    HttpContext.Current.Response.AddHeader("Cognome", utenteSessione.CognomeUtente);
                    HttpContext.Current.Response.AddHeader("Ruolo", utenteSessione.Ruolo.ToString());
                }
                return utenteSessione;
            }
        }

        public string Username
        {
            get { return _identity.Name; }
            set
            {
                //'Username' non accetta valori null
                if (value == null)
                    throw new ArgumentException("Il campo Username non accetta valori NULL");
                _identity = new GenericIdentity(value);
            }
        }

        public int idUtente
        {
            get { return _idUtente; }
            set { _idUtente = value; }
        }

        public bool isAttivo
        {
            get { return _isAttivo; }
            set { _isAttivo = value; }
        }

        public Ruolo Ruolo
        {
            get { return _ruolo; }
            set { _ruolo = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public string NomeProfilo
        {
            get { return _nomeProfilo; }
            set { _nomeProfilo = value; }
        }

        public string NomeUtente
        {
            get { return _nomeUtente; }
            set { _nomeUtente = value; }
        }

        public string CognomeUtente
        {
            get { return _cognomeUtente; }
            set { _cognomeUtente = value; }
        }

        public string CodiceFiscale
        {
            get { return _codiceFiscale; }
            set { _codiceFiscale = value; }
        }

        public List<UtenteStruttura> UtenteStrutture
        {
            get
            {
                Entities db = new Entities(true);
                List<UtenteStruttura> utenteStrutture = db.UtenteStruttura.Where(x => x.idUtente == idUtente).ToList();
                return utenteStrutture;
            }
        }

        #endregion

        #region Constructors
        public User()
        {
        }

        public User(string username)
        {
            this._identity = new GenericIdentity(username);
            this._username = username;
        }
        #endregion

        #region IPrincipal
        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string s)
        {
            return (s == _nomeProfilo);
        }
        #endregion

        #region Methods

        public void Carica()
        {
            Entities db = new Entities(true);

            Utente utente = db.Utente.Where(x => x.username == Username).FirstOrDefault();

            if (utente == null)
            {
                throw new Mapper.Models.Exceptions.ErroreInterno("Utente non trovato.");
            }


            this.idUtente = utente.id;
            this.isAttivo = utente.attivato;

            if (!this.isAttivo)
            {
                throw new Mapper.Models.Exceptions.ErroreInterno("Utente non attivato.");
            }

            if (utente.idRuolo != 5)
            {
                this.Ruolo = (Ruolo)utente.idRuolo;
                this.NomeProfilo = utente.Ruoli.nome;
            }
            else
            {
                throw new Mapper.Models.Exceptions.ErroreInterno("Gruppo utente non definito.");
            }
            this.NomeUtente = utente.nome;
            this.CognomeUtente = utente.cognome;
            this.Email = utente.email;
            this.Username = utente.username;
            this.CodiceFiscale = utente.CodiceFiscale;
            //this.Strutture = db.UtenteStruttura.Where(x => x.idUtente == idUtente).ToList();
        }
    }
    #endregion
    public class UserManager
    {
        public double UtentiPerPagina { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount
        {
            get { return Convert.ToInt32(Math.Ceiling(TotaleUtenti / UtentiPerPagina)); }
        }
        public int TotaleUtenti { get; set; }
        public StaticPagedList<Utente> UtentiPaged { get; set; }
        public static User Autentica(string username)
        {
            User u = new User(username);
            u.Carica();
            return u;
        }
    }

}
