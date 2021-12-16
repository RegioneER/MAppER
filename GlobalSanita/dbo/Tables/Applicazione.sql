CREATE TABLE [dbo].[Applicazione] (
    [IdApplicazione] INT          NOT NULL,
    [Nome]           VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Applicazione] PRIMARY KEY CLUSTERED ([IdApplicazione] ASC)
);

