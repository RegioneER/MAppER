CREATE FUNCTION [dbo].[fn_RepartoExist]
(@idReparto INT, @idWebServiceReparto INT, @valoreNullValido BIT) 
RETURNS BIT
AS
BEGIN
    DECLARE @exists bit = 0

    IF (@valoreNullValido=1 and @idReparto is null and @idWebServiceReparto is null)
	OR EXISTS (
      SELECT TOP 1 1 FROM vwReparto 
      WHERE (IdReparto = @idReparto 
				and IdWebServiceReparto =@idWebServiceReparto)
    ) BEGIN 
         SET @exists = 1 
      END;
      RETURN @exists
END
