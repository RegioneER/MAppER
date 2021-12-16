
/*
REGOLA IsPrivato
pubblicoPrivata = 1 è pubblica
pubblicoprivata = 2 è privata
pubblicoprovata = 3 è provata convenzionata
*/

CREATE VIEW [dbo].[vwStruttura]
AS
SELECT 1 idWebServiceStruttura 
	   ,[IdStrutturaErogatrice]
      ,s.[DataInizio] DataRiferimentoStruttura,
	  (CONVERT(varchar,[IdStrutturaErogatrice]) + '.' + CONVERT(varchar,1)) KeyStruttura,
	  (CONVERT(varchar,[IdStrutturaErogatrice]) + '.' + CONVERT(varchar,1) + '.'+ CONVERT(varchar,s.[DataInizio])) KeyStrutturaData
      ,s.[DataFine]
      ,[DataApertura]
      ,[DataChiusura]
	  ,'['+ t.CodTipoStrutturaMinistarialeInterna + ':' + CodMin + iif(SubCodMin='','',' '+SubCodMin)+ iif(s.Datafine IS NOT NULL, ':' + CONVERT(varchar,s.datainizio), '') + '] ' + s.denominazione  + IIF(IndirizzoAggiuntivo is null,'',' , '+ LOWER(indirizzoaggiuntivo)) + IIF(DataChiusura is null,'', ' (' + convert(varchar,dataapertura) +' , ' + convert(varchar,datachiusura)+')' ) Denominazione
      ,s.[Denominazione] DenominazioneOriginale
      ,s.[CodRegione]
	  ,S.CodAzienda
      ,[DataInizioAzienda]
	  ,CodAzienda + '.' + CONVERT(varchar, [DataInizioAzienda]) KeyAzienda
      ,[CodMin]
      ,[SubCodMin]
      ,[IdCodStruttura]
      ,s.[CodTipoAnagrafeRegionale]
	  ,CodTipoStrutturaMinistarialeInterna CodiceTipologiaStruttura
	  ,t.CodTipoStrutturaMinisteriale
	  ,CodMacroArea
      ,S.[DataInizioTipoAnagrafeRegionale]
      ,[PubblicoPrivato]
	  ,IIF([PubblicoPrivato] is null,convert(bit ,1), iif(PubblicoPrivato=1,convert(bit,0),convert(bit,1))) isPrivata
      ,[IndirizzoAggiuntivo] Indirizzo
      ,[CapAggiuntivo]
      ,[IdStrutturaFisica]
      ,[DataInizioStrutturaFisica]
      ,s.[TimeStampIns]
      ,s.[TimeStampVar]
	  ,1 Stato
	  ,IIF(cp.InConvenzione is null,IIF([PubblicoPrivato] is null,convert(bit ,0), iif(PubblicoPrivato=1,convert(bit,1),convert(bit,0))),IIF(cp.dal <= GETDATE() and (cp.al is null OR cp.al>=GETDATE()), cp.InConvenzione, iif(cp.inconvenzione =1 ,convert(bit,0), convert(bit,1)))) InConvenzione
FROM [GlobalSanita].[dbo].[Struttura] S 
	INNER JOIN [GlobalSanita].[dbo].[TipoAnagrafeRegionale] T ON (S.CodTipoAnagrafeRegionale=T.CodTipoAnagrafeRegionale and S.DataInizioTipoAnagrafeRegionale=T.DataInizio)
	INNER JOIN [GlobalSanita].[dbo].[FiltroTipoAnagrafeRegionale] F ON t.CodTipoAnagrafeRegionale=f.CodTipoAnagrafeRegionale and T.DataInizio=f.DataInizioTipoAnagrafeRegionale
	INNER JOIN TipologiaStruttura TS ON (T.CodTipoStrutturaMinistarialeInterna = TS.CodTipologia AND TS.IsAttivo=1)
	LEFT JOIN GlobalSanita.dbo.ConvenzionePrivacy cp ON (CONVERT(varchar,[IdStrutturaErogatrice]) + '.' + CONVERT(varchar,1)) =cp.keystruttura

