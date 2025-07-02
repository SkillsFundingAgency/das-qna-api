-- The $(ProjectPath) variable should be set in the Publish Database dialog if running the DACPAC from Visual Studio publish command
-- e.g. C:\Source\Repos\SFA\das-qna-api\src\SFA.DAS.QnA.Database\

DECLARE @ProjectPath NVARCHAR(4000) = '$(ProjectPath)';

IF NULLIF(LTRIM(RTRIM(@ProjectPath)), '') IS NOT NULL
BEGIN
    PRINT 'ProjectPath set. LoadProjectsFromStorage ' + @ProjectPath
    EXEC dbo.LoadProjectsFromStorage @ProjectPath;
END
ELSE
BEGIN
    PRINT 'ProjectPath not set. LoadProjectsFromStorage should be called in a separate step after database deployment completes by pipeline.';
END
