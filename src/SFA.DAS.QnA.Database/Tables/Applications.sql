CREATE TABLE [Applications]
(
	[Id] [uniqueidentifier] NOT NULL,
	[WorkflowId] [uniqueidentifier] NOT NULL,
	[Reference] [nvarchar](256) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[ApplicationStatus] [nvarchar](20) NOT NULL,
	[ApplicationData] [nvarchar](max) NULL
 CONSTRAINT [PK_Applications] PRIMARY KEY ( [Id]  )
 )
GO

ALTER TABLE [Applications] ADD  CONSTRAINT [Applications_Id]  DEFAULT (newid()) FOR [Id]
GO


