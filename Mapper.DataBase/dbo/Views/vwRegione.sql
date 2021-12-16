



CREATE VIEW [dbo].[vwRegione]
AS
SELECT       CodRegione,
			Denominazione

FROM            [$(GlobalSanita)].[dbo].[Regione]

