-- use existing '$(ProjectPath) or create an empty variable
--:SETVAR ProjectPath ""

DECLARE @ProjectPath NVARCHAR(4000) = '$(ProjectPath)';
PRINT @ProjectPath

IF NULLIF(LTRIM(RTRIM(@ProjectPath)), '') IS NOT NULL
BEGIN
    PRINT 'ProjectPath is set. LoadProjectsFromBlobStorage ' + @ProjectPath
    EXEC dbo.LoadProjectsFromBlobStorage @ProjectPath;
END
ELSE
BEGIN
    PRINT 'ProjectPath not set. LoadProjectsFromBlobStorage should be called in a separate step after database deployment completes by pipeline.';
END
