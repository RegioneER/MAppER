
CREATE PROCEDURE [dbo].[Azienda_Salva]
(
	@CodRegione varchar(3),
	@CodAzienda varchar(3),
	@DataInizio date,
	@DataFine date = NULL,	
	@CodTipoAzienda varchar(5),
	@Denominazione varchar(1000),
	@DescrizioneBreve varchar(60),
	@SiglaAzienda varchar(30) = NULL,
	@Provincia varchar(4) = NULL,
	@CodAziendaUsl varchar(6) = NULL,
	@PartitaIva varchar(11) = NULL,
	@TimeStampIns datetime,
	@TimeStampVar datetime
)
AS
 BEGIN

 if (not exists
				(
					select 1 
					from Azienda a 
					where a.CodRegione = @CodRegione
						and a .CodAzienda = @CodAzienda
						and a.DataInizio = @DataInizio
				)
	)	
	BEGIN
	INSERT INTO Azienda(
		CodRegione,
		CodAzienda,
		DataInizio,
		DataFine,
		CodTipoAzienda,
		Denominazione,
		DescrizioneBreve,
		SiglaAzienda,
		Provincia,
		CodAziendaUsl,
		PartitaIva,
		TimeStampIns,
		TimeStampVar
	)
	VALUES (
		@CodRegione,
		@CodAzienda,
		@DataInizio,
		@DataFine,
		@CodTipoAzienda,
		@Denominazione,
		@DescrizioneBreve,
		@SiglaAzienda,
		@Provincia,
		@CodAziendaUsl,
		@PartitaIva,
		@TimeStampIns,
		@TimeStampVar
	)
	END
ELSE
	BEGIN
		UPDATE Azienda 
		SET 
			DataFine = @DataFine,
			CodTipoAzienda = @CodTipoAzienda,
			Denominazione = @Denominazione,
			DescrizioneBreve= @DescrizioneBreve,
			SiglaAzienda = @SiglaAzienda,
			Provincia = @Provincia,
			CodAziendaUsl = @CodAziendaUsl,
			PartitaIva = @PartitaIva,
			TimeStampIns= @TimeStampIns,
			TimeStampVar = @TimeStampVar
			
		WHERE 
			CodAzienda= @CodAzienda
			AND CodRegione = @CodRegione
			AND DataInizio = @DataInizio
	END
END