USE [Mapper]
GO

CREATE USER [usrMapper] FOR LOGIN [usrMapper] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [usrMapper]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [usrMapper]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fn_AreaDisciplinaExist]
(@CodAreaDisciplina int=NULL) 
RETURNS BIT
AS
BEGIN
    DECLARE @exists bit = 0

    IF (@CodAreaDisciplina is null)
	OR EXISTS (
      SELECT TOP 1 1 FROM vwAreaDisciplina
      WHERE (CodAreaDisciplina = @CodAreaDisciplina
				)
    ) BEGIN 
         SET @exists = 1 
      END;
      RETURN @exists
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fn_AziendaAttivaExist]
(@codRegione varchar(3), @codAzienda varchar(3), @valoreNullValido BIT) 
RETURNS BIT
AS
BEGIN
    DECLARE @exists bit = 0
    IF (@valoreNullValido=1 and @codAzienda is null)
	OR EXISTS (
      SELECT TOP 1 1 FROM vwAzienda
      WHERE CodRegione=@codRegione and CodAzienda=@codAzienda and DataFine is null
    ) BEGIN 
         SET @exists = 1 
      END;
      RETURN @exists
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_KeyStrutturaAttivaExist]
(@keyStruttura varchar(61), @valoreNullValido BIT) 
RETURNS BIT
AS
BEGIN
    DECLARE @exists bit = 0
    IF (@valoreNullValido=1 and @keyStruttura is null)
	OR EXISTS (
      SELECT TOP 1 1 FROM vwStruttura
      WHERE (KeyStruttura = @keyStruttura and DataFine is null and DataChiusura is null)
    ) BEGIN 
         SET @exists = 1 
      END;
      RETURN @exists
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_RegioneExist]
(@codRegione varchar(3)) 
RETURNS BIT
AS
BEGIN
    DECLARE @exists bit = 0
    IF EXISTS (
      SELECT TOP 1 1 FROM vwRegione 
      WHERE CodRegione=@codRegione
    ) BEGIN 
         SET @exists = 1 
      END;
      RETURN @exists
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fn_RepartoExist]
(@idReparto INT, @idWebServiceReparto INT, @valoreNullValido BIT) 
RETURNS BIT
AS
BEGIN
    DECLARE @exists bit = 0

    IF (@valoreNullValido=1 and @idReparto is null and @idWebServiceReparto is null)
	OR EXISTS (
      SELECT TOP 1 1 FROM vwReparto 
      WHERE (IdReparto = @idReparto 
				and IdWebServiceReparto =@idWebServiceReparto)
    ) BEGIN 
         SET @exists = 1 
      END;
      RETURN @exists
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fn_StrutturaAttivaExist]
(@idStruttura INT, @idWebService INT, @valoreNullValido BIT) 
RETURNS BIT
AS
BEGIN
    DECLARE @exists bit = 0
    IF (@valoreNullValido=1 and @idStruttura is null and @idWebService is null)
	OR EXISTS (
      SELECT TOP 1 1 FROM vwStruttura
      WHERE (idstrutturaerogatrice = @idStruttura and IdWebServiceStruttura =@idWebService and DataFine is null and DataChiusura is null)
    ) BEGIN 
         SET @exists = 1 
      END;
      RETURN @exists
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  FUNCTION [dbo].[fn_TabellaAdesioni]
(
	@idScheda int = null,
	@idOsservazione int = null
)
RETURNS @temp TABLE 
(
Indicazione varchar(30),
	idIndicazione int PRIMARY KEY,	
	Adesioni int,
	NonAdesioni int,
	TotIndicazioni int,
	PercAdesione decimal,
	PercNonAdesione decimal
)
AS
BEGIN

DECLARE @appoggio TABLE 
(
	Indicazione varchar(30),
	idIndicazione int,	
	Adesioni int,
	NonAdesioni int,
	TotIndicazioni int,
	PercAdesione decimal,
	PercNonAdesione decimal
)

insert into @appoggio  
	select *,
		 [1] + [0] Totale , 
		([1]*100/([1] + [0])) PercAdesione ,
		(100- ([1]*100/([1] + [0]))) PercNonAdesione

		 from (
			select a.adesione Adesione,
					i.tipologia,
					i.id

			from  Osservazione os inner join 
				(
				Opportunita op 
					inner join Indicazione i on op.idIndicazione= i.id
					inner join Azione a on op.idAzione=a.id
				) on os.id=op.idOsservazione
			where (@idScheda IS NOT NULL AND idScheda=@idScheda)
				OR (@idOsservazione IS NOT NULL AND idOsservazione =@idOsservazione)

			
) t
pivot (
	Count(adesione)
	for adesione in ([1],
	[0])
	) 
	as pivot_table
	order by id

	INSERT INTO @appoggio 
	select [tipologia], id, 0, 0, 0, 0, 0 from dbo.Indicazione  i where not exists (select 1 from @appoggio t2 where i.tipologia = t2.Indicazione)
	
	insert @temp select * from @appoggio order by Indicazione

	RETURN 
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_TabellaOsservazioni]
(
	@idScheda int = NULL,
	@idOsservazione int = NULL
)
RETURNS @temp TABLE 
(
	Azione varchar(20) PRIMARY KEY,
	[Prima contatto paziente] int,
		[Prima di manovra di asepsi] int,
		[Dopo contatto fluido] int,
		[Dopo contatto paziente] int,
		[Dopo contatto ambiente ] int
)
AS
BEGIN

