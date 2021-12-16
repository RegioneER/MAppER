CREATE TABLE [dbo].[TipoAnagrafeRegionale] (
    [CodTipoAnagrafeRegionale]            VARCHAR (4)   NOT NULL,
    [DataInizio]                          DATE          NOT NULL,
    [DataFine]                            DATE          NULL,
    [Descrizione]                         VARCHAR (100) NOT NULL,
    [CodMacroArea]                        INT           NOT NULL,
    [CodTarget]                           INT           NULL,
    [CodTipoStrutturaMinisteriale]        VARCHAR (2)   NULL,
    [CodTipoStrutturaMinistarialeInterna] VARCHAR (3)   NOT NULL,
    [TimeStampIns]                        DATETIME      NOT NULL,
    [TimeStampVar]                        DATETIME      NOT NULL,
    PRIMARY KEY CLUSTERED ([CodTipoAnagrafeRegionale] ASC, [DataInizio] ASC)
);

