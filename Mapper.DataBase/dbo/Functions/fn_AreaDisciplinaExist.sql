
CREATE FUNCTION [dbo].[fn_AreaDisciplinaExist]
(@CodAreaDisciplina int=NULL) 
RETURNS BIT
AS
BEGIN
    DECLARE @exists bit = 0

    IF (@CodAreaDisciplina is null)
	OR EXISTS (
      SELECT TOP 1 1 FROM vwAreaDisciplina
      WHERE (CodAreaDisciplina = @CodAreaDisciplina
				)
    ) BEGIN 
         SET @exists = 1 
      END;
      RETURN @exists
END