DECLARE @appoggio TABLE 
(
	Azione varchar(20),
	[Prima contatto paziente] int,
		[Prima di manovra di asepsi] int,
		[Dopo contatto fluido] int,
		[Dopo contatto paziente] int,
		[Dopo contatto ambiente ] int
)

insert @appoggio  
	select * from (
		
		select a.tipologia Azione,
				i.tipologia Indicazione, 
				op.idIndicazione

		from  Osservazione os inner join 
				(
				Opportunita op 
					inner join Indicazione i on op.idIndicazione= i.id
					inner join Azione a on op.idAzione=a.id
				) on os.id=op.idOsservazione

where (@idScheda IS NOT NULL AND idScheda=@idScheda)
				or (@idOsservazione IS NOT NULL AND idOsservazione =@idOsservazione)

) t
pivot (
	COUNT(idindicazione)
	for Indicazione in (
		[Prima contatto paziente],
		[Prima di manovra di asepsi],
		[Dopo contatto fluido],
		[Dopo contatto paziente],
		[Dopo contatto ambiente ])

	) as pivot_table

	INSERT INTO @appoggio 
	select [tipologia],  0, 0, 0, 0, 0 from dbo.Azione  a where not exists (select 1 from @appoggio t2 where a.tipologia = t2.Azione)
	
	insert @temp select * from @appoggio order by Azione
	
	RETURN
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipologiaStruttura](
	[CodTipologia] [varchar](50) NOT NULL,
	[Descrizione] [varchar](100) NOT NULL,
	[TipoEnte] [varchar](10) NULL,
	[IsAttivo] [bit] NOT NULL,
	[Ordinale] [int] NOT NULL,
	[CodAreaDisciplina] [int] NULL,
 CONSTRAINT [PK_TipologiaStruttura] PRIMARY KEY CLUSTERED 
(
	[CodTipologia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO














CREATE VIEW [dbo].[vwReparto]
AS

SELECT 1 IdWebServiceReparto,
		r.IdReparto,
		 (CONVERT(varchar,r.IdReparto) + '.' + CONVERT(varchar,1)) KeyReparto,
		r.IdStrutturaErogatrice,
		vwS.DataRiferimentoStruttura DataRiferimentoStruttura,
		vwS.idWebServiceStruttura,
		vws.keyStruttura,
		vws.keyStrutturaData,
		r.DataInizio,
		r.DataFine,
		d.Descrizione + ' ' + r.ProgressivoDivisione NomeOriginale, 
	 	iif(ra.Nome is null, d.Descrizione + ' ' + r.ProgressivoDivisione, ra.Nome) Nome, -- nome semplice(reale o alias)
		iif(ra.Descrizione is null, '', ra.Descrizione) Descrizione,
		iif(ra.Nome is null, d.Descrizione + ' ' + r.ProgressivoDivisione, ra.Nome + ' [' + d.Descrizione + ' ' + r.ProgressivoDivisione + ']') NomeCompleto, --nome per le combo (reale o alias + reale)
		r.CodDisciplina,
		r.ProgressivoDivisione,
		vwS.CodRegione,
		vwS.CodAzienda,
		vwS.CodiceTipologiaStruttura,
		vwS.CodTipoAnagrafeRegionale,
		dad.codAreaDisciplina,
		iif(ra.cancellato is null, convert(bit,0),convert(bit,ra.cancellato)) Cancellato,
		iif(ra.IdReparto is null,1,2) Stato,
		convert(bit, iif(r.DataInizioStrutturaErogatrice = vwS.DataRiferimentoStruttura,0, 1)) isStorico
		
FROM [GlobalSanita].dbo.Reparto r
join GlobalSanita.dbo.Disciplina d on r.CodDisciplina=d.CodDisciplina 
join vwStruttura vwS on r.IdStrutturaErogatrice = vwS.IdStrutturaErogatrice  
left join GlobalSanita.dbo.RepartoAlias ra on r.IdReparto = ra.idReparto
left join GlobalSanita.dbo.DisciplinaAreaDisciplina dad on d.CodDisciplina=dad.CodDisciplina

WHERE ra.idapplicazione is null OR ra.idApplicazione=1

UNION

select 2 IdWebServiceReparto,
		rc.IdReparto,
		 (CONVERT(varchar,rc.IdReparto) + '.' + CONVERT(varchar,2)) KeyReparto,
		rc.IdStrutturaErogatrice,
		vwS.DataRiferimentoStruttura DataRiferimentoStruttura,
		vwS.idWebServiceStruttura,
		vws.keyStruttura,
		vws.keyStrutturaData,
		rc.DataInizio,
		rc.DataFine,
		rc.nome NomeOriginale,
	 	rc.Nome,
		rc.Descrizione,
		rc.Nome NomeCompleto,
		rc.CodDisciplina,
		rc.ProgressivoDivisione,
		vwS.CodRegione,
		vwS.CodAzienda,
		vwS.CodiceTipologiaStruttura,
		vwS.CodTipoAnagrafeRegionale,
		rc.codAreaDisciplina,
		rc.Cancellato,
		rc.Stato,
		Convert(bit,0) isStorico

FROM GlobalSanita.dbo.RepartoCustom rc 
join vwStruttura vwS on rc.IdStrutturaErogatrice = vwS.IdStrutturaErogatrice and rc.DataInizioStrutturaErogatrice = vwS.DataRiferimentoStruttura 

WHERE rc.idapplicazione is null OR rc.idApplicazione=1
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwAreaDisciplina]
AS
SELECT  CodAreaDisciplina,
		Nome,
		Descrizione,
		Ordinale

FROM            [GlobalSanita].[dbo].[AreaDisciplina]
WHERE IdApplicazione = 1
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwAzienda]
AS
SELECT        CodRegione, 
				CodAzienda, 
				IIF(Datafine is null, Denominazione, Denominazione + ' (' + CONVERT(VARCHAR(10),DataInizio,101) + ' , ' + CONVERT(VARCHAR(10),DataFine,101) + ')') AS Denominazione,
				DataInizio,
				DataFine,
				CodAzienda + '.' + CONVERT(varchar, DataInizio) KeyAzienda

FROM            [GlobalSanita].[dbo].[Azienda]

WHERE DataFine is null OR DataFine >= '01/01/2015'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwRegione]
AS
SELECT       CodRegione,
			Denominazione

