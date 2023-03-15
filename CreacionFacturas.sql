USE [db-slp]
GO
/****** Object:  StoredProcedure [dbo].[CreacionFacturas]    Script Date: 09-Sep-21 7:46:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		zutjmx@gmail.com
-- Create date: 15/03/2023
-- Description:	SP para un curso
-- =============================================
CREATE PROCEDURE [dbo].[CreacionFacturas]
	-- Add the parameters for the stored procedure here
	@fechaInicio datetime,
	@fechaFin datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @montoPorCadaPeticion decimal(4,4) = 1.0/5 -- 2.0/1000; -- 2 dolares por cada 1000 peticiones

	INSERT INTO Facturas (UsuarioId, Monto, FechaEmision, Pagada, FechaLimiteDePago)
	SELECT LlavesAPI.UsuarioId, 
	COUNT(*) * @montoPorCadaPeticion as monto,
	GETDATE() as fechaEmision,
	0 as Pagada,
	DATEADD(d, 60, GETDATE()) AS FechaLimiteDePago
	FROM Peticiones
	INNER JOIN LlavesAPI
	ON LlavesAPI.Id = Peticiones.LlaveId
	where LlavesAPI.TipoLlave != 1 and Peticiones.FechaPeticion >= @fechaInicio AND Peticiones.FechaPeticion < @fechaFin
	GROUP BY LlavesAPI.UsuarioId

	INSERT INTO FacturasEmitidas(Mes, Anio)
	Select 
	 CASE MONTH(GETDATE())
		WHEN 1 THEN 12
		ELSE MONTH(GETDATE())-1 END AS Mes,

	 CASE  MONTH(GETDATE())
		WHEN 1 THEN YEAR(GETDATE())-1
		ELSE YEAR(GETDATE()) END AS Anio
END
