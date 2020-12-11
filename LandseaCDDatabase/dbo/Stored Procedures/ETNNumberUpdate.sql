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
-- 02   2020-12-09		Add new GIB Invoice number column as part of the update to the table
-- 03   2020-12-10		Add the new Update and Customized Filed update to the XML file.
-- ====================================================================================
Create PROCEDURE ETNNumberUpdate
	@CargoWiseKey varchar(20),
	@ETNNumber varchar(20),
	@GIBInvoiceNumber varchar(50) = null
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
		,GIBInvoiceNumber = @GIBInvoiceNumber
	Where 
		[Key] = @CargoWiseKey;

    -- We need to update/add the Customized field nodes to al lthe matching XML's, also add in new Sellers Ref
	Declare @CustomFieldCount int,@ActiveID int;
	Declare @HasETNNumber bit,@HasGIBINvoice bit;

	Set @HasETNNumber = 0;
	Set @HasGIBINvoice = 0;

	IF OBJECT_ID('tempdb..#TempCargoFile') IS NOT NULL DROP TABLE #TempCargoFile;
		CREATE TABLE #TempCargoFile (FileID int);
		CREATE CLUSTERED INDEX ix_TempCargoFileID ON #TempCargoFile (FileID);

	--get the linked entities based on the user access and active client
	INSERT #TempCargoFile (FileID)
		SELECT 	
			ID
		From 
			dbo.CargoWiseFile
		Where [Key] = @CargoWiseKey;

	if @GIBInvoiceNumber IS NULL  --we cannot update xml values with null
		Set @GIBInvoiceNumber = '';

	--we have to loop thru all the files with the same key, and update all linked xmls
	 while (Select COUNT(*) From #TempCargoFile) > 0
		Begin
		
			Select TOP 1 @ActiveID = FileID From #TempCargoFile;

			Set @HasETNNumber = 0;
			Set @HasGIBINvoice = 0;
			Set @CustomFieldCount = 0;
			Set @NodeCount = 0;

			--we need the count of ChargeLines - to all SellRefence
			Select
				@NodeCount = FileContext.value('count(/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:JobCosting/*:ChargeLineCollection/*:ChargeLine)','INT') 
			From 
				dbo.CargoWiseFile 
			Where ID = @ActiveID;
					
			WHILE @NodeCount > 0
				Begin
					--we need to delete nodes - if exists, we cannot update empty node values
					Update CargoWiseFile
						Set FileContext.modify('delete ((/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:JobCosting/*:ChargeLineCollection/*:ChargeLine)[sql:variable("@NodeCount")]/*:SellReference)')
					Where 
						ID = @ActiveID;
					---re-ad the new node with the updated value
					Update dbo.CargoWiseFile
						Set FileContext.modify('insert <SellReference>{sql:variable("@ETNNumber")}</SellReference> into ((/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:JobCosting/*:ChargeLineCollection/*:ChargeLine)[sql:variable("@NodeCount")])[1]')
					Where 
						ID = @ActiveID;
					
					Set @NodeCount = @NodeCount - 1

				End

			--if no customeized collection exists - add the full new one
			IF EXISTS(Select * From CargoWiseFile Where ID = @ActiveID And FileContext.exist('/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:CustomizedFieldCollection') = 0)
				Begin
					Update dbo.CargoWiseFile
							Set FileContext.modify('insert 
									<CustomizedFieldCollection>
										<CustomizedField>
											<DataType>String</DataType>
											<Key>ETTN Number</Key>
											<Value>{sql:variable("@ETNNumber")}</Value>
										</CustomizedField>
										<CustomizedField>
											<DataType>String</DataType>
											<Key>GIB Invoice Number</Key>
											<Value>{sql:variable("@GIBInvoiceNumber")}</Value>
										</CustomizedField>
									</CustomizedFieldCollection> 
									into (/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment)[1]')
						Where 
							id = @ActiveID
							And FileContext.exist('/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:CustomizedFieldCollection') = 0
				End
			Else
				Begin

					--we need to check each Customzied Field child node
					Select
						@CustomFieldCount = FileContext.value('count(/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:CustomizedFieldCollection/*:CustomizedField)','INT') 
					From 
						dbo.CargoWiseFile 
					Where id = @ActiveID

					while @CustomFieldCount > 0
						Begin

							if exists (select * from CargoWiseFile where ID = @ActiveID And FileContext.value('((/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:CustomizedFieldCollection/*:CustomizedField)[sql:variable("@CustomFieldCount")]/*:Key)[1]','varchar(50)') = 'ETTN Number')
								begin
									Set @HasETNNumber = 1;
									--we need to delete the existing vlaue node, incase it's "empty" "empty" nodes cannot be updated
									Update CargoWiseFile
										Set FileContext.modify('delete ((/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:CustomizedFieldCollection/*:CustomizedField)[sql:variable("@CustomFieldCount")]/*:Value)')
									Where ID = @ActiveID
								
									--add the new node with it's value
									Update dbo.CargoWiseFile
										Set FileContext.modify('insert <Value>{sql:variable("@ETNNumber")}</Value>
												into ((/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:CustomizedFieldCollection/*:CustomizedField)[sql:variable("@CustomFieldCount")])[1]')
										Where 
											id = @ActiveID
		
								End
								--we do the same for the GIB invoice field
							if exists (select * from CargoWiseFile where ID = @ActiveID And FileContext.value('((/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:CustomizedFieldCollection/*:CustomizedField)[sql:variable("@CustomFieldCount")]/*:Key)[1]','varchar(50)') = 'GIB Invoice Number')
								begin
									Set @HasGIBINvoice = 1;
								
									Update CargoWiseFile
										Set FileContext.modify('delete ((/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:CustomizedFieldCollection/*:CustomizedField)[sql:variable("@CustomFieldCount")]/*:Value)')
									Where ID = @ActiveID
								
									Update dbo.CargoWiseFile
										Set FileContext.modify('insert <Value>{sql:variable("@GIBInvoiceNumber")}</Value>
												into ((/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:CustomizedFieldCollection/*:CustomizedField)[sql:variable("@CustomFieldCount")])[1]')
										Where 
											id = @ActiveID
								End
							Set @CustomFieldCount = @CustomFieldCount - 1;
						End
		
					If @HasETNNumber = 0 --value have not found - only other custom fields
						Begin
							Update dbo.CargoWiseFile
								Set FileContext.modify('insert 
											<CustomizedField>
												<DataType>String</DataType>
												<Key>ETTN Number</Key>
												<Value>{sql:variable("@ETNNumber")}</Value>
											</CustomizedField>
										into (/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:CustomizedFieldCollection)[1]')
							Where 
								id = @ActiveID
						End
		
					if @HasGIBINvoice = 0
						Begin
							Update dbo.CargoWiseFile
								Set FileContext.modify('insert 
											<CustomizedField>
												<DataType>String</DataType>
												<Key>GIB Invoice Number</Key>
												<Value>{sql:variable("@GIBInvoiceNumber")}</Value>
											</CustomizedField>
										into (/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:CustomizedFieldCollection)[1]')
							Where 
								id = @ActiveID
						
						End
				End

			Delete From #TempCargoFile Where FileID = @ActiveID;

		End


	Delete From dbo.FileExtractionHistory Where CargoWiseKey = @CargoWiseKey And ExtractionType = 'SF';

END
GO
