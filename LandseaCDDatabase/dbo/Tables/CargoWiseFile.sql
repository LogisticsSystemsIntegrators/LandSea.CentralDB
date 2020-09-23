CREATE TABLE [dbo].[CargoWiseFile] (
    [ID]               INT      IDENTITY (1, 1) NOT NULL,
    [FileContext]      XML      NOT NULL,
    [CreatedDate]      DATETIME NOT NULL,
    [SAPProcessed]     BIT      NOT NULL,
    [SAPProcessedDate] DATETIME NULL,
    CONSTRAINT [pk_CargoWiseFile_ID] PRIMARY KEY CLUSTERED ([ID] ASC)
);

