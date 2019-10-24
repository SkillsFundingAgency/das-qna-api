-- Load a project 
-- needs one for every project setup. (or we need to build Dynamic SQL - one for another day)
-- uses parameters
-- If using BLOB storage then the Database will need an External Data Source 'BlobStorage' needs to exist
/*    if working locally you will need a storage account and set the values in the Publish profilecreate
		create using:
		-- Create a database master key if one does not already exist, using your own password. This key is used to encrypt the credential secret in next step.
		CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'MaryHadALittleLAmb';
		
		-- Create a database scoped credential with Azure storage account key as the secret.
		CREATE DATABASE SCOPED CREDENTIAL BlobCredential WITH IDENTITY = 'SHARED ACCESS SIGNATURE', SECRET = 'shared access secret';

		-- Create an external data source with CREDENTIAL option.
		CREATE EXTERNAL DATA SOURCE BlobStorage WITH (LOCATION = '<ProjectPath>',CREDENTIAL = BlobCredential,TYPE = BLOB_STORAGE);
*/
-- $(ProjectPath) = Directory Path for local or BLOB


DECLARE @ProjectLocation VARCHAR(100) = '$(ProjectPath)';
DECLARE @ProjectExists INT;
DECLARE @ProjectName VARCHAR(100);
DECLARE @ProjectDesc VARCHAR(100) ;
DECLARE @ProjectId UNIQUEIDENTIFIER;
DECLARE @ApplicationDataSchema VARCHAR(MAX);
DECLARE @JSON VARCHAR(MAX);

DECLARE @Workflows VARCHAR(MAX);
DECLARE @WorkflowExists INT;
DECLARE @WorkflowId UNIQUEIDENTIFIER;
DECLARE @WorkFlowDescription VARCHAR(100);
DECLARE @WorkFlowVersion VARCHAR(100);
DECLARE @WorkFlowType VARCHAR(100);

DECLARE @sectionIndex INT;
DECLARE @sectionNo INT;
DECLARE @sequenceNo INT = 1;
DECLARE @sequenceExists INT;
DECLARE @sectionId UNIQUEIDENTIFIER;
DECLARE @sections VARCHAR(MAX);

DECLARE @SectionTitle VARCHAR(200);
DECLARE @SectionLinkTitle VARCHAR(200);
DECLARE @SectionDisplayType VARCHAR(200);

DECLARE @LoadBLOB BIT = 0;  -- assume local - set to 1 if $(ProjectPath) starts with http

DECLARE @SQLString NVARCHAR(4000);  
DECLARE @ParmDefinition NVARCHAR(500);


