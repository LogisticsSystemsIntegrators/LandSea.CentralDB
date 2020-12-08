-- ====================================================================================
-- Author:		BSW
-- Create date: 2020-12-07
-- Description:	Update the ETN Number in the Cargo Wsie file table
---
--**************************
--** Change History
--**************************
--** PR   Date				Description 
-- ----   ---			---------------------------------------------------------------
-- 01	2020-12-08		Add Update to new extaction history table/ ETN refernece filed on xml
-- ====================================================================================
CREATE PROCEDURE ETNNumberUpdate
	@CargoWiseKey varchar(20),
	@ETNNumber varchar(20)
WITH ENCRYPTION
AS
BEGIN
	
	SET NOCOUNT ON;
	
	Declare @Message varchar(max);
	Declare @Done int,@NodeCount int;

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


	--We need to update all the XML with the new ETN number - 1) Add the SellRefernce if it does not exists
	Select
		@NodeCount = FileContext.value('count(/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:JobCosting/*:ChargeLineCollection/*:ChargeLine)','INT') 
	From 
		dbo.CargoWiseFile 
	Where [Key] = @CargoWiseKey;

	Set @Done = @NodeCount;

	WHILE @Done > 0
		Begin
		
			Update dbo.CargoWiseFile
				Set FileContext.modify('insert <SellReference>sql:variable("@ETNNumber")</SellReference> into ((/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:JobCosting/*:ChargeLineCollection/*:ChargeLine)[sql:variable("@Done")])[1]')
			Where 
				[Key] = @CargoWiseKey
				And FileContext.exist('(/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:JobCosting/*:ChargeLineCollection/*:ChargeLine[sql:variable("@Done")]/SellReference)') = 0

			Set @Done = @Done - 1

		End

	--Make sure all values are 
	Set @Done = @NodeCount;

	WHILE @Done > 0
		Begin
		
			Update CargoWiseFile
				Set FileContext.modify('replace value of ((/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:JobCosting/*:ChargeLineCollection/*:ChargeLine)[sql:variable("@Done")]/*:SellReference/text())[1] 
				with sql:variable("@ETNNumber")')
			Where [Key] = @CargoWiseKey
		
			Set @Done = @Done - 1

		End


	Delete From dbo.FileExtractionHistory Where CargoWiseKey = @CargoWiseKey And ExtractionType = 'PM';

END
GO
