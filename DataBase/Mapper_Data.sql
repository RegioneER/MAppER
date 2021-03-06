USE Mapper
BEGIN TRANSACTION
SET IDENTITY_INSERT [dbo].[Azione] ON
INSERT INTO [dbo].[Azione] ([id], [tipologia], [ordinale], [adesione]) VALUES (1, N'lavaggio', 1, 1)
INSERT INTO [dbo].[Azione] ([id], [tipologia], [ordinale], [adesione]) VALUES (2, N'frizione', 2, 1)
INSERT INTO [dbo].[Azione] ([id], [tipologia], [ordinale], [adesione]) VALUES (3, N'nessuna', 3, 0)
INSERT INTO [dbo].[Azione] ([id], [tipologia], [ordinale], [adesione]) VALUES (4, N'nessuna con guanti', 4, 0)
SET IDENTITY_INSERT [dbo].[Azione] OFF

SET IDENTITY_INSERT [dbo].[StatoSessione] ON
INSERT INTO [dbo].[StatoSessione] ([id], [nome], [DescrizionePubblica]) VALUES (1, N'NONE      ', N'Nessuna             ')
INSERT INTO [dbo].[StatoSessione] ([id], [nome], [DescrizionePubblica]) VALUES (2, N'ACTIVATED ', N'In lavorazione      ')
INSERT INTO [dbo].[StatoSessione] ([id], [nome], [DescrizionePubblica]) VALUES (3, N'CLOSED    ', N'Consolidata         ')
INSERT INTO [dbo].[StatoSessione] ([id], [nome], [DescrizionePubblica]) VALUES (4, N'DELETED   ', N'Cancellata          ')
INSERT INTO [dbo].[StatoSessione] ([id], [nome], [DescrizionePubblica]) VALUES (5, N'DELETING  ', N'In cancellazione    ')
SET IDENTITY_INSERT [dbo].[StatoSessione] OFF

SET IDENTITY_INSERT [dbo].[Operatore] ON
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (1, N'1.1       ', N'Infermiere', N'Infermiere-a/ostetrico-a', N'opeAzzurro')
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (2, N'1.2       ', N'Ostetrica', N'Infermiere-a/ostetrico-a', N'opeRosa')
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (3, N'1.3       ', N'Studente Infermiere', N'Infermiere-a/ostetrico-a', N'opeGrigio')
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (4, N'2.1       ', N'Operatore di supporto all''assistenza', N'Operatore di supporto all''assistenza', N'opeGiallo')
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (7, N'3.1       ', N'Medico internista medicina generale/specialistica', N'Medico', N'opeBianco')
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (8, N'3.2       ', N'Chirurgo', N'Medico', N'opeVerde')
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (9, N'3.3       ', N'Anestesista / Rianimatore / Medico di pronto soccorso', N'Medico', N'opeBlu')
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (10, N'3.4       ', N'Pediatra', N'Medico', N'opeRosso')
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (11, N'3.5       ', N'Ginecologo', N'Medico', N'opeRosaScuro')
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (12, N'3.7       ', N'Studente di Medicina', N'Medico', N'opeArancione')
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (13, N'4.1       ', N'Terapista', N'Altro operatore sanitario', N'opeViola')
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (14, N'4.2       ', N'Tecnico', N'Altro operatore sanitario', N'opeViola')
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (15, N'4.3       ', N'Altro', N'Altro operatore sanitario', N'opeNeutro')
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (16, N'4.4       ', N'Studente Altro', N'Altro operatore sanitario', N'opeGialloScuro')
INSERT INTO [dbo].[Operatore] ([id], [codCategoria], [nomeCategoria], [classe], [ClasseColore]) VALUES (17, N'3.6       ', N'Specializzando Medico', N'Medico', N'opeMarrone')
SET IDENTITY_INSERT [dbo].[Operatore] OFF

