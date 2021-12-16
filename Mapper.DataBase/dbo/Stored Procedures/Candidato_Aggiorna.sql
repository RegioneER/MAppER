CREATE PROCEDURE [dbo].[Candidato_Aggiorna]
	@id int,
	@pubblico bit,
	@nome varchar(50),
	@cognome varchar(50),
	@email varchar(50),
	@idruolo int,
	@cf varchar(16),
	@codregione varchar(3),
	@codazienda varchar(3),
	@idstruttura int,
	@idwebservice int,
	@idUtente int OUTPUT,
	@esito BIT OUTPUT
AS

BEGIN

SET NOCOUNT ON;
SET @esito = CONVERT(bit, 0)
	if (@pubblico=0)
	/*il candidato privato viene inserito da qui sulla tabella utenti*/
		BEGIN
			set @idUtente= (select id from Utente where username=@cf)

			if (@idUtente is null)
				BEGIN
					INSERT INTO Utente(
						username,
						nome,
						cognome,
						email,
						idRuolo,
						attivato,
						cancellato,
						CodiceFiscale)
					VALUES (
						@cf,
						@nome,
						@cognome,
						@email,
						@idruolo,
						1,
						0,
						NULL
					)
					set @idUtente = (SELECT SCOPE_IDENTITY())
				END
		END 
	ELSE
	/*il candidato pubblico è stato inserito dal servicedesk. Se è già stato inserito, aggiorno il ruolo e il codicefiscale*/
		BEGIN
			set @idUtente= (select id from Utente where idruolo=5 and (email=@email OR CodiceFiscale=@cf))
			if (@idUtente is not null)
				BEGIN
					UPDATE Utente
					SET idRuolo = @idruolo, CodiceFiscale=@cf
					WHERE id=@idUtente
				END 
		END

		/*inserisco i dati per l'utentestruttura e aggiornare utentedacensire*/
	if (@idutente is not null and not exists(select top 1 u.ID from Utente u inner join UtenteStruttura us on u.id=us.idUtente where idUtente=@idUtente and idRuolo=@idruolo and codRegione=@codregione and codAzienda=@codazienda and idStrutturaErogatrice=@idstruttura and idWebServiceStruttura=@idwebservice))
		BEGIN
			INSERT INTO UtenteStruttura (
				idUtente,
				codRegione,
				codAzienda,
				idStrutturaErogatrice,
				idWebServiceStruttura,
				dataDal	
				)
			VALUES (
				@idUtente,
				@codregione,
				@codazienda,
				@idstruttura,
				@idwebservice,
				GETDATE()
				)

			update UtenteDaCensire
			set IdStato=3,
			dataprofilazione= GETDATE()
			where Id=@id

			SET @esito = CONVERT(bit, 1)
		END
	
END

GO


