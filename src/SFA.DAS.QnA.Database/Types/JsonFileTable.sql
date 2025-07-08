CREATE TYPE [dbo].[JsonFileTable] AS TABLE
(
    RelativeFilePath NVARCHAR(500),
    JsonContent NVARCHAR(MAX)
);