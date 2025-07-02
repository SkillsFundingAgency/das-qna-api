CREATE TABLE [dbo].[ProjectsJsonFiles]
(
	[RelativeFilePath] VARCHAR(500) NOT NULL PRIMARY KEY,
	[JsonContent] NVARCHAR(MAX) NOT NULL
)
