
/******************************
** Name: CargoWiseFileProcess
** Desc: Process CargoWiseFile entries for sending to SAP
** Auth: Dewald Nel
** Date: 2020-09-03
**************************
** Change History
**************************
** PR   Date				Description 
** --   --------			------------------------------------
** 1	2020-09-03			Creation
*******************************/
CREATE PROCEDURE [CargoWiseFileProcess]
WITH ENCRYPTION
AS
BEGIN
	DECLARE @FileContextID INT;

	SELECT		TOP 1 @FileContextID = [ID]
	FROM		[CargoWiseFile]
	WHERE		[SAPProcessed] = 0;

	IF @FileContextID IS NOT NULL
	BEGIN
		SELECT		[FileContext]
		FROM		[CargoWiseFile]
		WHERE		[ID] = @FileContextID;

		UPDATE		[CargoWiseFile]
		SET			[SAPProcessed] = 1,
					[SAPProcessedDate] = GETDATE()
		WHERE		[ID] = @FileContextID;
	END;
END

