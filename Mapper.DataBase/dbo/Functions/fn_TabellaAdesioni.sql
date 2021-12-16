CREATE  FUNCTION [dbo].[fn_TabellaAdesioni]
(
	@idScheda int = null,
	@idOsservazione int = null
)
RETURNS @temp TABLE 
(
Indicazione varchar(30),
	idIndicazione int PRIMARY KEY,	
	Adesioni int,
	NonAdesioni int,
	TotIndicazioni int,
	PercAdesione decimal,
	PercNonAdesione decimal
)
AS
BEGIN

DECLARE @appoggio TABLE 
(
	Indicazione varchar(30),
	idIndicazione int,	
	Adesioni int,
	NonAdesioni int,
	TotIndicazioni int,
	PercAdesione decimal,
	PercNonAdesione decimal
)

insert into @appoggio  
	select *,
		 [1] + [0] Totale , 
		([1]*100/([1] + [0])) PercAdesione ,
		(100- ([1]*100/([1] + [0]))) PercNonAdesione

		 from (
			select a.adesione Adesione,
					i.tipologia,
					i.id

			from  Osservazione os inner join 
				(
				Opportunita op 
					inner join Indicazione i on op.idIndicazione= i.id
					inner join Azione a on op.idAzione=a.id
				) on os.id=op.idOsservazione
			where (@idScheda IS NOT NULL AND idScheda=@idScheda)
				OR (@idOsservazione IS NOT NULL AND idOsservazione =@idOsservazione)

			
) t
pivot (
	Count(adesione)
	for adesione in ([1],
	[0])
	) 
	as pivot_table
	order by id

	INSERT INTO @appoggio 
	select [tipologia], id, 0, 0, 0, 0, 0 from dbo.Indicazione  i where not exists (select 1 from @appoggio t2 where i.tipologia = t2.Indicazione)
	
	insert @temp select * from @appoggio order by Indicazione

	RETURN 
END