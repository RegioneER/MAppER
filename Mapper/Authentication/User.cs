using System;
using System.Linq;
using System.Collections.Generic;
using Mapper.Models;
using PagedList;
using Newtonsoft.Json;
using Mapper.Controllers;

namespace Mapper.Authentication
{
    [Serializable]
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CodiceFiscale { get; set; }
        public string PolicyLevel { get; set; }
        public string TrustLevel { get; set; }
        public string AuthenticationMethod { get; set; }
        public string SpidCode { get; set; }
        public bool IsActive { get; set; }
        public Guid ActivationCode { get; set; }
        public bool UsaServiziFederati { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
        [JsonIgnore]
        public Ruolo Ruolo { get; set; }
        [JsonIgnore]
        public List<UtenteStruttura> UtenteStrutture
        {
            get
            {
                Entities db = new Entities(true);
                DateTime dt = DateTime.Now;
                List<UtenteStruttura> utenteStrutture = db.UtenteStruttura.Where(x => x.idUtente == this.UserId && (x.dataDal <= dt && (!x.dataAl.HasValue || x.dataAl >= dt))).ToList();
                return utenteStrutture;
            }
        }

        public void Carica(string userName, string codiceFiscale)
        {
            Entities db = new Entities(true);
            Utente utente = null;

            if (!String.IsNullOrEmpty(userName))
                utente = db.Utente.Where(x => x.username == userName && x.attivato && !x.cancellato).FirstOrDefault();

            if (utente == null)
                if (!String.IsNullOrEmpty(codiceFiscale))
                    utente = db.Utente.Where(x => x.CodiceFiscale == codiceFiscale && x.attivato && !x.cancellato).FirstOrDefault();

            if (utente == null)
            {
                throw new ErroreInterno("Utente non trovato o non attivo: " + userName);
            }
            
            UserId = utente.id;
            IsActive = utente.attivato;

            if (!IsActive)
            {
                throw new ErroreInterno("Utente non attivo:" + userName);
            }

            if (utente.idRuolo == (int)Ruolo.NonAssociato)
                throw new ErroreInterno("Ruolo utente non definito:" + userName);

            if (UtenteStrutture == null || UtenteStrutture.Count == 0)
                throw new ErroreInterno("L'utente non è associato a strutture attive:" + userName);

            if (utente.idRuolo == (int)Ruolo.Osservatore)
            {
                if (UtenteStrutture.Any(x => x.idReparto == null))
                    throw new ErroreInterno("L'utente non è associato a nessun reparto:" + userName);
            }

            Ruolo = (Ruolo)utente.idRuolo;

            FirstName = utente.nome;
            LastName = utente.cognome;
            Email = utente.email;
            Username = utente.username;
            CodiceFiscale = utente.CodiceFiscale;
        }
    }


}