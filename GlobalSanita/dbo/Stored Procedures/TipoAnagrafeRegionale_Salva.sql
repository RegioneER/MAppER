
CREATE PROCEDURE [dbo].[TipoAnagrafeRegionale_Salva]
(
	@CodTipoAnagrafeRegionale varchar(4),
	@DataInizio date,
	@DataFine date = NULL,
	@Descrizione varchar(100),
	@CodMacroArea int,
	@CodTarget int = NULL,
	@CodTipoStrutturaMinisteriale varchar(2) = NULL,
	@CodTipoStrutturaMinisterialeInterno varchar(3),
	@TimeStampIns datetime,
	@TimeStampVar datetime
)
AS
 BEGIN

 if (not exists
				(
					select 1 
					from TipoAnagrafeRegionale tar
					where tar.CodTipoAnagrafeRegionale = @CodTipoAnagrafeRegionale
						and tar.DataInizio = @DataInizio 
				)
	)	
	BEGIN
	INSERT INTO TipoAnagrafeRegionale(
		CodTipoAnagrafeRegionale,
		DataInizio,
		DataFine,
		Descrizione,
		CodMacroArea,
		CodTarget,
		CodTipoStrutturaMinisteriale,
		CodTipoStrutturaMinistarialeInterna,
		TimeStampIns,
		TimeStampVar
	)
	VALUES (
		@CodTipoAnagrafeRegionale,
		@DataInizio,
		@DataFine,
		@Descrizione,
		@CodMacroArea,
		@CodTarget,
		@CodTipoStrutturaMinisteriale,
		@CodTipoStrutturaMinisterialeInterno,
		@TimeStampIns,
		@TimeStampVar
	)
	END
ELSE
	BEGIN
		UPDATE TipoAnagrafeRegionale 
		SET 
			DataFine = @DataFine,
			Descrizione = @Descrizione,
			CodMacroArea = @CodMacroArea,
			CodTarget = @CodTarget,
			CodTipoStrutturaMinisteriale = @CodTipoStrutturaMinisteriale,
			CodTipoStrutturaMinistarialeInterna = @CodTipoStrutturaMinisterialeInterno,
			TimeStampIns= @TimeStampIns,
			TimeStampVar = @TimeStampVar
			
		WHERE 
			CodTipoAnagrafeRegionale= @CodTipoAnagrafeRegionale
			AND  DataInizio = @DataInizio
	END
END