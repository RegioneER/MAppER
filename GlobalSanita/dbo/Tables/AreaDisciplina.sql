CREATE TABLE [dbo].[AreaDisciplina] (
    [CodAreaDisciplina] INT            NOT NULL,
    [Nome]              VARCHAR (50)   NOT NULL,
    [Descrizione]       VARCHAR (1000) NULL,
    [Ordinale]          INT            NOT NULL,
    [IdApplicazione]    INT            NOT NULL,
    CONSTRAINT [PK_AreaDisciplina] PRIMARY KEY CLUSTERED ([CodAreaDisciplina] ASC),
    CONSTRAINT [FK_AreaDisciplina_Applicazione] FOREIGN KEY ([IdApplicazione]) REFERENCES [dbo].[Applicazione] ([IdApplicazione])
);

