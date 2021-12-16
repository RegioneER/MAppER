CREATE TABLE [dbo].[Regione] (
    [CodRegione]    VARCHAR (3)  NOT NULL,
    [Denominazione] VARCHAR (50) NOT NULL,
    [TimeStampIns]  DATE         NOT NULL,
    [TimeStampVar]  DATE         NOT NULL,
    CONSTRAINT [PK_Regione] PRIMARY KEY CLUSTERED ([CodRegione] ASC)
);

