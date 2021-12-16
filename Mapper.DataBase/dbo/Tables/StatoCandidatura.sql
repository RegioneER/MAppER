CREATE TABLE [dbo].[StatoCandidatura] (
    [Codice]      INT           NOT NULL,
    [Descrizione] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_StatoCandidatura] PRIMARY KEY CLUSTERED ([Codice] ASC)
);

