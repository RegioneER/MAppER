CREATE TABLE [dbo].[Azione] (
    [id]        INT           IDENTITY (1, 1) NOT NULL,
    [tipologia] NVARCHAR (50) NOT NULL,
    [ordinale]  INT           NULL,
    [adesione]  BIT           NOT NULL,
    CONSTRAINT [PK_Azioni] PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 90)
);

