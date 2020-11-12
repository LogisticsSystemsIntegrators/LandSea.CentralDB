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
** 3	2020-11-12			Remove SAPProcessed = 0 from first select
*******************************/
CREATE PROCEDURE [CargoWiseFileProcess]
(
	@XMLType VARCHAR(20),
	@KeyValue VARCHAR(50) = NULL
)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @FileContextID INT;

	SELECT		TOP 1 @FileContextID = [ID]
	FROM		[CargoWiseFile]
	WHERE		[LandseaProcessed] = 0
	AND			[XMLType] = @XMLType
	AND			(@KeyValue IS NULL OR [Key] = @KeyValue)
	ORDER BY	[CreatedDate] ASC;

	IF @FileContextID IS NOT NULL
	BEGIN
		SELECT		[ID] AS [MessageID],
					[FileContext] AS [Message]
		FROM		[CargoWiseFile]
		WHERE		[ID] = @FileContextID;

		UPDATE		[CargoWiseFile]
		SET			[SAPProcessed] = 1,
					[SAPProcessedDate] = GETDATE()
		WHERE		[ID] = @FileContextID;
	END;
END

