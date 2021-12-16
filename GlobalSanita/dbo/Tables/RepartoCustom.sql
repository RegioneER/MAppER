CREATE TABLE [dbo].[RepartoCustom] (
    [IdReparto]                     INT           IDENTITY (1, 1) NOT NULL,
    [DataInizio]                    DATE          NOT NULL,
    [DataFine]                      DATE          NULL,
    [idWebServiceStruttura]         INT           NOT NULL,
    [idStrutturaErogatrice]         INT           NOT NULL,
    [DataInizioStrutturaErogatrice] DATE          NOT NULL,
    [CodDisciplina]                 VARCHAR (3)   NULL,
    [ProgressivoDivisione]          VARCHAR (2)   NULL,
    [Nome]                          VARCHAR (100) NOT NULL,
    [Descrizione]                   VARCHAR (100) NULL,
    [CodAreaDisciplina]             INT           NOT NULL,
    [TimeStampIns]                  DATETIME      NOT NULL,
    [TimeStampVar]                  DATETIME      NOT NULL,
    [Cancellato]                    BIT           CONSTRAINT [DF_RepartoCustom_Cancellato] DEFAULT ((0)) NOT NULL,
    [Stato]                         INT           CONSTRAINT [DF_RepartoCustom_Stato] DEFAULT ((3)) NOT NULL,
    [IdApplicazione]                INT           NOT NULL,
    CONSTRAINT [PK_RepartoCustom] PRIMARY KEY CLUSTERED ([IdReparto] ASC),
    CONSTRAINT [FK_RepartoCustom_Applicazione] FOREIGN KEY ([IdApplicazione]) REFERENCES [dbo].[Applicazione] ([IdApplicazione]),
    CONSTRAINT [FK_RepartoCustom_AreaDisciplina] FOREIGN KEY ([CodAreaDisciplina]) REFERENCES [dbo].[AreaDisciplina] ([CodAreaDisciplina]),
    CONSTRAINT [FK_RepartoCustom_Disciplina] FOREIGN KEY ([CodDisciplina]) REFERENCES [dbo].[Disciplina] ([CodDisciplina]),
    CONSTRAINT [FK_RepartoCustom_Struttura] FOREIGN KEY ([idStrutturaErogatrice], [DataInizioStrutturaErogatrice]) REFERENCES [dbo].[Struttura] ([IdStrutturaErogatrice], [DataInizio])
);

