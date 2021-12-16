CREATE PROCEDURE [dbo].[CandidatoPubblico_InviaServiceDesk]
	@id int

	AS

BEGIN

SET NOCOUNT ON;


	update UtenteDaCensire 
	set datainvioservicedesk=GETDATE()
	where Id=@id 

END

GO


