CREATE TABLE [dbo].[StatoSessione] (
    [id]                  INT        IDENTITY (1, 1) NOT NULL,
    [nome]                NCHAR (10) NOT NULL,
    [DescrizionePubblica] NCHAR (20) NOT NULL,
    CONSTRAINT [PK_StatoSessione] PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 90)
);

