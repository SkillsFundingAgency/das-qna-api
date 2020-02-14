CREATE TABLE [ApplicationSequences]
(
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY DEFAULT NEWID(),
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[SequenceNo] [int] NOT NULL,
	[IsActive] [bit] NOT NULL DEFAULT 0
) 
GO

CREATE  INDEX [IX_ApplicationSequences_ApplicationId] ON [ApplicationSequences]   ( [ApplicationId] )
GO

CREATE INDEX [IX_ApplicationSequences_BySequenceNo] ON [ApplicationSections]   ( [ApplicationId], [SequenceNo]  )
GO
