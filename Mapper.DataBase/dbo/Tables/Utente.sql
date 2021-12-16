CREATE TABLE [dbo].[Utente] (
    [id]            INT           IDENTITY (1, 1) NOT NULL,
    [username]      NVARCHAR (50) NOT NULL,
    [nome]          NVARCHAR (50) NOT NULL,
    [cognome]       NVARCHAR (50) NOT NULL,
    [email]         NVARCHAR (50) NOT NULL,
    [idRuolo]       INT           NOT NULL,
    [attivato]      BIT           NOT NULL,
    [cancellato]    BIT           NOT NULL,
    [CodiceFiscale] VARCHAR (16)  NULL,
    CONSTRAINT [PK_Utenti] PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_Utenti_Ruoli] FOREIGN KEY ([idRuolo]) REFERENCES [dbo].[Ruoli] ([id])
);