FROM            [GlobalSanita].[dbo].[Regione]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwTipoAnagrafeRegionale]
AS
SELECT        tar.CodTipoAnagrafeRegionale,
				DataInizio,
				DataFine,
				Descrizione,
				CodMacroArea,
				CodTarget,
				CodTipoStrutturaMinisteriale,
				CodTipoStrutturaMinistarialeInterna
FROM            [GlobalSanita].dbo.TipoAnagrafeRegionale tar
inner join [GlobalSanita].dbo.FiltroTipoAnagrafeRegionale ftar on tar.CodTipoAnagrafeRegionale= ftar.CodTipoAnagrafeRegionale

where ftar.idapplicazione = 1
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Azione](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[tipologia] [nvarchar](50) NOT NULL,
	[ordinale] [int] NULL,
	[adesione] [bit] NOT NULL,
 CONSTRAINT [PK_Azioni] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bacteria](
	[code] [nchar](20) NOT NULL,
	[description_EN] [nvarchar](max) NULL,
	[description_IT] [nvarchar](max) NULL,
	[ordinale] [int] NULL,
 CONSTRAINT [PK_Bacteria] PRIMARY KEY CLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Indicazione](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[tipologia] [nvarchar](50) NOT NULL,
	[ordinale] [int] NULL,
 CONSTRAINT [PK_Indicazione] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
	[data] [datetime] NOT NULL,
	[type] [int] NOT NULL,
	[message] [nvarchar](max) NOT NULL,
	[user] [nvarchar](50) NULL,
	[IP] [varchar](50) NULL,
	[request] [nvarchar](max) NULL,
	[user-agent] [nvarchar](max) NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[data] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descrizione] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_LogType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Operatore](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[codCategoria] [varchar](50) NOT NULL,
	[nomeCategoria] [varchar](max) NOT NULL,
	[classe] [varchar](50) NOT NULL,
	[ClasseColore] [varchar](20) NULL,
 CONSTRAINT [PK_Operatore] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Opportunita](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[idAzione] [int] NOT NULL,
	[idOsservazione] [bigint] NOT NULL,
	[idIndicazione] [int] NOT NULL,
	[idBacteria] [nchar](20) NULL,
	[data] [datetime] NULL,
	[cancellato] [bit] NOT NULL,
 CONSTRAINT [PK_Opportunità] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Osservazione](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[idScheda] [int] NOT NULL,
	[numOperatori] [int] NOT NULL,
	[idOperatore] [int] NOT NULL,
	[operatoreEsterno] [bit] NULL,
	[data] [datetime] NULL,
 CONSTRAINT [PK_Osservazioni] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ruoli](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nome] [nvarchar](50) NOT NULL,
	[ordinale] [int] NULL,
 CONSTRAINT [PK_Ruoli] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Scheda](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[durataSessione] [int] NOT NULL,
	[idReparto] [int] NOT NULL,
	[idWebServiceReparto] [int] NOT NULL,
	[idUtente] [int] NOT NULL,
	[note] [nvarchar](50) NULL,
	[idStatoSessione] [int] NOT NULL,
	[data] [datetime] NOT NULL,
	[dataInserimento] [datetime] NOT NULL,
	[dataUltimaModificaStato] [datetime] NOT NULL,
 CONSTRAINT [PK_Schede] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatoCandidatura](
	[Codice] [int] NOT NULL,
	[Descrizione] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_StatoCandidatura] PRIMARY KEY CLUSTERED 
