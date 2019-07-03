CREATE TABLE [dbo].[Projects]
(
    [Id]                    [uniqueidentifier] NOT NULL,
    [Name]                  [nvarchar](250)    NOT NULL,
    [Description]           [nvarchar](max)    NOT NULL,
    [ApplicationDataSchema] [nvarchar](max)    NOT NULL,
    [CreatedAt]             [datetime2](7)     NOT NULL,
    [CreatedBy]             [nvarchar](250)    NOT NULL,
    CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED
        (
         [Id] ASC
            ) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Projects]
    ADD CONSTRAINT [DF_Projects_Id] DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[Projects]
    ADD CONSTRAINT [DF_Projects_Description] DEFAULT ('') FOR [Description]
GO

ALTER TABLE [dbo].[Projects]
    ADD CONSTRAINT [DF_Projects_CreatedAt] DEFAULT (getutcdate()) FOR [CreatedAt]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Projects_Name] ON [dbo].[Projects]
    (
     [Name] ASC
        ) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO