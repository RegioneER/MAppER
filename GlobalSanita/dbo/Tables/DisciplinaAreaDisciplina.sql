CREATE TABLE [dbo].[DisciplinaAreaDisciplina] (
    [CodDisciplina]     VARCHAR (3) NOT NULL,
    [CodAreaDisciplina] INT         NOT NULL,
    [IdApplicazione]    INT         CONSTRAINT [DF_DisciplinaAreaDisciplina_IdApplicazione] DEFAULT ((1)) NOT NULL
);