(
	[Codice] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatoSessione](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nome] [nchar](10) NOT NULL,
	[DescrizionePubblica] [nchar](20) NOT NULL,
 CONSTRAINT [PK_StatoSessione] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UltimaPosizione](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](50) NOT NULL,
	[url] [nvarchar](255) NOT NULL,
	[datiRicerca] [nvarchar](max) NULL,
	[data] [datetime] NOT NULL,
 CONSTRAINT [PK_UltimaPosizione] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Utente](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](50) NOT NULL,
	[nome] [nvarchar](50) NOT NULL,
	[cognome] [nvarchar](50) NOT NULL,
	[email] [nvarchar](50) NOT NULL,
	[idRuolo] [int] NOT NULL,
	[attivato] [bit] NOT NULL,
	[cancellato] [bit] NOT NULL,
	[CodiceFiscale] [varchar](16) NULL,
 CONSTRAINT [PK_Utenti] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UtenteDaCensire](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CodiceFiscale] [varchar](16) NOT NULL,
	[CodRegione] [varchar](3) NOT NULL,
	[CodAzienda] [varchar](3) NOT NULL,
	[Cognome] [varchar](50) NOT NULL,
	[Nome] [varchar](50) NOT NULL,
	[Email] [varchar](50) NOT NULL,
	[Pubblico] [bit] NOT NULL,
	[IdRuolo] [int] NOT NULL,
	[IdStato] [int] NOT NULL,
	[DataCandidatura] [datetime] NOT NULL,
	[KeyStruttura] [varchar](61) NOT NULL,
	[IndirizzoIPCandidatura] [varchar](50) NOT NULL,
	[DataInvioServiceDesk] [datetime] NULL,
	[DataProfilazione] [datetime] NULL,
	[DataNotificaCandidatura] [datetime] NULL,
 CONSTRAINT [PK_UtenteDaCensire] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UtenteStruttura](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[idUtente] [int] NOT NULL,
	[codRegione] [varchar](3) NULL,
	[codAzienda] [varchar](3) NULL,
	[idStrutturaErogatrice] [int] NULL,
	[idWebServiceStruttura] [int] NULL,
	[idReparto] [int] NULL,
	[idWebServiceReparto] [int] NULL,
	[dataDal] [datetime] NOT NULL,
	[dataAl] [datetime] NULL,
 CONSTRAINT [PK_UtenteStruttura] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[UtenteDaCensire] ADD  CONSTRAINT [DF_UtenteDaCensire_IdStato]  DEFAULT ((1)) FOR [IdStato]
GO
ALTER TABLE [dbo].[Log]  WITH CHECK ADD  CONSTRAINT [FK_Log_LogType] FOREIGN KEY([type])
REFERENCES [dbo].[LogType] ([id])
GO
ALTER TABLE [dbo].[Log] CHECK CONSTRAINT [FK_Log_LogType]
GO
ALTER TABLE [dbo].[Opportunita]  WITH CHECK ADD  CONSTRAINT [FK_Opportunità_Azioni] FOREIGN KEY([idAzione])
REFERENCES [dbo].[Azione] ([id])
GO
ALTER TABLE [dbo].[Opportunita] CHECK CONSTRAINT [FK_Opportunità_Azioni]
GO
ALTER TABLE [dbo].[Opportunita]  WITH CHECK ADD  CONSTRAINT [FK_Opportunita_Bacteria] FOREIGN KEY([idBacteria])
REFERENCES [dbo].[Bacteria] ([code])
GO
ALTER TABLE [dbo].[Opportunita] CHECK CONSTRAINT [FK_Opportunita_Bacteria]
GO
ALTER TABLE [dbo].[Opportunita]  WITH CHECK ADD  CONSTRAINT [FK_Opportunita_Indicazione] FOREIGN KEY([idIndicazione])
REFERENCES [dbo].[Indicazione] ([id])
GO
ALTER TABLE [dbo].[Opportunita] CHECK CONSTRAINT [FK_Opportunita_Indicazione]
GO
ALTER TABLE [dbo].[Opportunita]  WITH CHECK ADD  CONSTRAINT [FK_Opportunità_Osservazioni] FOREIGN KEY([idOsservazione])
REFERENCES [dbo].[Osservazione] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Opportunita] CHECK CONSTRAINT [FK_Opportunità_Osservazioni]
GO
ALTER TABLE [dbo].[Osservazione]  WITH CHECK ADD  CONSTRAINT [FK_Osservazione_Operatore] FOREIGN KEY([idOperatore])
REFERENCES [dbo].[Operatore] ([id])
GO
ALTER TABLE [dbo].[Osservazione] CHECK CONSTRAINT [FK_Osservazione_Operatore]
GO
ALTER TABLE [dbo].[Osservazione]  WITH CHECK ADD  CONSTRAINT [FK_Osservazioni_Schede] FOREIGN KEY([idScheda])
REFERENCES [dbo].[Scheda] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Osservazione] CHECK CONSTRAINT [FK_Osservazioni_Schede]
GO
ALTER TABLE [dbo].[Scheda]  WITH CHECK ADD  CONSTRAINT [FK_Scheda_StatoSessione] FOREIGN KEY([idStatoSessione])
REFERENCES [dbo].[StatoSessione] ([id])
GO
ALTER TABLE [dbo].[Scheda] CHECK CONSTRAINT [FK_Scheda_StatoSessione]
GO
ALTER TABLE [dbo].[Scheda]  WITH CHECK ADD  CONSTRAINT [FK_Scheda_Utente] FOREIGN KEY([idUtente])
REFERENCES [dbo].[Utente] ([id])
GO
ALTER TABLE [dbo].[Scheda] CHECK CONSTRAINT [FK_Scheda_Utente]
GO
ALTER TABLE [dbo].[Utente]  WITH CHECK ADD  CONSTRAINT [FK_Utenti_Ruoli] FOREIGN KEY([idRuolo])
REFERENCES [dbo].[Ruoli] ([id])
GO
ALTER TABLE [dbo].[Utente] CHECK CONSTRAINT [FK_Utenti_Ruoli]
GO
ALTER TABLE [dbo].[UtenteDaCensire]  WITH CHECK ADD  CONSTRAINT [FK_UtenteDaCensire_Ruoli] FOREIGN KEY([IdRuolo])
REFERENCES [dbo].[Ruoli] ([id])
GO
ALTER TABLE [dbo].[UtenteDaCensire] CHECK CONSTRAINT [FK_UtenteDaCensire_Ruoli]
GO
ALTER TABLE [dbo].[UtenteDaCensire]  WITH CHECK ADD  CONSTRAINT [FK_UtenteDaCensire_StatoCandidatura] FOREIGN KEY([IdStato])
REFERENCES [dbo].[StatoCandidatura] ([Codice])
GO
ALTER TABLE [dbo].[UtenteDaCensire] CHECK CONSTRAINT [FK_UtenteDaCensire_StatoCandidatura]
GO
ALTER TABLE [dbo].[UtenteStruttura]  WITH CHECK ADD  CONSTRAINT [FK_UtenteStruttura_Utente] FOREIGN KEY([idUtente])
REFERENCES [dbo].[Utente] ([id])
GO
ALTER TABLE [dbo].[UtenteStruttura] CHECK CONSTRAINT [FK_UtenteStruttura_Utente]
GO
ALTER TABLE [dbo].[Scheda]  WITH CHECK ADD  CONSTRAINT [CK_Scheda_Reparto] CHECK  (([dbo].[fn_RepartoExist]([idReparto],[idWebServiceReparto],(0))=(1)))
GO
ALTER TABLE [dbo].[Scheda] CHECK CONSTRAINT [CK_Scheda_Reparto]
GO
ALTER TABLE [dbo].[TipologiaStruttura]  WITH CHECK ADD  CONSTRAINT [CK_TipologiaStruttura] CHECK  (([dbo].[fn_AreaDisciplinaExist]([CodAreaDisciplina])=(1)))
GO
ALTER TABLE [dbo].[TipologiaStruttura] CHECK CONSTRAINT [CK_TipologiaStruttura]
GO
ALTER TABLE [dbo].[UtenteDaCensire]  WITH CHECK ADD  CONSTRAINT [CHK_UtenteDaCensire_AziendaAttivaExist] CHECK  (([dbo].[fn_AziendaAttivaExist]([codRegione],[codazienda],(0))=(1)))
GO
ALTER TABLE [dbo].[UtenteDaCensire] CHECK CONSTRAINT [CHK_UtenteDaCensire_AziendaAttivaExist]
GO
ALTER TABLE [dbo].[UtenteDaCensire]  WITH CHECK ADD  CONSTRAINT [CHK_UtenteDaCensire_RegioneExist] CHECK  (([dbo].[fn_RegioneExist]([codregione])=(1)))
GO
ALTER TABLE [dbo].[UtenteDaCensire] CHECK CONSTRAINT [CHK_UtenteDaCensire_RegioneExist]
GO
ALTER TABLE [dbo].[UtenteDaCensire]  WITH CHECK ADD  CONSTRAINT [CHK_UtenteDaCensire_StrutturaExists] CHECK  (([dbo].[fn_KeyStrutturaAttivaExist]([KeyStruttura],(0))=(1)))
GO
ALTER TABLE [dbo].[UtenteDaCensire] CHECK CONSTRAINT [CHK_UtenteDaCensire_StrutturaExists]
GO
ALTER TABLE [dbo].[UtenteStruttura]  WITH CHECK ADD  CONSTRAINT [CHK_UtenteStruttura_AziendaAttivaExist] CHECK  (([dbo].[fn_AziendaAttivaExist]([codRegione],[codazienda],(1))=(1)))
GO
ALTER TABLE [dbo].[UtenteStruttura] CHECK CONSTRAINT [CHK_UtenteStruttura_AziendaAttivaExist]
GO
ALTER TABLE [dbo].[UtenteStruttura]  WITH CHECK ADD  CONSTRAINT [CHK_UtenteStruttura_RegioneExist] CHECK  (([dbo].[fn_RegioneExist]([codregione])=(1)))
GO
ALTER TABLE [dbo].[UtenteStruttura] CHECK CONSTRAINT [CHK_UtenteStruttura_RegioneExist]
GO
ALTER TABLE [dbo].[UtenteStruttura]  WITH CHECK ADD  CONSTRAINT [CHK_UtenteStruttura_RepartoExist] CHECK  (([dbo].[fn_RepartoExist]([idReparto],[idWebServiceReparto],(1))=(1)))
GO
ALTER TABLE [dbo].[UtenteStruttura] CHECK CONSTRAINT [CHK_UtenteStruttura_RepartoExist]
GO
ALTER TABLE [dbo].[UtenteStruttura]  WITH CHECK ADD  CONSTRAINT [CHK_UtenteStruttura_StrutturaExist] CHECK  (([dbo].[fn_StrutturaAttivaExist]([idStrutturaErogatrice],[idWebServiceStruttura],(1))=(1)))
GO
ALTER TABLE [dbo].[UtenteStruttura] CHECK CONSTRAINT [CHK_UtenteStruttura_StrutturaExist]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CandidatiApprovati_Elenca]
	
	AS

