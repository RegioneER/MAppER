CREATE TABLE [dbo].[UtenteDaCensire] (
    [Id]                      INT          IDENTITY (1, 1) NOT NULL,
    [CodiceFiscale]           VARCHAR (16) NOT NULL,
    [CodRegione]              VARCHAR (3)  NOT NULL,
    [CodAzienda]              VARCHAR (3)  NOT NULL,
    [Cognome]                 VARCHAR (50) NOT NULL,
    [Nome]                    VARCHAR (50) NOT NULL,
    [Email]                   VARCHAR (50) NOT NULL,
    [Pubblico]                BIT          NOT NULL,
    [IdRuolo]                 INT          NOT NULL,
    [IdStato]                 INT          CONSTRAINT [DF_UtenteDaCensire_IdStato] DEFAULT ((1)) NOT NULL,
    [DataCandidatura]         DATETIME     NOT NULL,
    [KeyStruttura]            VARCHAR (61) NOT NULL,
    [IndirizzoIPCandidatura]  VARCHAR (50) NOT NULL,
    [DataInvioServiceDesk]    DATETIME     NULL,
    [DataProfilazione]        DATETIME     NULL,
    [DataNotificaCandidatura] DATETIME     NULL,
    CONSTRAINT [PK_UtenteDaCensire] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [CHK_UtenteDaCensire_AziendaAttivaExist] CHECK ([dbo].[fn_AziendaAttivaExist]([codRegione],[codazienda],(0))=(1)),
    CONSTRAINT [CHK_UtenteDaCensire_RegioneExist] CHECK ([dbo].[fn_RegioneExist]([codregione])=(1)),
    CONSTRAINT [CHK_UtenteDaCensire_StrutturaExists] CHECK ([dbo].[fn_KeyStrutturaAttivaExist]([KeyStruttura],(0))=(1)),
    CONSTRAINT [FK_UtenteDaCensire_Ruoli] FOREIGN KEY ([IdRuolo]) REFERENCES [dbo].[Ruoli] ([id]),
    CONSTRAINT [FK_UtenteDaCensire_StatoCandidatura] FOREIGN KEY ([IdStato]) REFERENCES [dbo].[StatoCandidatura] ([Codice])
);




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Verifica esistenza KeyStruttura', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'UtenteDaCensire', @level2type = N'CONSTRAINT', @level2name = N'CHK_UtenteDaCensire_StrutturaExists';

