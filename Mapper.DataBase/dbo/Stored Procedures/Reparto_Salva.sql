CREATE PROCEDURE [dbo].[Reparto_Salva]
(
	@IdReparto int,
	@IdWebServiceReparto int,
	@IdStrutturaErogatrice int,
	@DataInizioStrutturaErogatrice date,
	@DataInizio date,
	@DataFine date = NULL,
	@IdWebServiceStruttura int,
	@CodDisciplina varchar(3) = NULL,
	@ProgressivoDivisione varchar(2) = NULL,
	@Nome varchar(100) = NULL,
	@Descrizione varchar(100) = NULL,
	@CodAreaDisciplina int,
	@Cancellato bit
)
AS
 BEGIN
	if (@idreparto = 0 )
		BEGIN
			INSERT INTO [GlobalSanita].[dbo].[RepartoCustom](
				DataInizio,
				idWebServiceStruttura,
				idStrutturaErogatrice,
				DataInizioStrutturaErogatrice,
				CodDisciplina,
				ProgressivoDivisione,
				Nome,
				Descrizione,
				CodAreaDisciplina,
				Cancellato,
				TimeStampIns,
				TimeStampVar,
				Stato,
				IdApplicazione
			)
			VALUES (
				GETDATE(),
				@IdWebServiceStruttura,
				@IdStrutturaErogatrice,
				@DataInizioStrutturaErogatrice,
				@CodDisciplina,
				@ProgressivoDivisione,
				@Nome,
				@Descrizione,
				@CodAreaDisciplina,
				@Cancellato,
				GETDATE(),
				GETDATE(),
				3,
				1
			)
		END
	ELSE IF (@IdWebServiceReparto = 1)
		BEGIN
			IF not Exists(select 1 from [GlobalSanita].[dbo].[RepartoAlias] where IdReparto=@IdReparto)
				BEGIN
					INSERT INTO [GlobalSanita].[dbo].[RepartoAlias](
						IdReparto,
						Nome,
						Descrizione,
						Cancellato,
						Stato,
						IdApplicazione
					)
					VALUES (
						@IdReparto,
						@Nome,
						@Descrizione,
						@Cancellato,
						2,
						1
					)
				END 
			ELSE
				BEGIN
					UPDATE [GlobalSanita].[dbo].[RepartoAlias]
					SET
						Nome= @Nome,
						Descrizione=@Descrizione,
						Cancellato=@Cancellato
					WHERE 
						IdReparto=@IdReparto
				END 
		END
	ELSE 
		BEGIN
			UPDATE [GlobalSanita].[dbo].[RepartoCustom]
			SET 
				DataFine = @DataFine,
			 	IdStrutturaErogatrice= @IdStrutturaErogatrice,
				DataInizioStrutturaErogatrice = @DataInizioStrutturaErogatrice,
				idWebServiceStruttura= @IdWebServiceStruttura,
				CodDisciplina= @CodDisciplina,
				ProgressivoDivisione= @ProgressivoDivisione,
				Nome= @Nome,
				Descrizione= @Descrizione,
				CodAreaDisciplina= @CodAreaDisciplina,
				TimeStampVar = GETDATE(),
				Cancellato= @Cancellato,
				Stato = 3,
				IdApplicazione=1
			WHERE 
				IdReparto= @IdReparto
		END
END


GO


