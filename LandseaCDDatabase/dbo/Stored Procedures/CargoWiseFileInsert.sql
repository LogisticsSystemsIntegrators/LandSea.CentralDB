
/******************************
** Name: CargoWiseFileInsert
** Desc: Insert CargoWise files into CargoWiseFile
** Auth: Dewald Nel
** Date: 2020-09-03
**************************
** Change History
**************************
** PR   Date				Description 
** --   --------			------------------------------------
** 1	2020-09-03			Creation
*******************************/
CREATE PROCEDURE [CargoWiseFileInsert]
(
	@FileContext XML
)
WITH ENCRYPTION
AS
BEGIN
	INSERT INTO [CargoWiseFile]
	(
		[FileContext],
		[CreatedDate],
		[SAPProcessed]
	)
	VALUES
	(
		@FileContext,
		GETDATE(),
		0
	)
END

