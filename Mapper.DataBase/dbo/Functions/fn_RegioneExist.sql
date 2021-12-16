CREATE FUNCTION [dbo].[fn_RegioneExist]
(@codRegione varchar(3)) 
RETURNS BIT
AS
BEGIN
    DECLARE @exists bit = 0
    IF EXISTS (
      SELECT TOP 1 1 FROM vwRegione 
      WHERE CodRegione=@codRegione
    ) BEGIN 
         SET @exists = 1 
      END;
      RETURN @exists
END
