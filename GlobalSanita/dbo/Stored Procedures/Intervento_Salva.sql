
CREATE PROCEDURE [dbo].[Intervento_Salva]
(
	@CodIntervento varchar(4),
	@DataInizio date = NULL,
	@DataFine date = NULL,
	@Descrizione varchar(120),
	@DescrizioneBreve varchar(80),
	@TimeStampIns datetime,
	@TimeStampVar datetime
)
AS
 BEGIN

 if (not exists
				(
					select 1 
					from Intervento i
					where i.CodIntervento = @CodIntervento
				)
	)	
	BEGIN
	INSERT INTO Intervento(
CodIntervento,
DataInizio,
DataFine,
Descrizione,
DescrizioneBreve,
		TimestampIns,
		TimestampVar
	)
	VALUES (
		@CodIntervento,
		@DataInizio,		
		@DataFine,
		@Descrizione,
		@DescrizioneBreve,
		@TimeStampIns,
		@TimeStampVar
	)
	END
ELSE
	BEGIN
		UPDATE Intervento 
		SET 
			DataInizio = @DataInizio,
			DataFine = @DataFine,
			Descrizione = @Descrizione,
			DescrizioneBreve = @DescrizioneBreve,
			TimestampIns = @TimeStampIns,
			TimestampVar = @TimeStampVar
			
		WHERE 
			CodIntervento= @CodIntervento
	END
END