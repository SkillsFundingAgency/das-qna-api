CREATE TABLE [ApplicationSequences]
(
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[SequenceNo] [int] NOT NULL,
	[IsActive] [bit] NOT NULL DEFAULT 0,
     CONSTRAINT [PK_ApplicationSequences] PRIMARY KEY (	[Id] )
) 
GO

ALTER TABLE [ApplicationSequences] ADD  CONSTRAINT [DF_ApplicationSequences_Id_1]  DEFAULT (newid()) FOR [Id]
GO

CREATE  INDEX [IX_ApplicationSequences_ApplicationId] ON [ApplicationSequences]   (  [ApplicationId]  )
GO
