CREATE TABLE [dbo].[Ruoli] (
    [id]       INT           IDENTITY (1, 1) NOT NULL,
    [nome]     NVARCHAR (50) NOT NULL,
    [ordinale] INT           NULL,
    CONSTRAINT [PK_Ruoli] PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 90)
);

