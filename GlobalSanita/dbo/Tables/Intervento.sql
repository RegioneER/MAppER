CREATE TABLE [dbo].[Intervento] (
    [CodIntervento]    VARCHAR (4)   NOT NULL,
    [DataInizio]       DATE          NOT NULL,
    [DataFine]         DATE          NULL,
    [Descrizione]      VARCHAR (120) NOT NULL,
    [DescrizioneBreve] VARCHAR (80)  NULL,
    [TimestampIns]     DATETIME      NOT NULL,
    [TimestampVar]     DATETIME      NOT NULL,
    CONSTRAINT [PK_Intervento] PRIMARY KEY CLUSTERED ([CodIntervento] ASC, [DataInizio] ASC)
);

