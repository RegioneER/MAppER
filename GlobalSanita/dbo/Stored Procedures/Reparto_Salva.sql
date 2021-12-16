
CREATE PROCEDURE [dbo].[Reparto_Salva]
(
	@IdStrutturaErogatrice int,
	@DataInizioStrutturaErogatrice date,
	@DataInizio date,
	@DataFine date=NULL,
	@CodDisciplina varchar(3),
	@ProgressivoDivisione varchar(2),
	@TipoDivisione varchar(3) = NULL,
	@PostiLettoDayHospital int,
	@PostiLettoDegenzeOrdinarie int,
	@PostiLettoDegenzeOrdinariePagamento int,
	@PostiLettoDaySurgery int,
	@AssistenzaFamiliare varchar(1) = NULL,
	@Modello varchar(50) = NULL,
	@TimeStampIns datetime,
	@TimeStampVar datetime
)
AS
 BEGIN

 if (not exists
				(
					select 1 
					from Reparto r
					where r.IdStrutturaErogatrice = @IdStrutturaErogatrice
						AND r.DataInizioStrutturaErogatrice = @DataInizioStrutturaErogatrice
						AND r.CodDisciplina = @CodDisciplina
						AND r.ProgressivoDivisione = @ProgressivoDivisione
				)
	)	
	BEGIN
	INSERT INTO Reparto(
		IdStrutturaErogatrice,
		DataInizioStrutturaErogatrice,
		DataInizio,
		DataFine,
		CodDisciplina,
		ProgressivoDivisione,
		TipoDivisione,
		PostiLettoDayHospital,
		PostiLettoDegenzeOrdinarie,
		PostiLettoDegenzeOrdinariePagamento,
		PostiLettoDaySurgery,
		AssistenzaFamiliare,
		Modello,
		TimeStampIns,
		TimeStampVar
	)
	VALUES (
		@IdStrutturaErogatrice,
		@DataInizioStrutturaErogatrice,
		@DataInizio,
		@DataFine,
		@CodDisciplina,
		@ProgressivoDivisione, 
		@TipoDivisione,
		@PostiLettoDayHospital, 
		@PostiLettoDegenzeOrdinarie, 
		@PostiLettoDegenzeOrdinariePagamento,
		@PostiLettoDaySurgery,
		@AssistenzaFamiliare,
		@Modello,
		@TimeStampIns,
		@TimeStampVar
	)
	END
ELSE
	BEGIN
		UPDATE Reparto 
		SET 
			DataInizio = @DataInizio,
			DataFine = @DataFine,
			TipoDivisione=@TipoDivisione,
			PostiLettoDayHospital = @PostiLettoDayHospital,
			PostiLettoDegenzeOrdinarie = @PostiLettoDegenzeOrdinarie,
			PostiLettoDegenzeOrdinariePagamento = @PostiLettoDegenzeOrdinariePagamento,
			PostiLettoDaySurgery = @PostiLettoDaySurgery,
			AssistenzaFamiliare = @AssistenzaFamiliare,
			Modello = @Modello,
			TimeStampIns= @TimeStampIns,
			TimeStampVar = @TimeStampVar
			
		WHERE 
			IdStrutturaErogatrice = @IdStrutturaErogatrice
			AND DataInizioStrutturaErogatrice = @DataInizioStrutturaErogatrice
			AND CodDisciplina = @CodDisciplina
			AND ProgressivoDivisione = @ProgressivoDivisione
	END
END