where f.idapplicazione=1 

UNION

SELECT 2 idWebServiceStruttura 
      ,s.[IdStrutturaErogatrice]
      ,s.[DataInizio]  DataRiferimentoStruttura
	  ,(CONVERT(varchar,[IdStrutturaErogatrice]) + '.' + CONVERT(varchar,2)) KeyStruttura,
	  (CONVERT(varchar,[IdStrutturaErogatrice]) + '.' + CONVERT(varchar,2) + '.'+ CONVERT( varchar,s.[DataInizio])) KeyStrutturaData
      ,s.[DataFine]
      ,[DataApertura]
      ,[DataChiusura]
	  ,'['+ t.CodTipoStrutturaMinistarialeInterna + ':' + CodMin + iif(SubCodMin='','',' '+SubCodMin)+ iif(s.Datafine IS NOT NULL, ':' + CONVERT(varchar,s.datainizio), '') + '] ' + s.denominazione  + IIF(IndirizzoAggiuntivo is null,'',' , '+ LOWER(indirizzoaggiuntivo)) + IIF(DataChiusura is null,'', ' (' + convert(varchar,dataapertura) +' , ' + convert(varchar,datachiusura)+')' ) Denominazione
      ,s.[Denominazione] DenominazioneOriginale
      ,s.[CodRegione]
	  ,S.CodAzienda
      ,[DataInizioAzienda]
	  ,CodAzienda + '.' + CONVERT(varchar, [DataInizioAzienda]) KeyAzienda
      ,[CodMin]
      ,[SubCodMin]
      ,[IdCodStruttura]
      ,s.[CodTipoAnagrafeRegionale]
	  ,CodTipoStrutturaMinistarialeInterna CodiceTipologiaStruttura
	  ,t.CodTipoStrutturaMinisteriale
	  ,CodMacroArea
      ,S.[DataInizioTipoAnagrafeRegionale]
      ,[PubblicoPrivato]
	  ,IIF([PubblicoPrivato] is null,convert(bit ,1), iif(PubblicoPrivato=1,convert(bit,0),convert(bit,1))) isPrivata
      ,[IndirizzoAggiuntivo]  Indirizzo
      ,[CapAggiuntivo]
      ,[IdStrutturaFisica]
      ,[DataInizioStrutturaFisica]
      ,s.[TimeStampIns]
      ,s.[TimeStampVar]
      ,[Stato]
	  ,IIF(cp.InConvenzione is null,IIF([PubblicoPrivato] is null,convert(bit ,0), iif(PubblicoPrivato=1,convert(bit,1),convert(bit,0))),IIF(cp.dal <= GETDATE() and (cp.al is null OR cp.al>=GETDATE()), cp.InConvenzione, iif(cp.inconvenzione =1 ,convert(bit,0), convert(bit,1)))) InConvenzione
	  
FROM [GlobalSanita].[dbo].[StrutturaCustom] S 
	INNER JOIN [GlobalSanita].[dbo].[TipoAnagrafeRegionale] T ON (S.CodTipoAnagrafeRegionale=T.CodTipoAnagrafeRegionale and S.DataInizioTipoAnagrafeRegionale=T.DataInizio)
	INNER JOIN [GlobalSanita].[dbo].[FiltroTipoAnagrafeRegionale] F ON t.CodTipoAnagrafeRegionale=f.CodTipoAnagrafeRegionale and T.DataInizio=f.DataInizioTipoAnagrafeRegionale
	INNER JOIN TipologiaStruttura TS ON (T.CodTipoStrutturaMinistarialeInterna = TS.CodTipologia AND TS.IsAttivo=1)
	LEFT JOIN [GlobalSanita].[dbo].[ConvenzionePrivacy] cp ON (CONVERT(varchar,[IdStrutturaErogatrice]) + '.' + CONVERT(varchar,2)) = cp.keystruttura

WHERE s.idapplicazione=1

