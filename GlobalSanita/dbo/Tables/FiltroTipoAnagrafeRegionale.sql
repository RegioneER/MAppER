CREATE TABLE [dbo].[FiltroTipoAnagrafeRegionale] (
    [CodTipoAnagrafeRegionale]        VARCHAR (4) NOT NULL,
    [DataInizioTipoAnagrafeRegionale] DATE        NOT NULL,
    [IdApplicazione]                  INT         NOT NULL,
    CONSTRAINT [PK_FiltroTipoAnagrafeRegionale] PRIMARY KEY CLUSTERED ([CodTipoAnagrafeRegionale] ASC),
    CONSTRAINT [FK_FiltroTipoAnagrafeRegionale_Applicazione] FOREIGN KEY ([IdApplicazione]) REFERENCES [dbo].[Applicazione] ([IdApplicazione]),
    CONSTRAINT [FK_FiltroTipoAnagrafeRegionale_TipoAnagrafeRegionale] FOREIGN KEY ([CodTipoAnagrafeRegionale], [DataInizioTipoAnagrafeRegionale]) REFERENCES [dbo].[TipoAnagrafeRegionale] ([CodTipoAnagrafeRegionale], [DataInizio])
);

