-- The $(ProjectPath) variable should be set in the Publish Database dialog if running the DACPAC from Visual Studio publish command
-- e.g. C:\Source\Repos\SFA\das-qna-api\src\SFA.DAS.QnA.Database\

DECLARE @ProjectPath NVARCHAR(4000) = '$(ProjectPath)';

IF NULLIF(LTRIM(RTRIM(@ProjectPath)), '') IS NOT NULL
BEGIN
    PRINT 'Loading projects during DACPAC publish from ' + @ProjectPath
    EXEC dbo.LoadProjects @ProjectPath, NULL;
END
ELSE
BEGIN
    PRINT 'Loading projects will be completed after DACPAC publish by a pipeline task';
END
