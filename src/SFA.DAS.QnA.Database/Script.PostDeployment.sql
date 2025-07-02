/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

-- Deploy QnA Workflow Definitions for all projects when publishing the database locally from Visual Studio the $(ProjectPath) variable 
-- must be set in the Publish Database dialog e.g. C:\LocalPathToRepos\das-qna-api\src\SFA.DAS.QnA.Database\

DECLARE @ProjectPath NVARCHAR(4000) = '$(ProjectPath)';
DECLARE @JsonFiles dbo.JsonFileTable;

IF NULLIF(LTRIM(RTRIM(@ProjectPath)), '') IS NOT NULL
BEGIN
    PRINT 'Loading projects during DACPAC publish from ' + @ProjectPath
    EXEC dbo.LoadProjects @ProjectPath, @JsonFiles;
END
ELSE
BEGIN
    PRINT 'Loading projects will be completed after DACPAC publish by a pipeline task';
END