BEGIN

SET NOCOUNT ON;

	select Id,
		CodiceFiscale,
		CodRegione,
		CodAzienda,
		Cognome,
		Nome,
		Email,
		Pubblico,
		IdRuolo,
		KeyStruttura,
		DataInvioServiceDesk,
		DataProfilazione
	from UtenteDaCensire
	where IdStato = 1 
	and DataProfilazione is null
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Candidato_Aggiorna]
	@id int,
	@pubblico bit,
	@nome varchar(50),
	@cognome varchar(50),
	@email varchar(50),
	@idruolo int,
	@cf varchar(16),
	@codregione varchar(3),
	@codazienda varchar(3),
	@idstruttura int,
	@idwebservice int,
	@idUtente int OUTPUT,
	@esito BIT OUTPUT
AS

BEGIN

SET NOCOUNT ON;
SET @esito = CONVERT(bit, 0)
	if (@pubblico=0)
	/*il candidato privato viene inserito da qui sulla tabella utenti*/
		BEGIN
			set @idUtente= (select id from Utente where username=@cf)

			if (@idUtente is null)
				BEGIN
					INSERT INTO Utente(
						username,
						nome,
						cognome,
						email,
						idRuolo,
						attivato,
						cancellato,
						CodiceFiscale)
					VALUES (
						@cf,
						@nome,
						@cognome,
						@email,
						@idruolo,
						1,
						0,
						NULL
					)
					set @idUtente = (SELECT SCOPE_IDENTITY())
				END
		END 
	ELSE
	/*il candidato pubblico è stato inserito dal servicedesk. Se è già stato inserito, aggiorno il ruolo e il codicefiscale*/
		BEGIN
			set @idUtente= (select id from Utente where idruolo=5 and (email=@email OR CodiceFiscale=@cf))
			if (@idUtente is not null)
				BEGIN
					UPDATE Utente
					SET idRuolo = @idruolo, CodiceFiscale=@cf
					WHERE id=@idUtente
				END 
		END

		/*inserisco i dati per l'utentestruttura e aggiornare utentedacensire*/
	if (@idutente is not null and not exists(select top 1 u.ID from Utente u inner join UtenteStruttura us on u.id=us.idUtente where idUtente=@idUtente and idRuolo=@idruolo and codRegione=@codregione and codAzienda=@codazienda and idStrutturaErogatrice=@idstruttura and idWebServiceStruttura=@idwebservice))
		BEGIN
			INSERT INTO UtenteStruttura (
				idUtente,
				codRegione,
				codAzienda,
				idStrutturaErogatrice,
				idWebServiceStruttura,
				dataDal	
				)
			VALUES (
				@idUtente,
				@codregione,
				@codazienda,
				@idstruttura,
				@idwebservice,
				GETDATE()
				)

			update UtenteDaCensire
			set IdStato=3,
			dataprofilazione= GETDATE()
			where Id=@id

			SET @esito = CONVERT(bit, 1)
		END
	
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CandidatoPubblico_InviaServiceDesk]
	@id int

	AS

