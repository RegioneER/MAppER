USE [GlobalSanita]
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
CREATE TABLE [dbo].[Applicazione](
	[IdApplicazione] [int] NOT NULL,
	[Nome] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Applicazione] PRIMARY KEY CLUSTERED 
(
	[IdApplicazione] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AreaDisciplina](
	[CodAreaDisciplina] [int] NOT NULL,
	[Nome] [varchar](50) NOT NULL,
	[Descrizione] [varchar](1000) NULL,
	[Ordinale] [int] NOT NULL,
	[IdApplicazione] [int] NOT NULL,
 CONSTRAINT [PK_AreaDisciplina] PRIMARY KEY CLUSTERED 
(
	[CodAreaDisciplina] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Azienda](
	[CodRegione] [varchar](3) NOT NULL,
	[CodAzienda] [varchar](3) NOT NULL,
	[DataInizio] [date] NOT NULL,
	[DataFine] [date] NULL,
	[CodTipoAzienda] [varchar](5) NOT NULL,
	[Denominazione] [varchar](1000) NOT NULL,
	[DescrizioneBreve] [varchar](60) NOT NULL,
	[SiglaAzienda] [varchar](30) NULL,
	[Provincia] [varchar](4) NULL,
	[CodAziendaUsl] [varchar](6) NULL,
	[PartitaIva] [varchar](11) NULL,
	[TimeStampIns] [datetime] NOT NULL,
	[TimeStampVar] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CodRegione] ASC,
	[CodAzienda] ASC,
	[DataInizio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConvenzionePrivacy](
	[KeyStruttura] [varchar](61) NOT NULL,
	[InConvenzione] [bit] NOT NULL,
	[Dal] [date] NOT NULL,
	[Al] [date] NULL,
	[IdApplicazione] [int] NOT NULL,
 CONSTRAINT [PK_ConvenzionePrivacy] PRIMARY KEY CLUSTERED 
(
	[KeyStruttura] ASC,
	[Dal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Disciplina](
	[CodDisciplina] [varchar](3) NOT NULL,
	[DataInizio] [date] NULL,
	[Descrizione] [varchar](60) NOT NULL,
	[DataFine] [date] NULL,
	[TimeStampIns] [datetime] NOT NULL,
	[TimeStampVar] [datetime] NOT NULL,
 CONSTRAINT [PK_Disciplina] PRIMARY KEY CLUSTERED 
(
	[CodDisciplina] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DisciplinaAreaDisciplina](
	[CodDisciplina] [varchar](3) NOT NULL,
	[CodAreaDisciplina] [int] NOT NULL,
	[IdApplicazione] [int] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FiltroTipoAnagrafeRegionale](
	[CodTipoAnagrafeRegionale] [varchar](4) NOT NULL,
	[DataInizioTipoAnagrafeRegionale] [date] NOT NULL,
	[IdApplicazione] [int] NOT NULL,
 CONSTRAINT [PK_FiltroTipoAnagrafeRegionale] PRIMARY KEY CLUSTERED 
(
	[CodTipoAnagrafeRegionale] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Intervento](
	[CodIntervento] [varchar](4) NOT NULL,
	[DataInizio] [date] NOT NULL,
	[DataFine] [date] NULL,
	[Descrizione] [varchar](120) NOT NULL,
	[DescrizioneBreve] [varchar](80) NULL,
	[TimestampIns] [datetime] NOT NULL,
	[TimestampVar] [datetime] NOT NULL,
 CONSTRAINT [PK_Intervento] PRIMARY KEY CLUSTERED 
(
	[CodIntervento] ASC,
	[DataInizio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Regione](
	[CodRegione] [varchar](3) NOT NULL,
	[Denominazione] [varchar](50) NOT NULL,
	[TimeStampIns] [date] NOT NULL,
	[TimeStampVar] [date] NOT NULL,
 CONSTRAINT [PK_Regione] PRIMARY KEY CLUSTERED 
(
	[CodRegione] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reparto](
	[IdReparto] [int] IDENTITY(1,1) NOT NULL,
	[IdStrutturaErogatrice] [int] NOT NULL,
	[DataInizioStrutturaErogatrice] [date] NOT NULL,
	[DataInizio] [date] NOT NULL,
	[DataFine] [date] NULL,
	[CodDisciplina] [varchar](3) NOT NULL,
	[ProgressivoDivisione] [varchar](2) NOT NULL,
	[TipoDivisione] [varchar](3) NULL,
	[PostiLettoDayHospital] [int] NOT NULL,
	[PostiLettoDegenzeOrdinarie] [int] NOT NULL,
	[PostiLettoDegenzeOrdinariePagamento] [int] NOT NULL,
	[PostiLettoDaySurgery] [int] NOT NULL,
	[AssistenzaFamiliare] [varchar](1) NULL,
	[Modello] [varchar](50) NOT NULL,
	[TimeStampIns] [datetime] NOT NULL,
	[TimeStampVar] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdReparto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RepartoAlias](
	[IdReparto] [int] NOT NULL,
	[Nome] [varchar](1000) NOT NULL,
	[Descrizione] [varchar](1000) NULL,
	[Cancellato] [bit] NOT NULL,
	[Stato] [int] NOT NULL,
	[IdApplicazione] [int] NOT NULL,
 CONSTRAINT [PK_RepartoAlias] PRIMARY KEY CLUSTERED 
(
	[IdReparto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RepartoCustom](
	[IdReparto] [int] IDENTITY(1,1) NOT NULL,
	[DataInizio] [date] NOT NULL,
	[DataFine] [date] NULL,
	[idWebServiceStruttura] [int] NOT NULL,
	[idStrutturaErogatrice] [int] NOT NULL,
	[DataInizioStrutturaErogatrice] [date] NOT NULL,
	[CodDisciplina] [varchar](3) NULL,
	[ProgressivoDivisione] [varchar](2) NULL,
	[Nome] [varchar](100) NOT NULL,
	[Descrizione] [varchar](100) NULL,
	[CodAreaDisciplina] [int] NOT NULL,
	[TimeStampIns] [datetime] NOT NULL,
	[TimeStampVar] [datetime] NOT NULL,
	[Cancellato] [bit] NOT NULL,
	[Stato] [int] NOT NULL,
	[IdApplicazione] [int] NOT NULL,
 CONSTRAINT [PK_RepartoCustom] PRIMARY KEY CLUSTERED 
(
	[IdReparto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Struttura](
	[IdStrutturaErogatrice] [int] NOT NULL,
	[DataInizio] [date] NOT NULL,
	[DataFine] [date] NULL,
	[DataApertura] [date] NOT NULL,
	[DataChiusura] [date] NULL,
	[Denominazione] [varchar](150) NOT NULL,
	[CodRegione] [varchar](3) NOT NULL,
	[CodAzienda] [varchar](3) NOT NULL,
	[DataInizioAzienda] [date] NOT NULL,
	[CodMin] [varchar](6) NOT NULL,
	[SubCodMin] [varchar](2) NOT NULL,
	[IdCodStruttura] [int] NOT NULL,
	[CodTipoAnagrafeRegionale] [varchar](4) NOT NULL,
	[DataInizioTipoAnagrafeRegionale] [date] NOT NULL,
	[PubblicoPrivato] [varchar](1) NULL,
	[IndirizzoAggiuntivo] [varchar](100) NULL,
	[CapAggiuntivo] [varchar](5) NULL,
	[IdStrutturaFisica] [int] NOT NULL,
	[DataInizioStrutturaFisica] [date] NOT NULL,
	[TimeStampIns] [datetime] NOT NULL,
	[TimeStampVar] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdStrutturaErogatrice] ASC,
	[DataInizio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StrutturaCustom](
	[IdStrutturaErogatrice] [int] NOT NULL,
	[DataInizio] [date] NOT NULL,
	[DataFine] [date] NULL,
	[DataApertura] [date] NOT NULL,
	[DataChiusura] [date] NULL,
	[Denominazione] [varchar](150) NOT NULL,
	[CodRegione] [varchar](3) NOT NULL,
	[CodAzienda] [varchar](3) NOT NULL,
	[DataInizioAzienda] [date] NOT NULL,
	[CodMin] [varchar](6) NOT NULL,
	[SubCodMin] [varchar](3) NOT NULL,
	[IdCodStruttura] [int] NOT NULL,
	[CodTipoAnagrafeRegionale] [varchar](4) NOT NULL,
	[DataInizioTipoAnagrafeRegionale] [date] NOT NULL,
	[PubblicoPrivato] [varchar](1) NULL,
	[IndirizzoAggiuntivo] [varchar](100) NULL,
	[CapAggiuntivo] [varchar](5) NULL,
	[IdStrutturaFisica] [int] NOT NULL,
	[DataInizioStrutturaFisica] [date] NOT NULL,
	[TimeStampIns] [datetime] NOT NULL,
	[TimeStampVar] [datetime] NOT NULL,
	[Stato] [int] NOT NULL,
	[IdApplicazione] [int] NOT NULL,
 CONSTRAINT [PK_StrutturaCustom] PRIMARY KEY CLUSTERED 
(
	[IdStrutturaErogatrice] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoAnagrafeRegionale](
	[CodTipoAnagrafeRegionale] [varchar](4) NOT NULL,
	[DataInizio] [date] NOT NULL,
	[DataFine] [date] NULL,
	[Descrizione] [varchar](100) NOT NULL,
	[CodMacroArea] [int] NOT NULL,
	[CodTarget] [int] NULL,
	[CodTipoStrutturaMinisteriale] [varchar](2) NULL,
	[CodTipoStrutturaMinistarialeInterna] [varchar](3) NOT NULL,
	[TimeStampIns] [datetime] NOT NULL,
	[TimeStampVar] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CodTipoAnagrafeRegionale] ASC,
	[DataInizio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Disciplina] ADD  CONSTRAINT [DF_Disciplina_TimeStampIns]  DEFAULT (getdate()) FOR [TimeStampIns]
GO
ALTER TABLE [dbo].[DisciplinaAreaDisciplina] ADD  CONSTRAINT [DF_DisciplinaAreaDisciplina_IdApplicazione]  DEFAULT ((1)) FOR [IdApplicazione]
GO
ALTER TABLE [dbo].[RepartoAlias] ADD  CONSTRAINT [DF_RepartoAlias_Cancellato]  DEFAULT ((0)) FOR [Cancellato]
GO
ALTER TABLE [dbo].[RepartoAlias] ADD  CONSTRAINT [DF_RepartoAlias_Stato]  DEFAULT ((2)) FOR [Stato]
GO
ALTER TABLE [dbo].[RepartoCustom] ADD  CONSTRAINT [DF_RepartoCustom_Cancellato]  DEFAULT ((0)) FOR [Cancellato]
GO
ALTER TABLE [dbo].[RepartoCustom] ADD  CONSTRAINT [DF_RepartoCustom_Stato]  DEFAULT ((3)) FOR [Stato]
GO
ALTER TABLE [dbo].[StrutturaCustom] ADD  CONSTRAINT [DF_StrutturaCustom_Stato]  DEFAULT ((1)) FOR [Stato]
GO
ALTER TABLE [dbo].[AreaDisciplina]  WITH CHECK ADD  CONSTRAINT [FK_AreaDisciplina_Applicazione] FOREIGN KEY([IdApplicazione])
REFERENCES [dbo].[Applicazione] ([IdApplicazione])
GO
ALTER TABLE [dbo].[AreaDisciplina] CHECK CONSTRAINT [FK_AreaDisciplina_Applicazione]
GO
ALTER TABLE [dbo].[ConvenzionePrivacy]  WITH CHECK ADD  CONSTRAINT [FK_ConvenzionePrivacy_Applicazione] FOREIGN KEY([IdApplicazione])
REFERENCES [dbo].[Applicazione] ([IdApplicazione])
GO
ALTER TABLE [dbo].[ConvenzionePrivacy] CHECK CONSTRAINT [FK_ConvenzionePrivacy_Applicazione]
GO
ALTER TABLE [dbo].[FiltroTipoAnagrafeRegionale]  WITH CHECK ADD  CONSTRAINT [FK_FiltroTipoAnagrafeRegionale_Applicazione] FOREIGN KEY([IdApplicazione])
REFERENCES [dbo].[Applicazione] ([IdApplicazione])
GO
ALTER TABLE [dbo].[FiltroTipoAnagrafeRegionale] CHECK CONSTRAINT [FK_FiltroTipoAnagrafeRegionale_Applicazione]
GO
ALTER TABLE [dbo].[FiltroTipoAnagrafeRegionale]  WITH CHECK ADD  CONSTRAINT [FK_FiltroTipoAnagrafeRegionale_TipoAnagrafeRegionale] FOREIGN KEY([CodTipoAnagrafeRegionale], [DataInizioTipoAnagrafeRegionale])
REFERENCES [dbo].[TipoAnagrafeRegionale] ([CodTipoAnagrafeRegionale], [DataInizio])
GO
ALTER TABLE [dbo].[FiltroTipoAnagrafeRegionale] CHECK CONSTRAINT [FK_FiltroTipoAnagrafeRegionale_TipoAnagrafeRegionale]
GO
ALTER TABLE [dbo].[Reparto]  WITH CHECK ADD FOREIGN KEY([IdStrutturaErogatrice], [DataInizioStrutturaErogatrice])
REFERENCES [dbo].[Struttura] ([IdStrutturaErogatrice], [DataInizio])
GO
ALTER TABLE [dbo].[Reparto]  WITH CHECK ADD  CONSTRAINT [FK__Reparto__CodDisc__5C1A1321] FOREIGN KEY([CodDisciplina])
REFERENCES [dbo].[Disciplina] ([CodDisciplina])
GO
ALTER TABLE [dbo].[Reparto] CHECK CONSTRAINT [FK__Reparto__CodDisc__5C1A1321]
GO
ALTER TABLE [dbo].[RepartoAlias]  WITH CHECK ADD  CONSTRAINT [FK_RepartoAlias_Applicazione] FOREIGN KEY([IdApplicazione])
REFERENCES [dbo].[Applicazione] ([IdApplicazione])
GO
ALTER TABLE [dbo].[RepartoAlias] CHECK CONSTRAINT [FK_RepartoAlias_Applicazione]
GO
ALTER TABLE [dbo].[RepartoCustom]  WITH CHECK ADD  CONSTRAINT [FK_RepartoCustom_Applicazione] FOREIGN KEY([IdApplicazione])
REFERENCES [dbo].[Applicazione] ([IdApplicazione])
GO
ALTER TABLE [dbo].[RepartoCustom] CHECK CONSTRAINT [FK_RepartoCustom_Applicazione]
GO
ALTER TABLE [dbo].[RepartoCustom]  WITH CHECK ADD  CONSTRAINT [FK_RepartoCustom_AreaDisciplina] FOREIGN KEY([CodAreaDisciplina])
REFERENCES [dbo].[AreaDisciplina] ([CodAreaDisciplina])
GO
ALTER TABLE [dbo].[RepartoCustom] CHECK CONSTRAINT [FK_RepartoCustom_AreaDisciplina]
GO
ALTER TABLE [dbo].[RepartoCustom]  WITH CHECK ADD  CONSTRAINT [FK_RepartoCustom_Disciplina] FOREIGN KEY([CodDisciplina])
REFERENCES [dbo].[Disciplina] ([CodDisciplina])
GO
ALTER TABLE [dbo].[RepartoCustom] CHECK CONSTRAINT [FK_RepartoCustom_Disciplina]
GO
ALTER TABLE [dbo].[RepartoCustom]  WITH CHECK ADD  CONSTRAINT [FK_RepartoCustom_Struttura] FOREIGN KEY([idStrutturaErogatrice], [DataInizioStrutturaErogatrice])
REFERENCES [dbo].[Struttura] ([IdStrutturaErogatrice], [DataInizio])
GO
ALTER TABLE [dbo].[RepartoCustom] CHECK CONSTRAINT [FK_RepartoCustom_Struttura]
GO
ALTER TABLE [dbo].[Struttura]  WITH CHECK ADD FOREIGN KEY([CodTipoAnagrafeRegionale], [DataInizioTipoAnagrafeRegionale])
REFERENCES [dbo].[TipoAnagrafeRegionale] ([CodTipoAnagrafeRegionale], [DataInizio])
GO
ALTER TABLE [dbo].[Struttura]  WITH CHECK ADD FOREIGN KEY([CodRegione], [CodAzienda], [DataInizioAzienda])
REFERENCES [dbo].[Azienda] ([CodRegione], [CodAzienda], [DataInizio])
GO
ALTER TABLE [dbo].[StrutturaCustom]  WITH CHECK ADD  CONSTRAINT [FK_StrutturaCustom_Applicazione] FOREIGN KEY([IdApplicazione])
REFERENCES [dbo].[Applicazione] ([IdApplicazione])
GO
ALTER TABLE [dbo].[StrutturaCustom] CHECK CONSTRAINT [FK_StrutturaCustom_Applicazione]
GO
ALTER TABLE [dbo].[StrutturaCustom]  WITH CHECK ADD  CONSTRAINT [FK_StrutturaCustom_Azienda] FOREIGN KEY([CodRegione], [CodAzienda], [DataInizioAzienda])
REFERENCES [dbo].[Azienda] ([CodRegione], [CodAzienda], [DataInizio])
GO
ALTER TABLE [dbo].[StrutturaCustom] CHECK CONSTRAINT [FK_StrutturaCustom_Azienda]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Azienda_Salva]
(
	@CodRegione varchar(3),
	@CodAzienda varchar(3),
	@DataInizio date,
	@DataFine date = NULL,	
	@CodTipoAzienda varchar(5),
	@Denominazione varchar(1000),
	@DescrizioneBreve varchar(60),
	@SiglaAzienda varchar(30) = NULL,
	@Provincia varchar(4) = NULL,
	@CodAziendaUsl varchar(6) = NULL,
	@PartitaIva varchar(11) = NULL,
	@TimeStampIns datetime,
	@TimeStampVar datetime
)
AS
 BEGIN

 if (not exists
				(
					select 1 
					from Azienda a 
					where a.CodRegione = @CodRegione
						and a .CodAzienda = @CodAzienda
						and a.DataInizio = @DataInizio
				)
	)	
	BEGIN
	INSERT INTO Azienda(
		CodRegione,
		CodAzienda,
		DataInizio,
		DataFine,
		CodTipoAzienda,
		Denominazione,
		DescrizioneBreve,
		SiglaAzienda,
		Provincia,
		CodAziendaUsl,
		PartitaIva,
		TimeStampIns,
		TimeStampVar
	)
	VALUES (
		@CodRegione,
		@CodAzienda,
		@DataInizio,
		@DataFine,
		@CodTipoAzienda,
		@Denominazione,
		@DescrizioneBreve,
		@SiglaAzienda,
		@Provincia,
		@CodAziendaUsl,
		@PartitaIva,
		@TimeStampIns,
		@TimeStampVar
	)
	END
ELSE
	BEGIN
		UPDATE Azienda 
		SET 
			DataFine = @DataFine,
			CodTipoAzienda = @CodTipoAzienda,
			Denominazione = @Denominazione,
			DescrizioneBreve= @DescrizioneBreve,
			SiglaAzienda = @SiglaAzienda,
			Provincia = @Provincia,
			CodAziendaUsl = @CodAziendaUsl,
			PartitaIva = @PartitaIva,
			TimeStampIns= @TimeStampIns,
			TimeStampVar = @TimeStampVar
			
		WHERE 
			CodAzienda= @CodAzienda
			AND CodRegione = @CodRegione
			AND DataInizio = @DataInizio
	END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DataUltimoAggiornamento_Elenca]
(
	@nomeTabella varchar(50)
)
AS
 BEGIN

DECLARE @SQLString NVARCHAR(500);  
DECLARE @ParmDefinition NVARCHAR(500);  
DECLARE @max_date DATE;  

SET @SQLString = N'SELECT @max_dateOUT = max(TimeStampVar)   
   FROM ' + @nomeTabella;  
SET @ParmDefinition = N'@max_dateOUT DATE OUTPUT';  

EXEC sp_executesql @SQLString, @ParmDefinition, @max_dateOUT = @max_date OUTPUT;  

select @max_date DataUltimoAggiornamento



END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Disciplina_Salva]
(
	@CodDisciplina varchar(3),
	@DataInizio date = NULL,
	@Descrizione varchar(60),
	@DataFine date = NULL,
	@TimeStampIns datetime,
	@TimeStampVar datetime
)
AS
 BEGIN

if (not exists
				(
					select 1 
					from Disciplina d
					where d.CodDisciplina = @CodDisciplina
				)
	)	
	BEGIN
	INSERT INTO Disciplina(
	CodDisciplina,
	DataInizio,
	Descrizione,
	DataFine,
		TimeStampIns,
		TimeStampVar
	)
	VALUES (
		@CodDisciplina,
		@DataInizio,
		@Descrizione,
		@DataFine,
		@TimeStampIns,
		@TimeStampVar
	)
	END
ELSE
	BEGIN
		UPDATE Disciplina 
		SET 
			DataInizio = @DataInizio,
			Descrizione = @Descrizione,
			DataFine = @DataFine,
			TimeStampIns= @TimeStampIns,
			TimeStampVar = @TimeStampVar
			
		WHERE 
			CodDisciplina= @CodDisciplina
	END
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Intervento_Salva]
(
	@CodIntervento varchar(4),
	@DataInizio date = NULL,
	@DataFine date = NULL,
	@Descrizione varchar(120),
	@DescrizioneBreve varchar(80),
	@TimeStampIns datetime,
	@TimeStampVar datetime
)
AS
 BEGIN

 if (not exists
				(
					select 1 
					from Intervento i
					where i.CodIntervento = @CodIntervento
				)
	)	
	BEGIN
	INSERT INTO Intervento(
CodIntervento,
DataInizio,
DataFine,
Descrizione,
DescrizioneBreve,
		TimestampIns,
		TimestampVar
	)
	VALUES (
		@CodIntervento,
		@DataInizio,		
		@DataFine,
		@Descrizione,
		@DescrizioneBreve,
		@TimeStampIns,
		@TimeStampVar
	)
	END
ELSE
	BEGIN
		UPDATE Intervento 
		SET 
			DataInizio = @DataInizio,
			DataFine = @DataFine,
			Descrizione = @Descrizione,
			DescrizioneBreve = @DescrizioneBreve,
			TimestampIns = @TimeStampIns,
			TimestampVar = @TimeStampVar
		WHERE 
			CodIntervento= @CodIntervento
	END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Reparto_Salva]
(
	@IdStrutturaErogatrice int,
	@DataInizioStrutturaErogatrice date,
	@DataInizio date,
	@DataFine date=NULL,
	@CodDisciplina varchar(3),
	@ProgressivoDivisione varchar(2),
	@TipoDivisione varchar(3) = NULL,
	@PostiLettoDayHospital int,
	@PostiLettoDegenzeOrdinarie int,
	@PostiLettoDegenzeOrdinariePagamento int,
	@PostiLettoDaySurgery int,
	@AssistenzaFamiliare varchar(1) = NULL,
	@Modello varchar(50) = NULL,
	@TimeStampIns datetime,
	@TimeStampVar datetime
)
AS
 BEGIN

 if (not exists
				(
					select 1 
					from Reparto r
					where r.IdStrutturaErogatrice = @IdStrutturaErogatrice
						AND r.DataInizioStrutturaErogatrice = @DataInizioStrutturaErogatrice
						AND r.CodDisciplina = @CodDisciplina
						AND r.ProgressivoDivisione = @ProgressivoDivisione
				)
	)	
	BEGIN
	INSERT INTO Reparto(
		IdStrutturaErogatrice,
		DataInizioStrutturaErogatrice,
		DataInizio,
		DataFine,
		CodDisciplina,
		ProgressivoDivisione,
		TipoDivisione,
		PostiLettoDayHospital,
		PostiLettoDegenzeOrdinarie,
		PostiLettoDegenzeOrdinariePagamento,
		PostiLettoDaySurgery,
		AssistenzaFamiliare,
		Modello,
		TimeStampIns,
		TimeStampVar
	)
	VALUES (
		@IdStrutturaErogatrice,
		@DataInizioStrutturaErogatrice,
		@DataInizio,
		@DataFine,
		@CodDisciplina,
		@ProgressivoDivisione, 
		@TipoDivisione,
		@PostiLettoDayHospital, 
		@PostiLettoDegenzeOrdinarie, 
		@PostiLettoDegenzeOrdinariePagamento,
		@PostiLettoDaySurgery,
		@AssistenzaFamiliare,
		@Modello,
		@TimeStampIns,
		@TimeStampVar
	)
	END
ELSE
	BEGIN
		UPDATE Reparto 
		SET 
			DataInizio = @DataInizio,
			DataFine = @DataFine,
			TipoDivisione=@TipoDivisione,
			PostiLettoDayHospital = @PostiLettoDayHospital,
			PostiLettoDegenzeOrdinarie = @PostiLettoDegenzeOrdinarie,
			PostiLettoDegenzeOrdinariePagamento = @PostiLettoDegenzeOrdinariePagamento,
			PostiLettoDaySurgery = @PostiLettoDaySurgery,
			AssistenzaFamiliare = @AssistenzaFamiliare,
			Modello = @Modello,
			TimeStampIns= @TimeStampIns,
			TimeStampVar = @TimeStampVar
			
		WHERE 
			IdStrutturaErogatrice = @IdStrutturaErogatrice
			AND DataInizioStrutturaErogatrice = @DataInizioStrutturaErogatrice
			AND CodDisciplina = @CodDisciplina
			AND ProgressivoDivisione = @ProgressivoDivisione
	END
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Struttura_Salva]
(
	@IdStrutturaErogatrice int,
	@DataInizio date,
	@DataFine date = NULL,
	@DataApertura date,
	@DataChiusura date = NULL,
	@Denominazione varchar(150),
	@CodRegione varchar(3),
	@CodAzienda varchar(3),
	@DataInizioAzienda date,
	@CodMin varchar(6),
	@SubCodMin varchar(2),
	@IdCodStruttura int, 
	@CodTipoAnagrafeRegionale varchar(4),
	@DataInizioTipoAnagrafeRegionale date,
	@PubblicoPrivato varchar(1) =NULL,
	@IndirizzoAggiuntivo varchar(100) = NULL,
	@CapAggiuntivo varchar(5) = NULL,
	@IdStrutturaFisica int,
	@DataInizioStrutturaFisica date,
	@TimeStampIns datetime,
	@TimeStampVar datetime
)
AS
 BEGIN

 if (not exists
				(
					select 1 
					from Struttura s
					where s.IdStrutturaErogatrice = @IdStrutturaErogatrice
						and s.DataInizio = @DataInizio
				)
	)	
	BEGIN
	INSERT INTO Struttura(
		IdStrutturaErogatrice,
		DataInizio,
		DataFine,
		DataApertura,
		DataChiusura,
		Denominazione,
		CodRegione,
		CodAzienda,
		DataInizioAzienda,
		CodMin,
		SubCodMin,
		IdCodStruttura,
		CodTipoAnagrafeRegionale,
		DataInizioTipoAnagrafeRegionale,
		PubblicoPrivato,
		IndirizzoAggiuntivo,
		CapAggiuntivo,
		IdStrutturaFisica,
		DataInizioStrutturaFisica,
		TimeStampIns,
		TimeStampVar
	)
	VALUES (
		@IdStrutturaErogatrice,
		@DataInizio,
		@DataFine,
		@DataApertura,
		@DataChiusura,
		@Denominazione,
		@CodRegione,
		@CodAzienda,
		@DataInizioAzienda,
		@CodMin,
		@SubCodMin,
		@IdCodStruttura,
		@CodTipoAnagrafeRegionale,
		@DataInizioTipoAnagrafeRegionale,
		@PubblicoPrivato,
		@IndirizzoAggiuntivo,
		@CapAggiuntivo,
		@IdStrutturaFisica,
		@DataInizioStrutturaFisica,
		@TimeStampIns,
		@TimeStampVar
		)
	END
ELSE
	BEGIN
		UPDATE Struttura 
		SET 
			DataFine = @DataFine,
			DataApertura= @DataApertura,
			DataChiusura = @DataChiusura,
			Denominazione = @Denominazione,
			CodRegione = @CodRegione,
			CodAzienda = @CodAzienda,
			DataInizioAzienda = @DataInizioAzienda,
			CodMin = @CodMin,
			SubCodMin = @SubCodMin,
			IdCodStruttura = @IdCodStruttura,
			CodTipoAnagrafeRegionale = @CodTipoAnagrafeRegionale,
			DataInizioTipoAnagrafeRegionale = @DataInizioTipoAnagrafeRegionale,
			PubblicoPrivato =@PubblicoPrivato,
			IndirizzoAggiuntivo = @IndirizzoAggiuntivo,
			CapAggiuntivo = @CapAggiuntivo,
			IdStrutturaFisica = @IdStrutturaFisica,
			DataInizioStrutturaFisica = @DataInizioStrutturaFisica,
			TimeStampIns = @TimeStampIns,
			TimeStampVar= @TimeStampVar
			
		WHERE 
			IdStrutturaErogatrice= @IdStrutturaErogatrice
			AND  DataInizio = @DataInizio
	END
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TipoAnagrafeRegionale_Salva]
(
	@CodTipoAnagrafeRegionale varchar(4),
	@DataInizio date,
	@DataFine date = NULL,
	@Descrizione varchar(100),
	@CodMacroArea int,
	@CodTarget int = NULL,
	@CodTipoStrutturaMinisteriale varchar(2) = NULL,
	@CodTipoStrutturaMinisterialeInterno varchar(3),
	@TimeStampIns datetime,
	@TimeStampVar datetime
)
AS
 BEGIN

 if (not exists
				(
					select 1 
					from TipoAnagrafeRegionale tar
					where tar.CodTipoAnagrafeRegionale = @CodTipoAnagrafeRegionale
						and tar.DataInizio = @DataInizio 
				)
	)	
	BEGIN
	INSERT INTO TipoAnagrafeRegionale(
		CodTipoAnagrafeRegionale,
		DataInizio,
		DataFine,
		Descrizione,
		CodMacroArea,
		CodTarget,
		CodTipoStrutturaMinisteriale,
		CodTipoStrutturaMinistarialeInterna,
		TimeStampIns,
		TimeStampVar
	)
	VALUES (
		@CodTipoAnagrafeRegionale,
		@DataInizio,
		@DataFine,
		@Descrizione,
		@CodMacroArea,
		@CodTarget,
		@CodTipoStrutturaMinisteriale,
		@CodTipoStrutturaMinisterialeInterno,
		@TimeStampIns,
		@TimeStampVar
	)
	END
ELSE
	BEGIN
		UPDATE TipoAnagrafeRegionale 
		SET 
			DataFine = @DataFine,
			Descrizione = @Descrizione,
			CodMacroArea = @CodMacroArea,
			CodTarget = @CodTarget,
			CodTipoStrutturaMinisteriale = @CodTipoStrutturaMinisteriale,
			CodTipoStrutturaMinistarialeInterna = @CodTipoStrutturaMinisterialeInterno,
			TimeStampIns= @TimeStampIns,
			TimeStampVar = @TimeStampVar
			
		WHERE 
			CodTipoAnagrafeRegionale= @CodTipoAnagrafeRegionale
			AND  DataInizio = @DataInizio
	END
END
GO
