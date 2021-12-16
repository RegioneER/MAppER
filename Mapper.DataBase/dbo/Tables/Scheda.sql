CREATE TABLE [dbo].[Scheda] (
    [id]                      INT           IDENTITY (1, 1) NOT NULL,
    [durataSessione]          INT           NOT NULL,
    [idReparto]               INT           NOT NULL,
    [idWebServiceReparto]     INT           NOT NULL,
    [idUtente]                INT           NOT NULL,
    [note]                    NVARCHAR (50) NULL,
    [idStatoSessione]         INT           NOT NULL,
    [data]                    DATETIME      NOT NULL,
    [dataInserimento]         DATETIME      NOT NULL,
    [dataUltimaModificaStato] DATETIME      NOT NULL,
    CONSTRAINT [PK_Schede] PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [CK_Scheda_Reparto] CHECK ([dbo].[fn_RepartoExist]([idReparto],[idWebServiceReparto],(0))=(1)),
    CONSTRAINT [FK_Scheda_StatoSessione] FOREIGN KEY ([idStatoSessione]) REFERENCES [dbo].[StatoSessione] ([id]),
    CONSTRAINT [FK_Scheda_Utente] FOREIGN KEY ([idUtente]) REFERENCES [dbo].[Utente] ([id])
);



