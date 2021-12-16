CREATE TABLE [dbo].[ConvenzionePrivacy] (
    [KeyStruttura]   VARCHAR (61) NOT NULL,
    [InConvenzione]  BIT          NOT NULL,
    [Dal]            DATE         NOT NULL,
    [Al]             DATE         NULL,
    [IdApplicazione] INT          NOT NULL,
    CONSTRAINT [PK_ConvenzionePrivacy] PRIMARY KEY CLUSTERED ([KeyStruttura] ASC, [Dal] ASC),
    CONSTRAINT [FK_ConvenzionePrivacy_Applicazione] FOREIGN KEY ([IdApplicazione]) REFERENCES [dbo].[Applicazione] ([IdApplicazione])
);

