CREATE TABLE [dbo].[ApplicationSections](
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[SequenceNo] [int] NOT NULL,
	[SectionNo] [int] NOT NULL,
	[QnAData] [nvarchar](max) NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[LinkTitle] [nvarchar](50) NOT NULL,
	[DisplayType] [nvarchar](50) NOT NULL,
  [SequenceId] [uniqueidentifier] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ApplicationSections] ADD  CONSTRAINT [DF_ApplicationSequences_Id]  DEFAULT (newid()) FOR [Id]
GO




