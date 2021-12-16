CREATE TABLE [dbo].[UltimaPosizione] (
    [id]          INT            IDENTITY (1, 1) NOT NULL,
    [username]    NVARCHAR (50)  NOT NULL,
    [url]         NVARCHAR (255) NOT NULL,
    [datiRicerca] NVARCHAR (MAX) NULL,
    [data]        DATETIME       NOT NULL,
    CONSTRAINT [PK_UltimaPosizione] PRIMARY KEY CLUSTERED ([id] ASC)
);

