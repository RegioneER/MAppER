using System;
using System.Linq;
using System.Collections.Generic;
using Mapper.Candidatura.Models;
using Newtonsoft.Json;

namespace Mapper.Candidatura.Authentication
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
                utente = db.Utente.Where(x => x.username == userName).FirstOrDefault();

            else if (!String.IsNullOrEmpty(codiceFiscale))
                utente = db.Utente.Where(x => x.CodiceFiscale == codiceFiscale).FirstOrDefault();

            if (utente == null)
            {
                throw new Mapper.Candidatura.Models.Exceptions.ErroreInterno("Utente non trovato:" + userName);
            }

            UserId = utente.id;
            IsActive = utente.attivato;

            if (!IsActive)
            {
                throw new Mapper.Candidatura.Models.Exceptions.ErroreInterno("Utente non attivo:" + userName);
            }
           
            if (utente.idRuolo != 5)
            {
                Ruolo = (Ruolo)utente.idRuolo;
                //this.NomeProfilo = utente.Ruoli.nome;
            }
            else
            {
                throw new Mapper.Candidatura.Models.Exceptions.ErroreInterno("Gruppo utente non definito.");
            }
            if (UtenteStrutture.Count == 0)
                throw new Mapper.Candidatura.Models.Exceptions.ErroreInterno("L'utente non è associato a strutture attive:" + userName);

            FirstName = utente.nome;
            LastName = utente.cognome;
            Email = utente.email;
            Username = utente.username;
            CodiceFiscale = utente.CodiceFiscale;
        }
    }


}