CREATE TABLE [dbo].[ApplicationSequences](
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[SequenceNo] [int] NOT NULL,
	[IsActive] [bit] NOT NULL DEFAULT 0
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ApplicationSequences] ADD  CONSTRAINT [DF_ApplicationSequences_Id_1]  DEFAULT (newid()) FOR [Id]
GO


