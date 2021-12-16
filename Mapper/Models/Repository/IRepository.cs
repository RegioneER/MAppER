using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapper.Models;

namespace Mapper.Models.Repository
{
    public interface IRepository
    {
        Utente GetUtente(string userName);
        Utente GetUtente(int ID);
        UtenteStruttura GetUtenteStruttura(int ID);
        List<vwRegione> GetRegioni();
        List<vwRegione> GetRegioniUtente(string userName);
        List<vwAzienda> GetAziende(string codRegione);
        //List<vwAzienda> GetAziendeUtente(string userName, string codRegione);
       // List<vwStruttura> GetStruttureBuff(string codRegione, string codAzienda);
        //List<vwStruttura> GetStruttureBuff(string codRegione, string codAzienda, string codTipoStruttura);
        //List<vwStruttura> GetStruttureBuffUtente(string userName, string codRegione, string codAzienda);
        List<TipologiaStruttura> GetTipiStruttureAttive();
        List<vwReparto> GetRepartiByTipoStruttura(string codRegione, string codAzienda, string codTipoStruttura, int? idDisciplinaArea, bool mostraTutti);
        List<vwReparto> GetReparti(string codRegione, string codAzienda, string codStruttura, bool forStorico, bool mostraTutti);
        List<vwReparto> GetRepartiUtente(string userName, string codRegione, string codAzienda, string codStruttura);
        List<StatoSessione> GetStatiSessione();
        List<Scheda> GetSchede(int skip, int take, int idUtente, string codRegione, string codAzienda, string codTipoStruttura, string keyStruttura, string keyReparto, int? statoSessione, DateTime? dataInizio, DateTime? dataFine, bool? mostraCancellate, int? idScheda, out int totaleSchede);
        Opportunita GetOpportunita(long id);
        List<Opportunita> GetOpportunitaByOsservazione(long idOsservazione);
        List<Bacteria> GetBacteri();
        List<Indicazione> GetIndicazioni();
        int CountOpportunita(int idScheda);
        int CountSoggetti(int idScheda);
        int CountAdesioni(int idScheda);
        int CountNonAdesioni(int idScheda);
        int CountAdesioni(int idScheda, int idIndicazione);
        int CountNonAdesioni(int idScheda, int idIndicazione);
        Scheda GetScheda(int idScheda);
        vwReparto GetReparto(int idReparto);
        long SaveScheda(Scheda scheda);
        Osservazione GetOsservazione(long id);
        List<Operatore> GetOperatori();
        long SaveOsservazione(Osservazione osservazione);
        long SaveOpportunita(Opportunita opportunita);
        void SetStatoScheda(int idScheda, StatoSessione.Stato stato);
        void DeleteOsservazioneOpportunita(int idOsservazione);
        void DeleteOsservazione(int idOsservazione);
        void DeleteOpportunitaByIdOsservazione(int idOsservazione);
        void DeleteOpportunita(int idOpportunita);
        IEnumerable<fn_TabellaOsservazioni_Result> GetTabellaOsservazioni(int? idScheda, int? idOsservazione);
        IEnumerable<fn_TabellaAdesioni_Result> GetTabellaAdesioni(int? idScheda, int? idOsservazione);
        void UpdatePosizione(string username, string url, string datiRicerca);
        UltimaPosizione GetUltimaPosizione(string username);
        void InsertUpdateLog(string username, string message, string url);
        List<Ruoli> GetRuoli(int ruoloPartenza);
        long SaveUtenteStruttura(UtenteStruttura utenteStruttura);
        void DeleteUtenteStruttura(int id);
        bool CodiceFiscaleAssociatoAdUtente(int idUtente, string codiceFiscale);

    }
}
