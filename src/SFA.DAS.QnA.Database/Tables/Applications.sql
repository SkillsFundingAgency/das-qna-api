CREATE TABLE [Applications]
(
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY DEFAULT NEWID(),
	[WorkflowId] [uniqueidentifier] NOT NULL,
	[Reference] [nvarchar](256) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[ApplicationStatus] [nvarchar](20) NOT NULL,
	[ApplicationData] [nvarchar](max) NULL
 )
GO


