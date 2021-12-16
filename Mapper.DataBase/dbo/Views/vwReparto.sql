













CREATE VIEW [dbo].[vwReparto]
AS

SELECT 1 IdWebServiceReparto,
		r.IdReparto,
		 (CONVERT(varchar,r.IdReparto) + '.' + CONVERT(varchar,1)) KeyReparto,
		r.IdStrutturaErogatrice,
		vwS.DataRiferimentoStruttura DataRiferimentoStruttura,
		vwS.idWebServiceStruttura,
		vws.keyStruttura,
		vws.keyStrutturaData,
		r.DataInizio,
		r.DataFine,
		d.Descrizione + ' ' + r.ProgressivoDivisione NomeOriginale, 
	 	iif(ra.Nome is null, d.Descrizione + ' ' + r.ProgressivoDivisione, ra.Nome) Nome, -- nome semplice(reale o alias)
		iif(ra.Descrizione is null, '', ra.Descrizione) Descrizione,
		iif(ra.Nome is null, d.Descrizione + ' ' + r.ProgressivoDivisione, ra.Nome + ' [' + d.Descrizione + ' ' + r.ProgressivoDivisione + ']') NomeCompleto, --nome per le combo (reale o alias + reale)
		r.CodDisciplina,
		r.ProgressivoDivisione,
		vwS.CodRegione,
		vwS.CodAzienda,
		vwS.CodiceTipologiaStruttura,
		vwS.CodTipoAnagrafeRegionale,
		dad.codAreaDisciplina,
		iif(ra.cancellato is null, convert(bit,0),convert(bit,ra.cancellato)) Cancellato,
		iif(ra.IdReparto is null,1,2) Stato,
		convert(bit, iif(r.DataInizioStrutturaErogatrice = vwS.DataRiferimentoStruttura,0, 1)) isStorico
		
FROM [GlobalSanita].dbo.Reparto r
join GlobalSanita.dbo.Disciplina d on r.CodDisciplina=d.CodDisciplina 
join vwStruttura vwS on r.IdStrutturaErogatrice = vwS.IdStrutturaErogatrice  
left join GlobalSanita.dbo.RepartoAlias ra on r.IdReparto = ra.idReparto
left join GlobalSanita.dbo.DisciplinaAreaDisciplina dad on d.CodDisciplina=dad.CodDisciplina

WHERE ra.idapplicazione is null OR ra.idApplicazione=1

UNION

select 2 IdWebServiceReparto,
		rc.IdReparto,
		 (CONVERT(varchar,rc.IdReparto) + '.' + CONVERT(varchar,2)) KeyReparto,
		rc.IdStrutturaErogatrice,
		vwS.DataRiferimentoStruttura DataRiferimentoStruttura,
		vwS.idWebServiceStruttura,
		vws.keyStruttura,
		vws.keyStrutturaData,
		rc.DataInizio,
		rc.DataFine,
		rc.nome NomeOriginale,
	 	rc.Nome,
		rc.Descrizione,
		rc.Nome NomeCompleto,
		rc.CodDisciplina,
		rc.ProgressivoDivisione,
		vwS.CodRegione,
		vwS.CodAzienda,
		vwS.CodiceTipologiaStruttura,
		vwS.CodTipoAnagrafeRegionale,
		rc.codAreaDisciplina,
		rc.Cancellato,
		rc.Stato,
		Convert(bit,0) isStorico

FROM GlobalSanita.dbo.RepartoCustom rc 
join vwStruttura vwS on rc.IdStrutturaErogatrice = vwS.IdStrutturaErogatrice and rc.DataInizioStrutturaErogatrice = vwS.DataRiferimentoStruttura 

WHERE rc.idapplicazione is null OR rc.idApplicazione=1