BEGIN

SET NOCOUNT ON;


	update UtenteDaCensire 
	set datainvioservicedesk=GETDATE()
	where Id=@id 

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EsportaOpportunita]
(
	@codRegione varchar(10) = NULL,
	@codAzienda varchar(10) = NULL,
	@codTipoStruttura varchar(10) = NULL,
	@keyStruttura varchar(20) = NULL,
	@keyReparto varchar(20) = NULL,
	@statoSessione int = NULL,
	@dataDal datetime = NULL,
	@dataAl datetime = NULL,
	@cancellate Bit = NULL,
	@idScheda int = NULL
)
AS
 BEGIN
		 SELECT DISTINCT
			s.id IdScheda
			, s.data dataScheda
			, s.dataInserimento dataInserimentoScheda
			, r.CodRegione codiceRegione
			, reg.Denominazione nomeRegione
			, r.CodAzienda codiceAzienda
			, a.Denominazione nomeAzienda
			, r.CodiceTipologiaStruttura 
			, ts.Descrizione nomeTipologiaStruttura
			, sb.codMin codMinStruttura
			, sb.subCodMin subCodMinStruttura
			, sb.IdStrutturaErogatrice
			, sb.idWebServiceStruttura
			, sb.KeyStruttura
			, sb.Denominazione nomeStruttura
			, sb.Indirizzo indirizzoStruttura
			, r.nome nomeDisciplinaArea
			, r.CodDisciplina codiceDisciplina
			, r.NomeOriginale nomeDisciplina
			, r.IdReparto
			, r.IdWebServiceReparto
			, r.KeyReparto
			, r.nome nomeReparto
			, ss.nome statoSessioneScheda
			, s.durataSessione
			, oss.numOperatori numeroOperatori
			, ope.codCategoria codiceCategoriaOperatori
			, ope.nomeCategoria nomeCategoriaOperatore
			, ope.classe classeOperatore
			, oss.operatoreEsterno operatoreEsterno
			, opp.data dataOraOpportunità
			, az.id codiceTipologiaAzione
			, az.tipologia tipologiaAzione
			, ind.id codiceTipologiaIndicazione
			, ind.tipologia TipologiaIndicazione	
			, u.id idUtente
			, ru.nome ruoloUtente
			
		from  Scheda s 
			inner join vwReparto r on s.idReparto = r.IdReparto and s.idWebServiceReparto=r.IdWebServiceReparto
			inner join vwRegione reg on r.CodRegione= reg.CodRegione
			inner join vwAzienda a on r.CodAzienda = a.CodAzienda and r.CodRegione= a.CodRegione and (a.DataInizio<= s.dataInserimento and (a.DataFine is null OR a.DataFine>=s.dataInserimento))
			inner join vwStruttura sb on r.IdStrutturaErogatrice= sb.IdStrutturaErogatrice and r.idWebServiceStruttura= sb.idWebServiceStruttura and (sb.DataRiferimentoStruttura <= s.dataInserimento and (sb.DataFine is null OR sb.DataFine >= s.dataInserimento))
			inner join TipologiaStruttura ts on sb.codiceTipologiaStruttura = ts.CodTipologia
			inner join StatoSessione ss on s.idStatoSessione = ss.id
			inner join Utente u on s.idUtente = u.id
			inner join Ruoli ru on u.idRuolo=ru.id
			left join Osservazione oss on s.id=oss.idScheda
			left join Opportunita opp on oss.id = opp.idOsservazione
			left join Operatore ope on oss.idOperatore = ope.id
			left join Azione az on opp.idAzione = az.id
			left join Indicazione ind on opp.idIndicazione= ind.id

		where 
			(@codRegione is null or r.CodRegione=@codRegione)
			and (@codAzienda is null or r.CodAzienda=@codAzienda)
			and (@codTipoStruttura is null or r.codiceTipologiaStruttura=@codTipoStruttura)
			and (@keyStruttura is null or r.keyStruttura = @keyStruttura)
			and (@keyReparto is null or r.KeyReparto = @keyReparto)
			and (@statoSessione is null or s.idStatoSessione=@statoSessione)
			and (@dataDal is null or s.dataInserimento >= @dataDal)
			and (@dataAl is null or s.data <=@dataAl)
			and (@idScheda is null or s.id=@idScheda)
			and (@cancellate is null or @cancellate=1 Or (@cancellate=0 AND idStatoSessione <> 4) )
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[LogConsolidaScheda_Aggiungi]
	@data datetime,
	@type int,
	@user nvarchar(10),
	@message nvarchar(50)

	AS

