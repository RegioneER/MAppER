using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Mapper.Models.Cookies
{
    public class CookieRicerca
    {
        public string NomeCookie = "RicercaSchede";
        public int? IdUtente { get; set; }
        public string Regione { get; set; }
        public string Azienda { get; set; }
        public string TipoStruttura { get; set; }
        public string Struttura { get; set; }
        public string Reparto { get; set; }
        public DateTime? DataDal { get; set; }
        public DateTime? DataAl { get; set; }
        public int? StatoSessione { get; set; }
        public bool? Cancellate { get; set; }
        public int? idScheda { get; set; }
        public int? PaginaCorrente { get; set; }
        public bool? MostraStorico { get; set; }
        public string PubblicaPrivata { get; set; }

        public void CreateCookie(HttpResponseBase Response, HttpRequestBase Request)
        {
            if (Request.Cookies["Schede"] != null)
                Response.Cookies.Remove("Schede");

            HttpCookie cookie = new HttpCookie(NomeCookie);
            cookie.Expires = DateTime.Now.AddDays(1);
            cookie.Values["IdUtente"] = IdUtente.HasValue ? IdUtente.Value.ToString() : "";
            cookie.Values["Regione"] = Regione;
            cookie.Values["Azienda"] = Azienda;
            cookie.Values["TipoStruttura"] = TipoStruttura;
            cookie.Values["Struttura"] = Struttura;
            cookie.Values["Reparto"] = Reparto;
            cookie.Values["DataDal"] = DataDal.HasValue ? DataDal.Value.ToString() : "";
            cookie.Values["DataAl"] = DataAl.HasValue ? DataAl.Value.ToString() : "";
            cookie.Values["StatoSessione"] = StatoSessione.HasValue ? StatoSessione.Value.ToString() : "";
            cookie.Values["Cancellate"] = Cancellate.HasValue ? Cancellate.Value.ToString() : "";
            cookie.Values["idScheda"] = idScheda.HasValue ? idScheda.Value.ToString() : "";
            cookie.Values["Pagina"] = PaginaCorrente.ToString();
            cookie.Values["MostraStorico"] = MostraStorico.HasValue ? MostraStorico.ToString() : "";
            cookie.Values["PubblicaPrivata"] = PubblicaPrivata;
            Response.Cookies.Add(cookie);

        }

        public CookieRicerca GetCookie(HttpRequestBase Request)
        {
            if (Request.Cookies[NomeCookie] != null)
            {
                IdUtente = string.IsNullOrEmpty(Request.Cookies[NomeCookie]["IdUtente"]) ? (int?)null : Convert.ToInt32(Request.Cookies[NomeCookie]["IdUtente"]);
                DataDal = String.IsNullOrEmpty(Request.Cookies[NomeCookie]["DataDal"]) ? (DateTime?)null : Convert.ToDateTime(Request.Cookies[NomeCookie]["DataDal"]);
                DataAl = String.IsNullOrEmpty(Request.Cookies[NomeCookie]["DataAl"]) ? (DateTime?)null : Convert.ToDateTime(Request.Cookies[NomeCookie]["DataAl"]);
                Regione = Request.Cookies[NomeCookie]["Regione"];
                Azienda = Request.Cookies[NomeCookie]["Azienda"];
                TipoStruttura = Request.Cookies[NomeCookie]["TipoStruttura"];
                Struttura = Request.Cookies[NomeCookie]["Struttura"];
                Reparto = Request.Cookies[NomeCookie]["Reparto"];
                StatoSessione = string.IsNullOrEmpty(Request.Cookies[NomeCookie]["StatoSessione"]) ? (int?)null : Convert.ToInt32(Request.Cookies[NomeCookie]["StatoSessione"]);
                Cancellate = string.IsNullOrEmpty(Request.Cookies[NomeCookie]["Cancellate"]) ? (bool?)null : Convert.ToBoolean(Request.Cookies[NomeCookie]["Cancellate"]);
                idScheda = string.IsNullOrEmpty(Request.Cookies[NomeCookie]["idScheda"]) ? (int?)null : Convert.ToInt32(Request.Cookies[NomeCookie]["idScheda"]);
                PaginaCorrente = Convert.ToInt32(Request.Cookies[NomeCookie]["Pagina"]);
                MostraStorico = string.IsNullOrEmpty(Request.Cookies[NomeCookie]["MostraStorico"]) ? (bool?)null : Convert.ToBoolean(Request.Cookies[NomeCookie]["MostraStorico"]);
                PubblicaPrivata = Request.Cookies[NomeCookie]["PubblicaPrivata"];

                return this;
            }
            else return null;
        }
    }


    public class CookieRicercaUtente
    {
        public string NomeCookie = "RicercaUtente";
        public int? IdUtente { get; set; }
        public string Regione { get; set; }
        public string Azienda { get; set; }
        public string TipoStruttura { get; set; }
        public string Struttura { get; set; }
        public string Reparto { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public int? Ruolo { get; set; }
        public int? StatoSessione { get; set; }
        public bool? Cancellate { get; set; }
        public int PaginaCorrente { get; set; }
        public string PubblicaPrivata { get; set; }

        public void CreateCookie(HttpResponseBase Response, HttpRequestBase Request)
        {
            if (Response.Cookies[NomeCookie] != null)
                Request.Cookies.Remove(NomeCookie);

            HttpCookie cookie = new HttpCookie(NomeCookie);
            cookie.Values["IdUtente"] = IdUtente.HasValue ? IdUtente.Value.ToString() : "";
            cookie.Values["Regione"] = Regione;
            cookie.Values["Azienda"] = Azienda;
            cookie.Values["Ruolo"] = Ruolo.HasValue ? Ruolo.Value.ToString() : "";
            cookie.Values["TipoStruttura"] = TipoStruttura;
            cookie.Values["Struttura"] = Struttura;
            cookie.Values["Reparto"] = Reparto;
            cookie.Values["Nome"] = Nome;
            cookie.Values["Cognome"] = Cognome;
            cookie.Values["StatoSessione"] = StatoSessione.HasValue ? StatoSessione.Value.ToString() : "";
            cookie.Values["Cancellate"] = Cancellate.HasValue ? Cancellate.Value.ToString() : "";
            cookie.Values["Pagina"] = PaginaCorrente.ToString();
            cookie.Values["PubblicaPrivata"] = PubblicaPrivata;
            Response.Cookies.Add(cookie);
        }

        public CookieRicercaUtente GetCookie(HttpRequestBase Request)
        {
            if (Request.Cookies[NomeCookie] != null)
            {
                IdUtente = string.IsNullOrEmpty(Request.Cookies[NomeCookie]["IdUtente"]) ? (int?)null : Convert.ToInt32(Request.Cookies[NomeCookie]["IdUtente"]);
                Nome = Request.Cookies[NomeCookie]["Nome"];
                Cognome = Request.Cookies[NomeCookie]["Cognome"];
                Regione = Request.Cookies[NomeCookie]["Regione"];
                Azienda = Request.Cookies[NomeCookie]["Azienda"];
                TipoStruttura = Request.Cookies[NomeCookie]["TipoStruttura"];
                Struttura = Request.Cookies[NomeCookie]["Struttura"];
                Ruolo = string.IsNullOrEmpty(Request.Cookies[NomeCookie]["Ruolo"]) ? (int?)null : Convert.ToInt32(Request.Cookies[NomeCookie]["Ruolo"]);
                Reparto = Request.Cookies[NomeCookie]["Reparto"];
                StatoSessione = string.IsNullOrEmpty(Request.Cookies[NomeCookie]["StatoSessione"]) ? (int?)null : Convert.ToInt32(Request.Cookies[NomeCookie]["StatoSessione"]);
                Cancellate = string.IsNullOrEmpty(Request.Cookies[NomeCookie]["Cancellate"]) ? (bool?)null : Convert.ToBoolean(Request.Cookies[NomeCookie]["Cancellate"]);
                PaginaCorrente = Convert.ToInt32(Request.Cookies[NomeCookie]["Pagina"]);
                PubblicaPrivata = Request.Cookies[NomeCookie]["PubblicaPrivata"];
                return this;
            }
            else return null;
        }
    }
    public class CookieRicercaReparto
    {
        public string NomeCookie = "RicercaReparto";

        public string Regione { get; set; }
        public string Azienda { get; set; }
        public string TipoStruttura { get; set; }
        public string Struttura { get; set; }
        public string Reparto { get; set; }
        public int? StatoSessione { get; set; }
        public bool? Cancellate { get; set; }
        public int PaginaCorrente { get; set; }
        public string PubblicaPrivata { get; set; }

        public void CreateCookie(HttpResponseBase Response, HttpRequestBase Request)
        {
            if (Response.Cookies[NomeCookie] != null)
                Request.Cookies.Remove(NomeCookie);

            HttpCookie cookie = new HttpCookie(NomeCookie);
            cookie.Values["Regione"] = Regione;
            cookie.Values["Azienda"] = Azienda;
            cookie.Values["Struttura"] = Struttura;
            cookie.Values["TipoStruttura"] = TipoStruttura;
            cookie.Values["Reparto"] = Reparto;
            cookie.Values["StatoSessione"] = StatoSessione.HasValue ? StatoSessione.Value.ToString() : "";
            cookie.Values["Cancellate"] = Cancellate.HasValue ? Cancellate.Value.ToString() : "";
            cookie.Values["Pagina"] = PaginaCorrente.ToString();
            cookie.Values["PubblicaPrivata"] = PubblicaPrivata;
            Response.Cookies.Add(cookie);
        }

        public CookieRicercaReparto GetCookie(HttpRequestBase Request)
        {
            if (Request.Cookies[NomeCookie] != null)
            {
                Regione = Request.Cookies[NomeCookie]["Regione"];
                Azienda = Request.Cookies[NomeCookie]["Azienda"];
                Struttura = Request.Cookies[NomeCookie]["Struttura"];
                TipoStruttura = Request.Cookies[NomeCookie]["TipoStruttura"];
                Reparto = Request.Cookies[NomeCookie]["Reparto"];
                StatoSessione = string.IsNullOrEmpty(Request.Cookies[NomeCookie]["StatoSessione"]) ? (int?)null : Convert.ToInt32(Request.Cookies[NomeCookie]["StatoSessione"]);
                Cancellate = string.IsNullOrEmpty(Request.Cookies[NomeCookie]["Cancellate"]) ? (bool?)null : Convert.ToBoolean(Request.Cookies[NomeCookie]["Cancellate"]);
                PaginaCorrente = Convert.ToInt32(Request.Cookies[NomeCookie]["Pagina"]);
                PubblicaPrivata = Request.Cookies[NomeCookie]["PubblicaPrivata"];
                return this;
            }
            else return null;
        }
    }
    public class CookieRicercaCandidatura
    {
        public string NomeCookie = "RicercaCandidatura";

        public string Regione { get; set; }
        public string Azienda { get; set; }
        public string TipoStruttura { get; set; }
        public string PubblicaPrivata { get; set; }
        public string Struttura { get; set; }
        public int? Ruolo { get; set; }
        public int Stato { get; set; }
        public DateTime? DataCandidaturaDal { get; set; }
        public DateTime? DataCandidaturaAl { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string CodiceFiscale { get; set; }
        public int PaginaCorrente { get; set; }
        

        public void CreateCookie(HttpResponseBase Response, HttpRequestBase Request)
        {
            if (Response.Cookies[NomeCookie] != null)
                Request.Cookies.Remove(NomeCookie);

            HttpCookie cookie = new HttpCookie(NomeCookie);
            cookie.Values["Regione"] = Regione;
            cookie.Values["Azienda"] = Azienda;
            cookie.Values["TipoStruttura"] = TipoStruttura;
            cookie.Values["Struttura"] = Struttura;
            cookie.Values["Cognome"] = Cognome;
            cookie.Values["Nome"] = Nome;
            cookie.Values["DataDal"] = DataCandidaturaDal.HasValue ? DataCandidaturaDal.Value.ToString() : "";
            cookie.Values["DataAl"] = DataCandidaturaAl.HasValue ? DataCandidaturaAl.Value.ToString() : "";
            cookie.Values["CodiceFiscale"] = CodiceFiscale;
            cookie.Values["Ruolo"] = Ruolo.HasValue ? Ruolo.Value.ToString() : "";
            cookie.Values["Stato"] = Stato.ToString();
            cookie.Values["Pagina"] = PaginaCorrente.ToString();
            cookie.Values["PubblicaPrivata"] = PubblicaPrivata;
            Response.Cookies.Add(cookie);
        }

        public CookieRicercaCandidatura GetCookie(HttpRequestBase Request)
        {
            if (Request.Cookies[NomeCookie] != null)
            {
                Regione = Request.Cookies[NomeCookie]["Regione"];
                Azienda = Request.Cookies[NomeCookie]["Azienda"];
                TipoStruttura = Request.Cookies[NomeCookie]["TipoStruttura"];
                Struttura = Request.Cookies[NomeCookie]["Struttura"];
                Nome = Request.Cookies[NomeCookie]["Nome"];
                Cognome = Request.Cookies[NomeCookie]["Cognome"];
                DataCandidaturaDal = !string.IsNullOrEmpty(Request.Cookies[NomeCookie]["DataDal"]) ? (DateTime?)Convert.ToDateTime(Request.Cookies[NomeCookie]["DataDal"]) : null;
                DataCandidaturaAl = !string.IsNullOrEmpty(Request.Cookies[NomeCookie]["DataAl"]) ? (DateTime?)Convert.ToDateTime(Request.Cookies[NomeCookie]["DataAl"]) : null;
                Stato = Convert.ToInt32(Request.Cookies[NomeCookie]["Stato"]);
                Ruolo = string.IsNullOrEmpty(Request.Cookies[NomeCookie]["Ruolo"]) ? (int?)null : Convert.ToInt32(Request.Cookies[NomeCookie]["Ruolo"]);
                PaginaCorrente = Convert.ToInt32(Request.Cookies[NomeCookie]["Pagina"]);
                PubblicaPrivata = Request.Cookies[NomeCookie]["PubblicaPrivata"];

                return this;
            }
            else return null;
        }
    }




}