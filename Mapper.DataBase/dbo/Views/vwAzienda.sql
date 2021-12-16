CREATE VIEW [dbo].[vwAzienda]
AS
SELECT        CodRegione, 
				CodAzienda, 
				IIF(Datafine is null, Denominazione, Denominazione + ' (' + CONVERT(VARCHAR(10),DataInizio,101) + ' , ' + CONVERT(VARCHAR(10),DataFine,101) + ')') AS Denominazione,
				DataInizio,
				DataFine,
				CodAzienda + '.' + CONVERT(varchar, DataInizio) KeyAzienda

FROM            [$(GlobalSanita)].[dbo].[Azienda]

WHERE DataFine is null OR DataFine >= '01/01/2015'