INSERT INTO [dbo].[StatoCandidatura] ([Codice], [Descrizione]) VALUES (0, N'In attesa di approvazione')
INSERT INTO [dbo].[StatoCandidatura] ([Codice], [Descrizione]) VALUES (1, N'Approvata')
INSERT INTO [dbo].[StatoCandidatura] ([Codice], [Descrizione]) VALUES (2, N'Rifiutata')
INSERT INTO [dbo].[StatoCandidatura] ([Codice], [Descrizione]) VALUES (3, N'Censito')

INSERT INTO [dbo].[Bacteria] ([code], [description_EN], [description_IT], [ordinale]) VALUES (N'CDI                 ', N'Clostridium difficile pseudomonas ', N'Clostridium difficile', 1)
INSERT INTO [dbo].[Bacteria] ([code], [description_EN], [description_IT], [ordinale]) VALUES (N'CRE                 ', N'Carbapenems resistant Enterobacteriaceae ', N'Enterobatteri Resistenti ai carbapenemi', 2)
INSERT INTO [dbo].[Bacteria] ([code], [description_EN], [description_IT], [ordinale]) VALUES (N'CRPSA               ', N' pseudomonas  resistente ai carbapenemi', N' Pseudomonas  resistente ai carbapenemi', 3)
INSERT INTO [dbo].[Bacteria] ([code], [description_EN], [description_IT], [ordinale]) VALUES (N'ESBL                ', N'Extended-spectrum beta (β)-lactamase gram-negative organisms', N' Batteri gram negativi produttori di beta-lattamasi ad ampio spettro', 4)
INSERT INTO [dbo].[Bacteria] ([code], [description_EN], [description_IT], [ordinale]) VALUES (N'MRAB                ', N'Multi-resistant Acinetobacter baumannii ', N'Acinetobacter Baumannii multi-resistente', 5)
INSERT INTO [dbo].[Bacteria] ([code], [description_EN], [description_IT], [ordinale]) VALUES (N'MRSA                ', N'Methicillin resistant Staphyloccous aureus', N' Staphyloccous Aureus meticillino-resistente', 6)
INSERT INTO [dbo].[Bacteria] ([code], [description_EN], [description_IT], [ordinale]) VALUES (N'nessun batterio     ', NULL, N'nessun microrganismo', 8)
INSERT INTO [dbo].[Bacteria] ([code], [description_EN], [description_IT], [ordinale]) VALUES (N'VRE                 ', N'Vancomycin resistant Enterococci spp.', N' Enterococco Vancomicina resistente', 7)

SET IDENTITY_INSERT [dbo].[Indicazione] ON
INSERT INTO [dbo].[Indicazione] ([id], [tipologia], [ordinale]) VALUES (1, N'Prima contatto paziente', 1)
INSERT INTO [dbo].[Indicazione] ([id], [tipologia], [ordinale]) VALUES (2, N'Prima di manovra di asepsi', 2)
INSERT INTO [dbo].[Indicazione] ([id], [tipologia], [ordinale]) VALUES (3, N'Dopo contatto fluido', 3)
INSERT INTO [dbo].[Indicazione] ([id], [tipologia], [ordinale]) VALUES (4, N'Dopo contatto paziente', 4)
INSERT INTO [dbo].[Indicazione] ([id], [tipologia], [ordinale]) VALUES (5, N'Dopo contatto ambiente ', 5)
SET IDENTITY_INSERT [dbo].[Indicazione] OFF

SET IDENTITY_INSERT [dbo].[Ruoli] ON
INSERT INTO [dbo].[Ruoli] ([id], [nome], [ordinale]) VALUES (1, N'Osservatore                     ', 4)
INSERT INTO [dbo].[Ruoli] ([id], [nome], [ordinale]) VALUES (2, N'Aziendale                      ', 2)
INSERT INTO [dbo].[Ruoli] ([id], [nome], [ordinale]) VALUES (3, N'Regionale                     ', 1)
INSERT INTO [dbo].[Ruoli] ([id], [nome], [ordinale]) VALUES (4, N'Referente di struttura', 3)
INSERT INTO [dbo].[Ruoli] ([id], [nome], [ordinale]) VALUES (5, N'Non Associato                    ', 5)
SET IDENTITY_INSERT [dbo].[Ruoli] OFF

