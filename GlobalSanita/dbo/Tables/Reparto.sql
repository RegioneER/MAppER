CREATE TABLE [dbo].[Reparto] (
    [IdReparto]                           INT          IDENTITY (1, 1) NOT NULL,
    [IdStrutturaErogatrice]               INT          NOT NULL,
    [DataInizioStrutturaErogatrice]       DATE         NOT NULL,
    [DataInizio]                          DATE         NOT NULL,
    [DataFine]                            DATE         NULL,
    [CodDisciplina]                       VARCHAR (3)  NOT NULL,
    [ProgressivoDivisione]                VARCHAR (2)  NOT NULL,
    [TipoDivisione]                       VARCHAR (3)  NULL,
    [PostiLettoDayHospital]               INT          NOT NULL,
    [PostiLettoDegenzeOrdinarie]          INT          NOT NULL,
    [PostiLettoDegenzeOrdinariePagamento] INT          NOT NULL,
    [PostiLettoDaySurgery]                INT          NOT NULL,
    [AssistenzaFamiliare]                 VARCHAR (1)  NULL,
    [Modello]                             VARCHAR (50) NOT NULL,
    [TimeStampIns]                        DATETIME     NOT NULL,
    [TimeStampVar]                        DATETIME     NOT NULL,
    PRIMARY KEY CLUSTERED ([IdReparto] ASC),
    FOREIGN KEY ([IdStrutturaErogatrice], [DataInizioStrutturaErogatrice]) REFERENCES [dbo].[Struttura] ([IdStrutturaErogatrice], [DataInizio]),
    CONSTRAINT [FK__Reparto__CodDisc__5C1A1321] FOREIGN KEY ([CodDisciplina]) REFERENCES [dbo].[Disciplina] ([CodDisciplina])
);

