CREATE TABLE [dbo].[Operatore] (
    [id]            INT           IDENTITY (1, 1) NOT NULL,
    [codCategoria]  VARCHAR (50)  NOT NULL,
    [nomeCategoria] VARCHAR (MAX) NOT NULL,
    [classe]        VARCHAR (50)  NOT NULL,
    [ClasseColore]  VARCHAR (20)  NULL,
    CONSTRAINT [PK_Operatore] PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 90)
);

