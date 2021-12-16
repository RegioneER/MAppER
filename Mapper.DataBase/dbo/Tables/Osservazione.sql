CREATE TABLE [dbo].[Osservazione] (
    [id]               BIGINT   IDENTITY (1, 1) NOT NULL,
    [idScheda]         INT      NOT NULL,
    [numOperatori]     INT      NOT NULL,
    [idOperatore]      INT      NOT NULL,
    [operatoreEsterno] BIT      NULL,
    [data]             DATETIME NULL,
    CONSTRAINT [PK_Osservazioni] PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_Osservazione_Operatore] FOREIGN KEY ([idOperatore]) REFERENCES [dbo].[Operatore] ([id]),
    CONSTRAINT [FK_Osservazioni_Schede] FOREIGN KEY ([idScheda]) REFERENCES [dbo].[Scheda] ([id]) ON DELETE CASCADE
);

