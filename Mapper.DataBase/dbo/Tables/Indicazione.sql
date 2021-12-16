CREATE TABLE [dbo].[Indicazione] (
    [id]        INT           IDENTITY (1, 1) NOT NULL,
    [tipologia] NVARCHAR (50) NOT NULL,
    [ordinale]  INT           NULL,
    CONSTRAINT [PK_Indicazione] PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 90)
);

