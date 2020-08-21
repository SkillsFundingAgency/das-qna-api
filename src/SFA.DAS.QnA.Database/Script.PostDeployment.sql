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
:r .\OneOffScripts\APR-1583.sql
:r .\OneOffScripts\APR-1650.sql
:r .\OneOffScripts\APR-1779.sql
-- END