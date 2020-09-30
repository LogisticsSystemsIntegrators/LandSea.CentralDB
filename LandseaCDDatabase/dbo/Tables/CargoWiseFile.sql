﻿CREATE TABLE [dbo].[CargoWiseFile] (
    [ID] INT IDENTITY (1, 1) NOT NULL,
	[XMLType] VARCHAR(20) NOT NULL,
	[Key] VARCHAR(50) NOT NULL,
    [FileContext] XML NOT NULL,
    [CreatedDate] DATETIME NOT NULL,
    [SAPProcessed] BIT NOT NULL,
    [SAPProcessedDate] DATETIME NULL,
	[LandseaProcessed] BIT NOT NULL,
	[LandseaProcessedDate] DATETIME NULL,
    CONSTRAINT [pk_CargoWiseFile_ID] PRIMARY KEY CLUSTERED ([ID] ASC)
);

