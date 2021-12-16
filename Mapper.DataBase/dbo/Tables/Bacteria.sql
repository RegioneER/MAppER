CREATE TABLE [dbo].[Bacteria] (
    [code]           NCHAR (20)     NOT NULL,
    [description_EN] NVARCHAR (MAX) NULL,
    [description_IT] NVARCHAR (MAX) NULL,
    [ordinale]       INT            NULL,
    CONSTRAINT [PK_Bacteria] PRIMARY KEY CLUSTERED ([code] ASC) WITH (FILLFACTOR = 90)
);

