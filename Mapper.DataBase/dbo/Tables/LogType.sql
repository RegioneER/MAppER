CREATE TABLE [dbo].[LogType] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [descrizione] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_LogType] PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 90)
);

