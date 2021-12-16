CREATE TABLE [dbo].[RepartoAlias] (
    [IdReparto]      INT            NOT NULL,
    [Nome]           VARCHAR (1000) NOT NULL,
    [Descrizione]    VARCHAR (1000) NULL,
    [Cancellato]     BIT            CONSTRAINT [DF_RepartoAlias_Cancellato] DEFAULT ((0)) NOT NULL,
    [Stato]          INT            CONSTRAINT [DF_RepartoAlias_Stato] DEFAULT ((2)) NOT NULL,
    [IdApplicazione] INT            NOT NULL,
    CONSTRAINT [PK_RepartoAlias] PRIMARY KEY CLUSTERED ([IdReparto] ASC),
    CONSTRAINT [FK_RepartoAlias_Applicazione] FOREIGN KEY ([IdApplicazione]) REFERENCES [dbo].[Applicazione] ([IdApplicazione])
);

