CREATE PROCEDURE [dbo].[LoadProjectsJsonFiles]
    @Files [dbo].[JsonFileTable] READONLY
AS
BEGIN
    SET NOCOUNT ON;

    TRUNCATE TABLE [dbo].[ProjectsJsonFiles];

    DECLARE @RelativeFilePath NVARCHAR(500),
            @JsonContent NVARCHAR(MAX)

    DECLARE FilesCur CURSOR FOR
        SELECT RelativeFilePath, JsonContent FROM @Files;

    OPEN FilesCur;
    FETCH NEXT FROM FilesCur INTO @RelativeFilePath, @JsonContent;

    WHILE @@FETCH_STATUS = 0
    BEGIN      
        INSERT INTO ProjectsJsonFiles (RelativeFilePath, JsonContent)
        VALUES (@RelativeFilePath, @JsonContent);
        
        FETCH NEXT FROM FilesCur INTO @RelativeFilePath, @JsonContent;
    END

    CLOSE FilesCur;
    DEALLOCATE FilesCur;
END
