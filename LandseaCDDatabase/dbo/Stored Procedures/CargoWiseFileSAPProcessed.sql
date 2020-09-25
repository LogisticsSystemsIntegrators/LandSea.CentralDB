
/******************************
** Name: CargoWiseFileLandseaProcessed
** Desc: Mark XML files as processed by Landsea Global
** Auth: Dewald Nel
** Date: 2020-09-03
**************************
** Change History
**************************
** PR   Date				Description 
** --   --------			------------------------------------
** 1	2020-09-25			Creation
*******************************/
CREATE PROCEDURE [CargoWiseFileLandseaProcessed]
(
	@MessageID INT
)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @ReturnID INT;

	SELECT		@ReturnID = ISNULL([ID], -1)
	FROM		[CargoWiseFile]
	WHERE		[ID] = @MessageID;

	IF @ReturnID > -1
	BEGIN
		UPDATE		[CargoWiseFile]
		SET			[LandseaProcessed] = 1,
					[LandseaProcessedDate] = GETDATE()
		WHERE		[ID] = @MessageID
	END;

	SELECT		@ReturnID;
END

