
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
** 2	2020-09-25			Add LandseaProcessed flag
** 3    2020-12-07			Add code to handle Native XML/add Error raise if unsupported xml is received
** 4	2021-01-13			Add logic to extract Shipment when first node is ForwardingConsol
*******************************/
CREATE PROCEDURE [CargoWiseFileInsert]
(
	@FileContext XML
)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @XMLType VARCHAR(20), @Key VARCHAR(50);

	SET @XMLType = COALESCE(@FileContext.value('(/*:UniversalInterchange/*:Body/*:UniversalEvent/*:Event/*:DataContext/*:DataSourceCollection/*:DataSource/*:Type)[1]', 'varchar(20)')
				, @FileContext.value('(/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:DataContext/*:DataSourceCollection/*:DataSource/*:Type)[1]', 'varchar(20)')
				,@FileContext.value('(/*:UniversalInterchange/*:Body/*:Native/*:Header/*:DataContext/*:DataSourceCollection/*:DataSource/*:Type)[1]', 'varchar(20)'));

	-- If @XMLType is ForwardingConsol, get the ForwardingShipment node instead
	IF @XMLType = 'ForwardingConsol'
		BEGIN
			SET @XMLType = COALESCE(@FileContext.value('(/*:UniversalInterchange/*:Body/*:UniversalEvent/*:Event/*:DataContext/*:DataSourceCollection/*:DataSource/*:Type)[2]', 'varchar(20)')
					, @FileContext.value('(/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:DataContext/*:DataSourceCollection/*:DataSource/*:Type)[2]', 'varchar(20)')
					,@FileContext.value('(/*:UniversalInterchange/*:Body/*:Native/*:Header/*:DataContext/*:DataSourceCollection/*:DataSource/*:Type)[2]', 'varchar(20)'));

			SET @Key = COALESCE(@FileContext.value('(/*:UniversalInterchange/*:Body/*:UniversalEvent/*:Event/*:DataContext/*:DataSourceCollection/*:DataSource/*:Key)[2]', 'varchar(50)')
				, @FileContext.value('(/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:DataContext/*:DataSourceCollection/*:DataSource/*:Key)[2]', 'varchar(50)')
				, @FileContext.value('(/*:UniversalInterchange/*:Body/*:Native/*:Header/*:DataContext/*:DataSourceCollection/*:DataSource/*:Key)[2]', 'varchar(50)'));
		END
	ELSE
	BEGIN
		SET @Key = COALESCE(@FileContext.value('(/*:UniversalInterchange/*:Body/*:UniversalEvent/*:Event/*:DataContext/*:DataSourceCollection/*:DataSource/*:Key)[1]', 'varchar(50)')
				, @FileContext.value('(/*:UniversalInterchange/*:Body/*:UniversalShipment/*:Shipment/*:DataContext/*:DataSourceCollection/*:DataSource/*:Key)[1]', 'varchar(50)')
				, @FileContext.value('(/*:UniversalInterchange/*:Body/*:Native/*:Header/*:DataContext/*:DataSourceCollection/*:DataSource/*:Key)[1]', 'varchar(50)'));
	END;

	if @XMLType IS NULL  --we need to rais a error is the xml forat is not supported, to aler the caller.
		Begin
				RAISERROR ('Unsupported xml format received. Only UniversalEvent, UniversalShipment and Native layouts are supported.', 16, 1);  
			return
		End;

	if @Key IS NULL  --we need to rais a error is the xml forat is not supported, to aler the caller.
		Begin
			 RAISERROR ('Unsupported xml format received. Only UniversalEvent, UniversalShipment and Native layouts are supported.', 16, 1);  
			return
		End;

	IF @XMLType IS NOT NULL AND @Key IS NOT NULL
	BEGIN
		INSERT INTO [CargoWiseFile]
		(
			[XMLType],
			[Key],
			[FileContext],
			[CreatedDate],
			[SAPProcessed],
			[LandseaProcessed]
		)
		VALUES
		(
			@XMLType,
			@Key,
			@FileContext,
			GETDATE(),
			0,
			0
		)
	END;
END;