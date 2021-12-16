
CREATE PROCEDURE [dbo].[Disciplina_Salva]
(
	@CodDisciplina varchar(3),
	@DataInizio date = NULL,
	@Descrizione varchar(60),
	@DataFine date = NULL,
	@TimeStampIns datetime,
	@TimeStampVar datetime
)
AS
 BEGIN

if (not exists
				(
					select 1 
					from Disciplina d
					where d.CodDisciplina = @CodDisciplina
				)
	)	
	BEGIN
	INSERT INTO Disciplina(
	CodDisciplina,
	DataInizio,
	Descrizione,
	DataFine,
		TimeStampIns,
		TimeStampVar
	)
	VALUES (
		@CodDisciplina,
		@DataInizio,
		@Descrizione,
		@DataFine,
		@TimeStampIns,
		@TimeStampVar
	)
	END
ELSE
	BEGIN
		UPDATE Disciplina 
		SET 
			DataInizio = @DataInizio,
			Descrizione = @Descrizione,
			DataFine = @DataFine,
			TimeStampIns= @TimeStampIns,
			TimeStampVar = @TimeStampVar
			
		WHERE 
			CodDisciplina= @CodDisciplina
	END
END