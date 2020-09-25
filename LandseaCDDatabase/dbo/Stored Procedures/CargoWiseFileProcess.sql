
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
** 2	2020-09-25			Add XML Type (Shipment/Organisation), Search Value parameters
*******************************/
CREATE PROCEDURE [CargoWiseFileProcess]
(
	@XMLType VARCHAR(20),
	@KeyValue VARCHAR(50)
)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @FileContextID INT;

	SELECT		TOP 1 @FileContextID = [ID]
	FROM		[CargoWiseFile]
	WHERE		[SAPProcessed] = 0
	AND			[LandseaProcessed] = 0
	AND			[XMLType] = @XMLType
	AND			[Key] = @KeyValue;

	IF @FileContextID IS NOT NULL
	BEGIN
		SELECT		[ID],
					[FileContext]
		FROM		[CargoWiseFile]
		WHERE		[ID] = @FileContextID;

		UPDATE		[CargoWiseFile]
		SET			[SAPProcessed] = 1,
					[SAPProcessedDate] = GETDATE()
		WHERE		[ID] = @FileContextID;
	END;
END

