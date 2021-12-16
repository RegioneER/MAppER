CREATE PROCEDURE [dbo].[LogConsolidaScheda_Aggiungi]
	@data datetime,
	@type int,
	@user nvarchar(10),
	@message nvarchar(50)

	AS

BEGIN
	insert into Log ([data], [type], [user], [message]) values (@data, @type, @user, @message)
END

GO


