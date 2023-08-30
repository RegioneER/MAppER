# Mapper

Il tema della sicurezza del paziente è di centrale importanza per i Servizi sanitari. L'igiene delle mani è stata riconosciuta come uno degli elementi essenziali per proteggere il paziente dalla trasmissione crociata di microrganismi. Molti studi dimostrano che una adesione elevata alla corretta igiene delle mani riduce il rischio di infezioni correlate all'assistenza.

L'Agenzia sanitaria e sociale regionale dell'Emilia-Romagna ha coordinato in Italia la campagna **Clean Care is Safer Care**, promossa dall'Organizzazione mondiale della sanità a livello mondiale. La campagna era mirata a favorire l'igiene delle mani nelle strutture sanitarie attraverso l'adozione di linee guida basate su evidenze e la loro implementazione utilizzando una strategia multimodale.

Tra le azioni di intervento per sostenere il miglioramento dell'adesione all'igiene delle mani è compresa l'osservazione del comportamento degli operatori sanitari e il feedback dei risultati, realizzata seguendo la metodologia promossa dall'OMS attraverso la raccolta di informazioni su una scheda cartacea (sul sito [WHO](https://www.who.int/teams/integrated-health-services/infection-prevention-control/hand-hygiene/monitoring-tools)).

Dopo la fase di sperimentazione, molte strutture sanitarie a livello nazionale continuano a sostenere le iniziative annuali promosse dall'OMS.

Per facilitare i processi di miglioramento e promozione nelle Aziende sanitarie locali, si è pensato di sviluppare uno strumento informatizzato - MAppER (Mani App Emilia-Romagna) - che permette un feedback immediato, la riduzione di errori di trascrizione, il risparmio di risorse.
MAppER è un'applicazione composta da due elementi fondamentali:

- l'applicazione per la raccolta dati sul campo è una web app che può essere utilizzata tramite browser su qualunque dispositivo connesso ad Internet;
- un portale web di analisi dei dati raccolti, accessibile ai referenti di struttura.

L'obiettivo è mettere a disposizione dei professionisti sanitari un mezzo alternativo di raccolta e gestione dei dati, potenziando il ritorno delle informazioni rispetto alla tradizionale scheda cartacea, e successivo input e analisi in software specifico. Il dispositivo offre la possibilità di velocizzare la fase di raccolta e trascrizione dei dati e permette l'accesso immediato ad una reportistica dinamica a livello locale; inoltre, consente di registrare i dati della propria struttura in un archivio centrale.

La struttura e i principi metodologici presenti nello strumento sono conformi alla scheda cartacea di osservazione dell'OMS.

## Indice
L'intallazione e le personalizzazioni sono a carico del soggetto che prende a riuso la soluzione, eventuale supporto **oneroso** per le suddette attività andrà concordato con Enginerring Ingegneria Informatica (https://www.eng.it/) che ha sviluppato l'applicazione per la Regione Emilia-Romagna.

- [Come iniziare](#comeiniziare)
- [Contenuto del pacchetto](#contenutopacchetto)
- [Licenze software dei componenti di terze parti](#licenzesoftware)

## <a name="comeiniziare"/>Come iniziare
### Prerequisiti
L'applicazione MAppER necessita di:
- Microsoft SQLServer 2016 o versioni superiori
- Microsoft IIS 8.0 Windows Server 2012 o versioni superiori
- Microsoft Visual Studio 2017 o versioni superiori
- Microsoft .NET Framework 4.8

## <a name="contenutopacchetto"/>Contenuto del pacchetto
La soluzione MAppER include 5 progetti, suddivisi in progetti database, applicazioni e 4 script di creazione/inserimento dati raccolti nel file MapperRiuso.zip e Script_Mapper.zip, di seguito elencati:

### Progetti database
- GlobalSanita: database con dati condivisi (regioni, strutture, aziende sanitarie, tabelle di decodifica.) N.B. I dati fanno esplicito riferimento alla Regione Emilia-Romagna, possono essere usati a titolo di esempio ma vanno sostituiti con i dati specifici dell'utilizzatore.
- Mapper: database per l’applicazione

### Progetti applicazione
- Mapper: applicazione di gestione
- Mapper.Candidatura: applicazione per raccolta domande candidature
- RER.Tools.MVC.Agid: libreria di classi per la gestione dei controlli web AGID
N.B. la grafica fa esplicito riferimento alla Regione Emilia-Romagna e dovrà essere modificata dal soggetto che prende a riuso la soluzione adattandola alle proprie esigenze.
### Script di configurazione
Nel progetto sono stati inseriti alcuni script per la creazione delle tabelle, viste, stored procedure e i relativi dati base:
- GlobalSanita_Creazione_Riuso.sql: creazione tabelle per il database GlobalSanita
- GlobalSanita_Data.sql: script di inserimento dati  N.B. I dati fanno esplicito riferimento alla Regione Emilia-Romagna, possono essere usati a titolo di esempio ma vanno sostituiti con i dati specifici dell'utilizzatore.
- Mapper_Creazione_Riuso.sql: creazione tabelle per il database Mapper
- Mapper_Data.sql: script di inserimento dati

### Configurazione
Per l'installazione e configurazione del sistema procedere come di seguito:

1. creare l'utenza **usrMapper** (utenza e password) a livello master per il login nell'istanza SQLServer
2. creare un nuovo database denominato **GlobalSanita**
3. eseguire lo script *GlobalSanita_Creazione_Riuso.sql*. Lo script crea le tabelle, viste, stored procedure e l'utenza usrMapper con ruolo db_datareader e db_datawriter di accesso
4. eseguire lo script *GlobalSanita_Data.sql* per popolare le tabelle
5. creare un nuovo database denominato **Mapper**
6. eseguire lo script *Mapper_Creazione_Riuso.sql*. Come il precedente script, sono create tabelle, viste, stored procedure, functions e ruolo dell'utente
7. eseguire lo script *Mapper_Data.sql* per popolare le tabelle
8. estratti i progetti in una cartella, eseguire Microsoft Visual Studio e aprire il file *Mapper.sln*
9. eseguire il ripristino dei packages tramite NuGet
10.	modificare i riferimenti della connessione a SQL Server nei web.config (istanza e password)
11.	compilare la soluzione


## <a name="licenzesoftware"/>Licenze software dei componenti di terze parti

### Componenti distribuiti con MAppER
Vengono di seguito elencati i componenti distribuiti o richiesti con MAppER che hanno una propria licenza diversa da CC0.

- [bootstrap-italia 1.4.3](https://italia.github.io/bootstrap-italia/) ©
Agenzia per l'Italia Digitale e Team per la Trasformazione Digitale, licenza BSD-3-Clause
- [fontawesome 5.11.2](https://fontawesome.com/) © Fonticons, Inc., licenza GPL
- [Owl Carousel 2](https://owlcarousel2.github.io/OwlCarousel2/) © Owl (David Deutsch), licenza MIT
- [jQuery Easing](http://gsgd.co.uk/sandbox/jquery/easing/) © George McGinley Smith, licenza BSD

### Principali dipendenze per la fase di compilazione e sviluppo
- [Antlr](https://github.com/antlr/antlrcs) © Sam Harwell, Terence Parr, licenza BSD
- [Bootstrap 4.5.1](https://getbootstrap.com/) © Twitter, Inc., licenza MIT
- [Bootstrap Select 1.13.18](https://developer.snapappointments.com/bootstrap-select) © Casey Holzer, Silvio Moreto, SnapAppointments LLC, licenza MIT
- [EntityFramework 6.2.0](https://github.com/dotnet/ef6/wiki) © Microsoft, licenza Microsoft .NET Library
- [EntityFramework.it 6.2.0](https://github.com/dotnet/ef6/wiki) © Microsoft, licenza Microsoft .NET Library
- [jQuery 3.5.1](https://jquery.com/) © jQuery Foundation, licenza MIT
- [jquery.datatables 1.10.15](https://datatables.net/) © Allan Jardine, licenza MIT
- [jQuery.Validation 1.19.1](https://jqueryvalidation.org/) © Jörn Zaefferer, licenza MIT
- [Microsoft.AspNet.Mvc 5.2.7](https://www.asp.net/web-pages) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.AspNet.Mvc.it 5.2.7](https://www.asp.net/web-pages) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.AspNet.Razor 3.2.7](https://www.asp.net/web-pages) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.AspNet.Razor.it 3.2.7](https://www.asp.net/web-pages) © Microsoft, licenza Microsoft .NET Library
- Microsoft.AspNet.Web.Optimization 1.1.3 © Microsoft, licenza Microsoft .NET Library
- [Microsoft.AspNet.WebPages 3.2.7](https://www.asp.net/web-pages) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.AspNet.WebPages.it 3.2.7](https://www.asp.net/web-pages) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.CodeDom.Providers.DotNetCompilerPlatform 2.0.1](https://www.asp.net) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.jQuery.Unobtrusive.Validation 3.2.11](https://www.asp.net) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.Web.Infrastructure 1.0.0](https://www.asp.net) © Microsoft, licenza Microsoft .NET Library
- [Modernizr 2.8.3](https://modernizr.com/) © Faruk Ateş, Paul Irish, Alex Sexton, licenza MIT
- [Newtonsoft.Json 12.0.2](https://www.newtonsoft.com/json) © James Newton-King, licenza MIT
- [PagedList 1.17.0](https://github.com/TroyGoode/PagedList) © Troy Goode, licenza MIT
- [PagedList.Mvc 4.5.0](https://github.com/TroyGoode/PagedList) © Troy Goode, licenza MIT
- [popper.js 1.16.1](https://popper.js.org/) © FezVrasta and contributors, licenza MIT
- [RestSharp 106.12.0](https://restsharp.dev/) © John Sheehan, Andrew Young, Alexey Zimarev and RestSharp community, licenza Apache-2.0
- [WebGrease 1.6.0](http://webgrease.codeplex.com/) © webgrease@microsoft.com, licenza Microsoft .NET Library


### Componenti utilizzati per la documentazione

Di seguito è elencato il componente utilizzato per il sito della documentazione, ma non ridistribuito nel software MAppER

- [ghostwriter](http://wereturtle.github.io/ghostwriter) © wereturtle, licenza GPLv3

La licenza di MAppER è **GNU Affero General Public License (AGPL) versione 3 e successive (codice SPDX: AGPL-3.0-or-later)** ed è visibile sul sito [GNU Affero General Public License](https://www.gnu.org/licenses/agpl-3.0.html)

