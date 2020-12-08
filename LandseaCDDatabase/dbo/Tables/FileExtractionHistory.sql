CREATE TABLE [dbo].[FileExtractionHistory] (
    [CargoWiseFileID] INT          NOT NULL,
    [CargoWiseKey]    VARCHAR (50) NOT NULL,
    [ExtractionType]  CHAR (2)     NOT NULL,
    [ExtractionDate]  DATETIME     NOT NULL,
    CONSTRAINT [FK_FileExtractionHistory_CargoWiseFile] FOREIGN KEY ([CargoWiseFileID]) REFERENCES [dbo].[CargoWiseFile] ([ID]) NOT FOR REPLICATION
);


GO
CREATE NONCLUSTERED INDEX [IX_FileExtractionHistory_2]
    ON [dbo].[FileExtractionHistory]([ExtractionType] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FileExtractionHistory_1]
    ON [dbo].[FileExtractionHistory]([CargoWiseKey] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FileExtractionHistory]
    ON [dbo].[FileExtractionHistory]([CargoWiseFileID] ASC);

