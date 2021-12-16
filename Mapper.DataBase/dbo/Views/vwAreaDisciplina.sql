CREATE VIEW [dbo].[vwAreaDisciplina]
AS
SELECT  CodAreaDisciplina,
		Nome,
		Descrizione,
		Ordinale

FROM            [$(GlobalSanita)].[dbo].[AreaDisciplina]
WHERE IdApplicazione = 1

