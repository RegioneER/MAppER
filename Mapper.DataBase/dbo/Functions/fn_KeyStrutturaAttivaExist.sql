
CREATE FUNCTION [dbo].[fn_KeyStrutturaAttivaExist]
(@keyStruttura varchar(61), @valoreNullValido BIT) 
RETURNS BIT
AS
BEGIN
    DECLARE @exists bit = 0
    IF (@valoreNullValido=1 and @keyStruttura is null)
	OR EXISTS (
      SELECT TOP 1 1 FROM vwStruttura
      WHERE (KeyStruttura = @keyStruttura and DataFine is null and DataChiusura is null)
    ) BEGIN 
         SET @exists = 1 
      END;
      RETURN @exists
END
