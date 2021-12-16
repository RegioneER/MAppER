
CREATE PROCEDURE [dbo].[DataUltimoAggiornamento_Elenca]
(
	@nomeTabella varchar(50)
)
AS
 BEGIN

DECLARE @SQLString NVARCHAR(500);  
DECLARE @ParmDefinition NVARCHAR(500);  
DECLARE @max_date DATE;  

SET @SQLString = N'SELECT @max_dateOUT = max(TimeStampVar)   
   FROM ' + @nomeTabella;  
SET @ParmDefinition = N'@max_dateOUT DATE OUTPUT';  

EXEC sp_executesql @SQLString, @ParmDefinition, @max_dateOUT = @max_date OUTPUT;  

select @max_date DataUltimoAggiornamento



END