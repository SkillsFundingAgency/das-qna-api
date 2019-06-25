CREATE TABLE [dbo].[Applications](
	[Id] [uniqueidentifier] NOT NULL,
	[WorkflowId] [uniqueidentifier] NOT NULL,
	[Reference] [nvarchar](256) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[ApplicationStatus] [nvarchar](20) NOT NULL,
	[ApplicationData] [nvarchar](max) NULL
 CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Applications] ADD  CONSTRAINT [Applications_Id]  DEFAULT (newid()) FOR [Id]
GO


