CREATE TABLE [dbo].[UtenteStruttura] (
    [ID]                    INT         IDENTITY (1, 1) NOT NULL,
    [idUtente]              INT         NOT NULL,
    [codRegione]            VARCHAR (3) NULL,
    [codAzienda]            VARCHAR (3) NULL,
    [idStrutturaErogatrice] INT         NULL,
    [idWebServiceStruttura] INT         NULL,
    [idReparto]             INT         NULL,
    [idWebServiceReparto]   INT         NULL,
    [dataDal]               DATETIME    NOT NULL,
    [dataAl]                DATETIME    NULL,
    CONSTRAINT [PK_UtenteStruttura] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [CHK_UtenteStruttura_AziendaAttivaExist] CHECK ([dbo].[fn_AziendaAttivaExist]([codRegione],[codazienda],(1))=(1)),
    CONSTRAINT [CHK_UtenteStruttura_RegioneExist] CHECK ([dbo].[fn_RegioneExist]([codregione])=(1)),
    CONSTRAINT [CHK_UtenteStruttura_RepartoExist] CHECK ([dbo].[fn_RepartoExist]([idReparto],[idWebServiceReparto],(1))=(1)),
    CONSTRAINT [CHK_UtenteStruttura_StrutturaExist] CHECK ([dbo].[fn_StrutturaAttivaExist]([idStrutturaErogatrice],[idWebServiceStruttura],(1))=(1)),
    CONSTRAINT [FK_UtenteStruttura_Utente] FOREIGN KEY ([idUtente]) REFERENCES [dbo].[Utente] ([id])
);



