CREATE TABLE [ApplicationSections]
(
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY DEFAULT NEWID(),
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[SequenceNo] [int] NOT NULL,
	[SectionNo] [int] NOT NULL,
	[QnAData] [nvarchar](max) NOT NULL,
	[Title] [nvarchar](250) NOT NULL,
	[LinkTitle] [nvarchar](250) NOT NULL,
	[DisplayType] [nvarchar](50) NOT NULL,
    [SequenceId] [uniqueidentifier] NOT NULL
) 
GO

CREATE INDEX [IX_ApplicationSections_ApplicationId] ON [ApplicationSections]  ( [ApplicationId] )
GO

CREATE INDEX [IX_ApplicationSections_BySequenceId] ON [ApplicationSections]   ( [ApplicationId], [SequenceId] )
GO

CREATE INDEX [IX_ApplicationSections_BySequenceNo] ON [ApplicationSections]   ( [ApplicationId], [SequenceNo] )
GO

CREATE INDEX [IX_ApplicationSections_BySequenceNoAndSectionNo] ON [ApplicationSections]   ( [ApplicationId], [SequenceNo], [SectionNo] )
GO