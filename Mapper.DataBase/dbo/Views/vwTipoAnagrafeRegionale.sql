CREATE VIEW [dbo].[vwTipoAnagrafeRegionale]
AS
SELECT        tar.CodTipoAnagrafeRegionale,
				DataInizio,
				DataFine,
				Descrizione,
				CodMacroArea,
				CodTarget,
				CodTipoStrutturaMinisteriale,
				CodTipoStrutturaMinistarialeInterna
FROM            [$(GlobalSanita)].dbo.TipoAnagrafeRegionale tar
inner join [$(GlobalSanita)].dbo.FiltroTipoAnagrafeRegionale ftar on tar.CodTipoAnagrafeRegionale= ftar.CodTipoAnagrafeRegionale

where ftar.idapplicazione = 1

