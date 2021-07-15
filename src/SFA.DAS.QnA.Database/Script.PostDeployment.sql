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

-- deploy QnA Workflow Definitions for all projects
:r .\projects\Inject-projects.sql

-- Deploy any One Off Scripts
-- NOTE: Remember to make these idempotent and remove when not required any more
:r .\OneOffScripts\APR-2563.sql
-- END


-- Finally rebuild indexes on all tables, helps keep fragmentation down
ALTER INDEX ALL ON [Projects] REBUILD;
ALTER INDEX ALL ON [Workflows] REBUILD;
ALTER INDEX ALL ON [WorkflowSequences] REBUILD;
ALTER INDEX ALL ON [WorkflowSections] REBUILD;
ALTER INDEX ALL ON [Applications] REBUILD;
ALTER INDEX ALL ON [ApplicationSequences] REBUILD;
ALTER INDEX ALL ON [ApplicationSections] REBUILD;