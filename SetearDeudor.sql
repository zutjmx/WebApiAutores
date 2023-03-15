USE [db-slp]
GO
/****** Object:  StoredProcedure [dbo].[SetearMalaPaga]    Script Date: 09-Sep-21 7:47:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SetearMalaPaga]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE AspNetUsers
	set 
	MalaPaga = 'True'
	from Facturas
	inner join AspNetUsers
	on AspNetUsers.Id = Facturas.UsuarioId
	where Pagada = 'False' and FechaLimiteDePago < GETDATE()
END
