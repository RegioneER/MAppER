
CREATE PROCEDURE [dbo].[EsportaOpportunita]
(
	@codRegione varchar(10) = NULL,
	@codAzienda varchar(10) = NULL,
	@codTipoStruttura varchar(10) = NULL,
	@keyStruttura varchar(20) = NULL,
	@keyReparto varchar(20) = NULL,
	@statoSessione int = NULL,
	@dataDal datetime = NULL,
	@dataAl datetime = NULL,
	@cancellate Bit = NULL,
	@idScheda int = NULL
)
AS
 BEGIN
		 SELECT DISTINCT
			s.id IdScheda
			, s.data dataScheda
			, s.dataInserimento dataInserimentoScheda
			, r.CodRegione codiceRegione
			, reg.Denominazione nomeRegione
			, r.CodAzienda codiceAzienda
			, a.Denominazione nomeAzienda
			, r.CodiceTipologiaStruttura 
			, ts.Descrizione nomeTipologiaStruttura
			, sb.codMin codMinStruttura
			, sb.subCodMin subCodMinStruttura
			, sb.IdStrutturaErogatrice
			, sb.idWebServiceStruttura
			, sb.KeyStruttura
			, sb.Denominazione nomeStruttura
			, sb.Indirizzo indirizzoStruttura
			, r.nome nomeDisciplinaArea
			, r.CodDisciplina codiceDisciplina
			, r.NomeOriginale nomeDisciplina
			, r.IdReparto
			, r.IdWebServiceReparto
			, r.KeyReparto
			, r.nome nomeReparto
			, ss.nome statoSessioneScheda
			, s.durataSessione
			, oss.numOperatori numeroOperatori
			, ope.codCategoria codiceCategoriaOperatori
			, ope.nomeCategoria nomeCategoriaOperatore
			, ope.classe classeOperatore
			, oss.operatoreEsterno operatoreEsterno
			, opp.data dataOraOpportunità
			, az.id codiceTipologiaAzione
			, az.tipologia tipologiaAzione
			, ind.id codiceTipologiaIndicazione
			, ind.tipologia TipologiaIndicazione	
			, u.id idUtente
			, ru.nome ruoloUtente
			
		from  Scheda s 
			inner join vwReparto r on s.idReparto = r.IdReparto and s.idWebServiceReparto=r.IdWebServiceReparto
			inner join vwRegione reg on r.CodRegione= reg.CodRegione
			inner join vwAzienda a on r.CodAzienda = a.CodAzienda and r.CodRegione= a.CodRegione and (a.DataInizio<= s.dataInserimento and (a.DataFine is null OR a.DataFine>=s.dataInserimento))
			inner join vwStruttura sb on r.IdStrutturaErogatrice= sb.IdStrutturaErogatrice and r.idWebServiceStruttura= sb.idWebServiceStruttura and (sb.DataRiferimentoStruttura <= s.dataInserimento and (sb.DataFine is null OR sb.DataFine >= s.dataInserimento))
			inner join TipologiaStruttura ts on sb.codiceTipologiaStruttura = ts.CodTipologia
			inner join StatoSessione ss on s.idStatoSessione = ss.id
			inner join Utente u on s.idUtente = u.id
			inner join Ruoli ru on u.idRuolo=ru.id
			left join Osservazione oss on s.id=oss.idScheda
			left join Opportunita opp on oss.id = opp.idOsservazione
			left join Operatore ope on oss.idOperatore = ope.id
			left join Azione az on opp.idAzione = az.id
			left join Indicazione ind on opp.idIndicazione= ind.id

		where 
			(@codRegione is null or r.CodRegione=@codRegione)
			and (@codAzienda is null or r.CodAzienda=@codAzienda)
			and (@codTipoStruttura is null or r.codiceTipologiaStruttura=@codTipoStruttura)
			and (@keyStruttura is null or r.keyStruttura = @keyStruttura)
			and (@keyReparto is null or r.KeyReparto = @keyReparto)
			and (@statoSessione is null or s.idStatoSessione=@statoSessione)
			and (@dataDal is null or s.dataInserimento >= @dataDal)
			and (@dataAl is null or s.data <=@dataAl)
			and (@idScheda is null or s.id=@idScheda)
			and (@cancellate is null or @cancellate=1 Or (@cancellate=0 AND idStatoSessione <> 4) )
END
GO