BEGIN
	insert into Log ([data], [type], [user], [message]) values (@data, @type, @user, @message)
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Reparto_Salva]
(
	@IdReparto int,
	@IdWebServiceReparto int,
	@IdStrutturaErogatrice int,
	@DataInizioStrutturaErogatrice date,
	@DataInizio date,
	@DataFine date = NULL,
	@IdWebServiceStruttura int,
	@CodDisciplina varchar(3) = NULL,
	@ProgressivoDivisione varchar(2) = NULL,
	@Nome varchar(100) = NULL,
	@Descrizione varchar(100) = NULL,
	@CodAreaDisciplina int,
	@Cancellato bit
)
AS
 BEGIN
	if (@idreparto = 0 )
		BEGIN
			INSERT INTO [GlobalSanita].[dbo].[RepartoCustom](
				DataInizio,
				idWebServiceStruttura,
				idStrutturaErogatrice,
				DataInizioStrutturaErogatrice,
				CodDisciplina,
				ProgressivoDivisione,
				Nome,
				Descrizione,
				CodAreaDisciplina,
				Cancellato,
				TimeStampIns,
				TimeStampVar,
				Stato,
				IdApplicazione
			)
			VALUES (
				GETDATE(),
				@IdWebServiceStruttura,
				@IdStrutturaErogatrice,
				@DataInizioStrutturaErogatrice,
				@CodDisciplina,
				@ProgressivoDivisione,
				@Nome,
				@Descrizione,
				@CodAreaDisciplina,
				@Cancellato,
				GETDATE(),
				GETDATE(),
				3,
				1
			)
		END
	ELSE IF (@IdWebServiceReparto = 1)
		BEGIN
			IF not Exists(select 1 from [GlobalSanita].[dbo].[RepartoAlias] where IdReparto=@IdReparto)
				BEGIN
					INSERT INTO [GlobalSanita].[dbo].[RepartoAlias](
						IdReparto,
						Nome,
						Descrizione,
						Cancellato,
						Stato,
						IdApplicazione
					)
					VALUES (
						@IdReparto,
						@Nome,
						@Descrizione,
						@Cancellato,
						2,
						1
					)
				END 
			ELSE
				BEGIN
					UPDATE [GlobalSanita].[dbo].[RepartoAlias]
					SET
						Nome= @Nome,
						Descrizione=@Descrizione,
						Cancellato=@Cancellato
					WHERE 
						IdReparto=@IdReparto
				END 
		END
	ELSE 
		BEGIN
			UPDATE [GlobalSanita].[dbo].[RepartoCustom]
			SET 
				DataFine = @DataFine,
			 	IdStrutturaErogatrice= @IdStrutturaErogatrice,
				DataInizioStrutturaErogatrice = @DataInizioStrutturaErogatrice,
				idWebServiceStruttura= @IdWebServiceStruttura,
				CodDisciplina= @CodDisciplina,
				ProgressivoDivisione= @ProgressivoDivisione,
				Nome= @Nome,
				Descrizione= @Descrizione,
				CodAreaDisciplina= @CodAreaDisciplina,
				TimeStampVar = GETDATE(),
				Cancellato= @Cancellato,
				Stato = 3,
				IdApplicazione=1
			WHERE 
				IdReparto= @IdReparto
		END
END

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Verifica esistenza KeyStruttura' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UtenteDaCensire', @level2type=N'CONSTRAINT',@level2name=N'CHK_UtenteDaCensire_StrutturaExists'
GO
