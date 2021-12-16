
CREATE PROCEDURE [dbo].[Struttura_Salva]
(
	@IdStrutturaErogatrice int,
	@DataInizio date,
	@DataFine date = NULL,
	@DataApertura date,
	@DataChiusura date = NULL,
	@Denominazione varchar(150),
	@CodRegione varchar(3),
	@CodAzienda varchar(3),
	@DataInizioAzienda date,
	@CodMin varchar(6),
	@SubCodMin varchar(2),
	@IdCodStruttura int, 
	@CodTipoAnagrafeRegionale varchar(4),
	@DataInizioTipoAnagrafeRegionale date,
	@PubblicoPrivato varchar(1) =NULL,
	@IndirizzoAggiuntivo varchar(100) = NULL,
	@CapAggiuntivo varchar(5) = NULL,
	@IdStrutturaFisica int,
	@DataInizioStrutturaFisica date,
	@TimeStampIns datetime,
	@TimeStampVar datetime
)
AS
 BEGIN

 if (not exists
				(
					select 1 
					from Struttura s
					where s.IdStrutturaErogatrice = @IdStrutturaErogatrice
						and s.DataInizio = @DataInizio
				)
	)	
	BEGIN
	INSERT INTO Struttura(
		IdStrutturaErogatrice,
		DataInizio,
		DataFine,
		DataApertura,
		DataChiusura,
		Denominazione,
		CodRegione,
		CodAzienda,
		DataInizioAzienda,
		CodMin,
		SubCodMin,
		IdCodStruttura,
		CodTipoAnagrafeRegionale,
		DataInizioTipoAnagrafeRegionale,
		PubblicoPrivato,
		IndirizzoAggiuntivo,
		CapAggiuntivo,
		IdStrutturaFisica,
		DataInizioStrutturaFisica,
		TimeStampIns,
		TimeStampVar
	)
	VALUES (
		@IdStrutturaErogatrice,
		@DataInizio,
		@DataFine,
		@DataApertura,
		@DataChiusura,
		@Denominazione,
		@CodRegione,
		@CodAzienda,
		@DataInizioAzienda,
		@CodMin,
		@SubCodMin,
		@IdCodStruttura,
		@CodTipoAnagrafeRegionale,
		@DataInizioTipoAnagrafeRegionale,
		@PubblicoPrivato,
		@IndirizzoAggiuntivo,
		@CapAggiuntivo,
		@IdStrutturaFisica,
		@DataInizioStrutturaFisica,
		@TimeStampIns,
		@TimeStampVar
		)
	END
ELSE
	BEGIN
		UPDATE Struttura 
		SET 
			DataFine = @DataFine,
			DataApertura= @DataApertura,
			DataChiusura = @DataChiusura,
			Denominazione = @Denominazione,
			CodRegione = @CodRegione,
			CodAzienda = @CodAzienda,
			DataInizioAzienda = @DataInizioAzienda,
			CodMin = @CodMin,
			SubCodMin = @SubCodMin,
			IdCodStruttura = @IdCodStruttura,
			CodTipoAnagrafeRegionale = @CodTipoAnagrafeRegionale,
			DataInizioTipoAnagrafeRegionale = @DataInizioTipoAnagrafeRegionale,
			PubblicoPrivato =@PubblicoPrivato,
			IndirizzoAggiuntivo = @IndirizzoAggiuntivo,
			CapAggiuntivo = @CapAggiuntivo,
			IdStrutturaFisica = @IdStrutturaFisica,
			DataInizioStrutturaFisica = @DataInizioStrutturaFisica,
			TimeStampIns = @TimeStampIns,
			TimeStampVar= @TimeStampVar
			
		WHERE 
			IdStrutturaErogatrice= @IdStrutturaErogatrice
			AND  DataInizio = @DataInizio
	END
END