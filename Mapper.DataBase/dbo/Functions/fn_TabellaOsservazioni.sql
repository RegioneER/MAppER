CREATE FUNCTION [dbo].[fn_TabellaOsservazioni]
(
	@idScheda int = NULL,
	@idOsservazione int = NULL
)
RETURNS @temp TABLE 
(
	Azione varchar(20) PRIMARY KEY,
	[Prima contatto paziente] int,
		[Prima di manovra di asepsi] int,
		[Dopo contatto fluido] int,
		[Dopo contatto paziente] int,
		[Dopo contatto ambiente ] int
)
AS
BEGIN

DECLARE @appoggio TABLE 
(
	Azione varchar(20),
	[Prima contatto paziente] int,
		[Prima di manovra di asepsi] int,
		[Dopo contatto fluido] int,
		[Dopo contatto paziente] int,
		[Dopo contatto ambiente ] int
)

insert @appoggio  
	select * from (
		
		select a.tipologia Azione,
				i.tipologia Indicazione, 
				op.idIndicazione

		from  Osservazione os inner join 
				(
				Opportunita op 
					inner join Indicazione i on op.idIndicazione= i.id
					inner join Azione a on op.idAzione=a.id
				) on os.id=op.idOsservazione

where (@idScheda IS NOT NULL AND idScheda=@idScheda)
				or (@idOsservazione IS NOT NULL AND idOsservazione =@idOsservazione)

) t
pivot (
	COUNT(idindicazione)
	for Indicazione in (
		[Prima contatto paziente],
		[Prima di manovra di asepsi],
		[Dopo contatto fluido],
		[Dopo contatto paziente],
		[Dopo contatto ambiente ])

	) as pivot_table

	INSERT INTO @appoggio 
	select [tipologia],  0, 0, 0, 0, 0 from dbo.Azione  a where not exists (select 1 from @appoggio t2 where a.tipologia = t2.Azione)
	
	insert @temp select * from @appoggio order by Azione
	
	RETURN
END