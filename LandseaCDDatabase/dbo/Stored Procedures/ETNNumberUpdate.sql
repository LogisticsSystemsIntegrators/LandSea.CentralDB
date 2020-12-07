-- ====================================================================================
-- Author:		BSW
-- Create date: 2020-12-07
-- Description:	Update the ETN Number in the Cargo Wsie file table
-- ====================================================================================
CREATE PROCEDURE ETNNumberUpdate
	@CargoWiseKey varchar(20),
	@ETNNumber varchar(20)
WITH ENCRYPTION
AS
BEGIN
	
	SET NOCOUNT ON;
	
	Declare @Message varchar(max);

	If NOT EXISTS(Select * From dbo.CargoWiseFile Where [Key] = @CargoWiseKey)
		Begin
			Set @Message = 'Invalid Key Value: ' + @CargoWiseKey + ' was not found.';
			RAISERROR (@Message, 16, 1);  
			return
		End

	Update dbo.CargoWiseFile
		Set ETNNUmber = @ETNNumber
	Where 
		[Key] = @CargoWiseKey;


END
GO
