
CREATE FUNCTION [dbo].[fn_StrutturaAttivaExist]
(@idStruttura INT, @idWebService INT, @valoreNullValido BIT) 
RETURNS BIT
AS
BEGIN
    DECLARE @exists bit = 0
    IF (@valoreNullValido=1 and @idStruttura is null and @idWebService is null)
	OR EXISTS (
      SELECT TOP 1 1 FROM vwStruttura
      WHERE (idstrutturaerogatrice = @idStruttura and IdWebServiceStruttura =@idWebService and DataFine is null and DataChiusura is null)
    ) BEGIN 
         SET @exists = 1 
      END;
      RETURN @exists
END
