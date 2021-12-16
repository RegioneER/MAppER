CREATE TABLE [dbo].[Opportunita] (
    [id]             BIGINT     IDENTITY (1, 1) NOT NULL,
    [idAzione]       INT        NOT NULL,
    [idOsservazione] BIGINT     NOT NULL,
    [idIndicazione]  INT        NOT NULL,
    [idBacteria]     NCHAR (20) NULL,
    [data]           DATETIME   NULL,
    [cancellato]     BIT        NOT NULL,
    CONSTRAINT [PK_Opportunità] PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_Opportunità_Azioni] FOREIGN KEY ([idAzione]) REFERENCES [dbo].[Azione] ([id]),
    CONSTRAINT [FK_Opportunita_Bacteria] FOREIGN KEY ([idBacteria]) REFERENCES [dbo].[Bacteria] ([code]),
    CONSTRAINT [FK_Opportunita_Indicazione] FOREIGN KEY ([idIndicazione]) REFERENCES [dbo].[Indicazione] ([id]),
    CONSTRAINT [FK_Opportunità_Osservazioni] FOREIGN KEY ([idOsservazione]) REFERENCES [dbo].[Osservazione] ([id]) ON DELETE CASCADE ON UPDATE CASCADE
);

