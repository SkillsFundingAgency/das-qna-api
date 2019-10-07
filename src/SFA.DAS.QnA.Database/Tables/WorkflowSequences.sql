CREATE TABLE [dbo].[WorkflowSequences](
                                          [Id] [uniqueidentifier] NOT NULL,
                                          [WorkflowId] [uniqueidentifier] NOT NULL,
                                          [SequenceNo] [int] NOT NULL,
                                          [SectionNo] [int] NOT NULL,
                                          [SectionId] [uniqueidentifier] NOT NULL,
                                          [Status] [nvarchar](50) NOT NULL,
                                          [IsActive] [bit] NOT NULL DEFAULT 0
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WorkflowSequences] ADD  CONSTRAINT [DF_WorkflowSequences_Id]  DEFAULT (newid()) FOR [Id]
GO

