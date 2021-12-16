CREATE TABLE [dbo].[Azienda] (
    [CodRegione]       VARCHAR (3)    NOT NULL,
    [CodAzienda]       VARCHAR (3)    NOT NULL,
    [DataInizio]       DATE           NOT NULL,
    [DataFine]         DATE           NULL,
    [CodTipoAzienda]   VARCHAR (5)    NOT NULL,
    [Denominazione]    VARCHAR (1000) NOT NULL,
    [DescrizioneBreve] VARCHAR (60)   NOT NULL,
    [SiglaAzienda]     VARCHAR (30)   NULL,
    [Provincia]        VARCHAR (4)    NULL,
    [CodAziendaUsl]    VARCHAR (6)    NULL,
    [PartitaIva]       VARCHAR (11)   NULL,
    [TimeStampIns]     DATETIME       NOT NULL,
    [TimeStampVar]     DATETIME       NOT NULL,
    PRIMARY KEY CLUSTERED ([CodRegione] ASC, [CodAzienda] ASC, [DataInizio] ASC)
);

