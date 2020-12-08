-- ====================================================================================
-- Author:		BSW
-- Create date: 2020-12-08
-- Description:	Extract CargoWise File with updated ETN Numbers - not already extrcted
---
--**************************
--** Change History
--**************************
--** PR   Date				Description 
-- ----   ---			---------------------------------------------------------------
-- 
-- ====================================================================================
CREATE PROCEDURE PaymentExtraction 
 WITH ENCRYPTION
AS
BEGIN
	
	SET NOCOUNT ON;

	Declare @FileID int;
	Declare @CargoWiseKey varchar(50);

	Select TOP 1 @FileID = F.ID
		,@CargoWiseKey = F.[Key]
	From 
		dbo.CargoWiseFile F 
		Left Outer Join dbo.FileExtractionHistory EH On (F.ID = EH.CargoWiseFileID And EH.ExtractionType = 'PM')
	Where 
		F.ETNNumber IS NOT NULL
		And EH.CargoWiseFileID IS NULL
		And F.XMLType = 'ForwardingShipment'
	Order By 
		F.CreatedDate ASC;

	IF @FileID IS NOT NULL
		Begin
			
			Insert Into dbo.FileExtractionHistory(CargoWiseFileID,CargoWiseKey,ExtractionType,ExtractionDate)
			Values(@FileID,@CargoWiseKey,'PM',GETDATE());

			Select 
				FileContext
			From 
				dbo.CargoWiseFile
			Where
				ID = @FileID;

		End

END
GO
