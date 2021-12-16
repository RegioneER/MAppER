CREATE FUNCTION [dbo].[fn_AziendaAttivaExist]
(@codRegione varchar(3), @codAzienda varchar(3), @valoreNullValido BIT) 
RETURNS BIT
AS
BEGIN
    DECLARE @exists bit = 0
    IF (@valoreNullValido=1 and @codAzienda is null)
	OR EXISTS (
      SELECT TOP 1 1 FROM vwAzienda
      WHERE CodRegione=@codRegione and CodAzienda=@codAzienda and DataFine is null
    ) BEGIN 
         SET @exists = 1 
      END;
      RETURN @exists
END
