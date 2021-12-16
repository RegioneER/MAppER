CREATE TABLE [dbo].[Disciplina] (
    [CodDisciplina] VARCHAR (3)  NOT NULL,
    [DataInizio]    DATE         NULL,
    [Descrizione]   VARCHAR (60) NOT NULL,
    [DataFine]      DATE         NULL,
    [TimeStampIns]  DATETIME     CONSTRAINT [DF_Disciplina_TimeStampIns] DEFAULT (getdate()) NOT NULL,
    [TimeStampVar]  DATETIME     NOT NULL,
    CONSTRAINT [PK_Disciplina] PRIMARY KEY CLUSTERED ([CodDisciplina] ASC)
);