-- START
BEGIN
	IF SUBSTRING(@ProjectLocation,1,4) = 'http'
	BEGIN
		SET @LoadBLOB = 1;
		PRINT 'Loading from BLOB Storage';
	END

	-- inject project

	-- get project file
	IF @LoadBLOB = 1
		SET @SQLString = 'SELECT @project = BulkColumn
		FROM OPENROWSET
		(BULK ''projects/epaoall/project.json'', DATA_SOURCE = ''BlobStorage'', SINGLE_CLOB) 
		AS project';
	ELSE
		SET @SQLString = 'SELECT @project = BulkColumn
		FROM OPENROWSET
		(BULK ''$(ProjectPath)\projects\epaoall\project.json'', SINGLE_CLOB) 
		AS project';
		
	SET @ParmDefinition = '@project VARCHAR(MAX) OUTPUT';
	EXECUTE sp_executesql @SQLString, @ParmDefinition, @project = @JSON OUTPUT;

	-- extract project and workflow  (Note currently only handling ONE Workflow per project)
	SELECT @ProjectName = JSON_VALUE(@JSON,'$.Name'),  @ProjectDesc = JSON_VALUE(@JSON,'$.Description'), @Workflows = JSON_QUERY(@JSON,'$.Workflows[0]')
	-- extract workflow(s)
	SELECT @WorkflowDescription = JSON_VALUE(@Workflows,'$.Description'), @WorkflowVersion = JSON_VALUE(@Workflows,'$.Version'), @WorkflowType = JSON_VALUE(@Workflows,'$.Type')

	-- check if project exists
	SELECT @ProjectExists = COUNT(*) FROM projects WHERE Name = @ProjectName

	IF @ProjectExists = 0 
	BEGIN
	-- Need to create the "Project"
	-- Get the ApplicationDataSchema
		IF @LoadBLOB = 1
			SET @SQLString = 'SELECT @ad = BulkColumn
			FROM OPENROWSET
			(BULK ''projects/epaoall/ApplicationDataSchema.json'', DATA_SOURCE = ''BlobStorage'', SINGLE_CLOB) 
			AS ad';
		ELSE
			SET @SQLString = 'SELECT @ad = BulkColumn
			FROM OPENROWSET
			(BULK ''$(ProjectPath)\projects\epaoall\ApplicationDataSchema.json'', SINGLE_CLOB) 
			AS ad';
		
		SET @ParmDefinition = '@ad VARCHAR(MAX) OUTPUT';
		EXECUTE sp_executesql @SQLString, @ParmDefinition, @ad = @ApplicationDataSchema OUTPUT;
		
		INSERT INTO projects (Name, Description, ApplicationDataSchema, CreatedAt, CreatedBy)
			VALUES (@ProjectName, @ProjectDesc, @ApplicationDataSchema, GETUTCDATE(), 'Deployment');
	END
	-- get project id (back)
	SELECT @ProjectId = Id FROM projects WHERE Name = @ProjectName


	SELECT @WorkflowExists = COUNT(*) 
	FROM [Workflows]
	WHERE ProjectId = @ProjectId 
	  AND [Description] = @WorkFlowDescription
	  AND [Version] = @WorkFlowVersion;
	  
	IF @WorkflowExists = 0 
	BEGIN	  
	-- Need to create the "Workflow"
	
		INSERT INTO [Workflows] ([ProjectId], [Description], [Version], [Type], [Status], [CreatedAt] ,[CreatedBy], [ApplicationDataSchema])
		SELECT p1.Id ProjectId, @WorkFlowDescription, @WorkFlowVersion, @WorkFlowType, 'Draft', [CreatedAt] ,[CreatedBy], [ApplicationDataSchema]
		FROM projects p1
		WHERE Name = @ProjectName;

	END
	
	-- get workflow id (back)
	SELECT @WorkflowId = Id 
	 FROM [Workflows]
	WHERE ProjectId = @ProjectId 
	  AND [Description] = @WorkFlowDescription
	  AND [Version] = @WorkFlowVersion

	-- load the Sequences and Sections
	SET @sectionIndex = 0;
	-- loop  thorugh the sections
	WHILE @sectionIndex >= 0
	BEGIN
	-- get the first/next section from workflow.
		SELECT @sections = JSON_QUERY(@Workflows,'$.section['+RTRIM(convert(char,@sectionIndex))+']');
		
		IF @sections IS NULL
			BREAK;
			
		SELECT @sectionNo = JSON_VALUE(@sections ,'$.SectionNo'), @sequenceNo = JSON_VALUE(@sections ,'$.SequenceNo');

		PRINT 'Configure Sequence '+RTRIM(CONVERT(char,@sequenceNo))+' Section '+RTRIM(CONVERT(char,@sectionNo));

		SELECT @sequenceExists = COUNT(*) 
		FROM [WorkflowSequences]
		WHERE [WorkflowId] = @WorkflowId AND [SequenceNo] = @sequenceNo AND [SectionNo] = @sectionNo;

		IF @sequenceExists = 0
		BEGIN
			PRINT 'Insert Workflow for Sequence '+RTRIM(CONVERT(char,@sequenceNo))+' Section '+RTRIM(CONVERT(char,@sectionNo));
			INSERT INTO [WorkflowSequences] (Workflowid, SequenceNo, SectionNo, SectionId, IsActive)
			VALUES ( @WorkflowId, @sequenceNo, @sectionNo, NEWID(), 1);
		END
		
		-- get section id 
		SELECT @sectionId = [SectionId] 
		FROM [WorkflowSequences]
		WHERE [WorkflowId] = @WorkflowId AND [SequenceNo] = @sequenceNo AND [SectionNo] = @sectionNo;

		PRINT 'Load Sequence '+RTRIM(CONVERT(char,@sequenceNo))+' Section '+RTRIM(CONVERT(char,@sectionNo));
		IF @LoadBLOB = 1
			SET @SQLString = 'SELECT @qnaData = BulkColumn
			FROM OPENROWSET
			(BULK ''projects/epaoall/sections/section'+RTRIM(CONVERT(char,@sectionNo))+'.json'', DATA_SOURCE = ''BlobStorage'', SINGLE_CLOB) 
			AS qnaData';
		ELSE
			SET @SQLString = 'SELECT @qnaData = BulkColumn
			FROM OPENROWSET
			(BULK ''$(ProjectPath)\projects\epaoall\sections\section'+RTRIM(CONVERT(char,@sectionNo))+'.json'', SINGLE_CLOB) 
			AS qnaData';

		SET @ParmDefinition = '@qnaData VARCHAR(MAX) OUTPUT';
		EXECUTE sp_executesql @SQLString, @ParmDefinition, @qnaData = @JSON OUTPUT;
		
		-- get the Section details
		SELECT @SectionTitle = JSON_VALUE(@JSON,'$.Title'),  @SectionLinkTitle = JSON_VALUE(@JSON,'$.LinkTitle'), @SectionDisplayType = JSON_VALUE(@JSON,'$.DisplayType')
		
		MERGE INTO [Workflowsections] ws1
		USING (SELECT @sectionId sectionId) upd
		ON (ws1.Id = upd.sectionId)
		WHEN MATCHED THEN 
			UPDATE SET QnAData = @JSON, [Title] = @SectionTitle, [LinkTitle] = @SectionLinkTitle, [DisplayType] = @SectionDisplayType
		WHEN NOT MATCHED THEN
			INSERT (Id, ProjectId, QnAData, Title, LinkTitle, DisplayType)
			VALUES (@sectionId, @ProjectId, @JSON, @SectionTitle, @SectionLinkTitle, @SectionDisplayType);

		SET @sectionIndex = @sectionIndex + 1;
	END

-- tidyup
	IF @WorkflowExists = 0 
	-- have created a new workflow
	BEGIN
		UPDATE Workflows SET Status = 'Dead' WHERE [Id] != @WorkflowId AND [Type] = @WorkflowType AND [ProjectId] = @ProjectId
		UPDATE Workflows SET Status = 'Live' WHERE [Id] = @WorkflowId
	END

END


GO

