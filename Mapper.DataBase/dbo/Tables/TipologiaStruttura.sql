CREATE TABLE [dbo].[TipologiaStruttura] (
    [CodTipologia]      VARCHAR (50)  NOT NULL,
    [Descrizione]       VARCHAR (100) NOT NULL,
    [TipoEnte]          VARCHAR (10)  NULL,
    [IsAttivo]          BIT           NOT NULL,
    [Ordinale]          INT           NOT NULL,
    [CodAreaDisciplina] INT           NULL,
    CONSTRAINT [PK_TipologiaStruttura] PRIMARY KEY CLUSTERED ([CodTipologia] ASC),
    CONSTRAINT [CK_TipologiaStruttura] CHECK ([dbo].[fn_AreaDisciplinaExist]([CodAreaDisciplina])=(1))
);