SET IDENTITY_INSERT [dbo].[LogType] ON
INSERT INTO [dbo].[LogType] ([id], [descrizione]) VALUES (1, N'Exceptions')
INSERT INTO [dbo].[LogType] ([id], [descrizione]) VALUES (2, N'Access')
INSERT INTO [dbo].[LogType] ([id], [descrizione]) VALUES (3, N'Permissions')
INSERT INTO [dbo].[LogType] ([id], [descrizione]) VALUES (4, N'Update')
SET IDENTITY_INSERT [dbo].[LogType] OFF

INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('A04','RIA',NULL,0,14,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('A05','HOSPICE',NULL,0,15,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('A06','DAY SURGERY',NULL,0,16,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('A08','STRUTTURA INTERMEDIA SANITARIA TERRITORIALE - OSPEDALE DI COMUNITA',NULL,1,17,19)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('A09','DAY HOSPITAL PSICHIATRICO',NULL,0,18,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('A10','FARMACEUTICA',NULL,0,19,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('A97','CARCERE',NULL,0,22,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('A98','PRONTO SOCCORSO',NULL,0,23,8)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('A99','ALTRO',NULL,0,20,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('H00','AZIENDA OSPEDALIERA','AOSP',1,1,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('H01','OSPEDALE A GESTIONE DIRETTA PRESIDIO DELLA U.S.L.','AUSL',1,2,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('H02','AZIENDA OSPEDALIERO-UNIVERSITARIA E POLICLINICO','AOSP',1,3,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('H03','ISTITUTO DI RICOVERO E CURA A CARATTERE SCIENTIFICO','IRCSS',1,4,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('H04','OSPEDALE CLASSIFICATO O ASSIMILATO AI SENSI DELL''ARTICOLO 1, ULTIMO COMMA, DELLA LEGGE 132/1968','AUSL',1,5,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('H05','CASA DI CURA PRIVATA','AUSL',0,6,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('H06','ISTITUTO PSICHIATRICO RESIDUALE','AUSL',1,7,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('H08','ISTITUTO QUALIFICATO PRESIDIO DELLA U.S.L.','AUSL',1,8,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('H09','ENTE DI RICERCA','AUSL',1,9,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('H95','DAY SURGERY','AUSL',0,21,NULL)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('S01','AMBULATORIO E LABORATORIO',NULL,0,10,11)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('S02','ALTRO TIPO DI STRUTTURA TERRITORIALE',NULL,0,11,19)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('S03','STRUTTURA SEMIRESIDENZIALE',NULL,0,12,18)
INSERT INTO [dbo].[TipologiaStruttura] ([CodTipologia],[Descrizione],[TipoEnte],[IsAttivo],[Ordinale],[CodAreaDisciplina]) VALUES ('S04','STRUTTURA RESIDENZIALE',NULL,1,13,18)

INSERT INTO [dbo].[Utente] ([username],[nome],[cognome],[email],[idRuolo],[attivato],[cancellato],[CodiceFiscale]) VALUES ('utenteRegionale','Utente','Regionale','utente@regione.it',3,1,0,NULL)
INSERT INTO [dbo].[Utente] ([username],[nome],[cognome],[email],[idRuolo],[attivato],[cancellato],[CodiceFiscale]) VALUES ('utenteProva','Utente','Prova','utente@prova.it',1,1,0,'AAABBB70B15F097T')
INSERT INTO [dbo].[UtenteStruttura]([idUtente],[codRegione],[codAzienda],[idStrutturaErogatrice],[idWebServiceStruttura],[idReparto],[idWebServiceReparto],[dataDal],[dataAl]) VALUES (1,'080','908',80908,1,2250,1,'2021-01-01',NULL)

COMMIT TRANSACTION
