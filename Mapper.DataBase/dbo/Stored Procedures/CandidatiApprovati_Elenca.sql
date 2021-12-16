CREATE PROCEDURE [dbo].[CandidatiApprovati_Elenca]
	
	AS

BEGIN

SET NOCOUNT ON;

	select Id,
		CodiceFiscale,
		CodRegione,
		CodAzienda,
		Cognome,
		Nome,
		Email,
		Pubblico,
		IdRuolo,
		KeyStruttura,
		dataInvioservicedesk,
		dataprofilazione
	from UtenteDaCensire
	where IdStato=1 
	and dataprofilazione is null
END


GO


