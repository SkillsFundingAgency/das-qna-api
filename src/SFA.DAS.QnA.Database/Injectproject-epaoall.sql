-- Load a project 
-- needs one for every project setup. (or we need to build Dynamic SQL - one for another day)
-- uses parameters
-- $(ProjectLocation) = "local" (Default) / "azure" 
-- If "azure" - then Database then External data Source / BlobStorage needs to exist
/*    if working locally you will need a storage account and set the values in the Publish profilecreate
		create using:
		-- Create a database master key if one does not already exist, using your own password. This key is used to encrypt the credential secret in next step.
		CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'MaryHadALittleLAmb';
		
		-- Create a database scoped credential with Azure storage account key as the secret.
		CREATE DATABASE SCOPED CREDENTIAL BlobCredential WITH IDENTITY = 'SHARED ACCESS SIGNATURE', SECRET = 'shared access secret';

		-- Create an external data source with CREDENTIAL option.
		CREATE EXTERNAL DATA SOURCE BlobStorage WITH (LOCATION = '<ProjectPath>',CREDENTIAL = BlobCredential,TYPE = BLOB_STORAGE);
*/
-- $(ProjectPath) = Directory Path for local / Location for Blob Storage


DECLARE @ProjectLocation VARCHAR(100) = '$(ProjectLocation)';
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

DECLARE @sectionNo INT;
DECLARE @sequenceNo INT = 1;
DECLARE @sequenceExists INT;
DECLARE @sectionId UNIQUEIDENTIFIER;

DECLARE @SectionTitle VARCHAR(200);
DECLARE @SectionLinkTitle VARCHAR(200);
DECLARE @SectionDisplayType VARCHAR(200);

DECLARE @LoadBLOB BIT = 0;  -- assume local - set to 1 if $(ProjectLocation) = "azure"

-- START
BEGIN
	IF @ProjectLocation = 'azure'
	BEGIN
		SET @LoadBLOB = 1;
		PRINT 'Loading from BLOB Storage';
	END

	-- inject project

	-- get project file
	IF @LoadBLOB = 1
		SELECT @JSON = BulkColumn
		FROM OPENROWSET
		(BULK 'projects/epaoall/project.json', DATA_SOURCE = 'BlobStorage', SINGLE_CLOB) 
		AS project;
	ELSE
		SELECT @JSON = BulkColumn
		FROM OPENROWSET 
		(BULK '$(ProjectPath)\projects\epaoall\project.json', SINGLE_CLOB) 
		AS project;

	-- extract project
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
			SELECT @ApplicationDataSchema = BulkColumn
			FROM OPENROWSET
			(BULK 'projects/epaoall/ApplicationDataSchema.json', DATA_SOURCE = 'BlobStorage', SINGLE_CLOB) 
			AS ad;
		ELSE
			SELECT @ApplicationDataSchema = BulkColumn
			FROM OPENROWSET 
			(BULK '$(ProjectPath)\projects\epaoall\ApplicationDataSchema.json', SINGLE_CLOB) 
			AS ad;
		
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

	SET @sectionNo = 1
	WHILE @sectionNo <= 4
	BEGIN
		IF @sectionNo = 4
			SET @sequenceNo = 2;

		PRINT 'Configure Sequence '+CONVERT(char,@sequenceNo)+' Section '+CONVERT(char,@sectionNo);

		SELECT @sequenceExists = COUNT(*) 
		FROM [WorkflowSequences]
		WHERE [WorkflowId] = @WorkflowId AND [SequenceNo] = @sequenceNo AND [SectionNo] = @sectionNo;

		IF @sequenceExists = 0
		BEGIN
			PRINT 'Insert Workflow for Sequence '+CONVERT(char,@sequenceNo)+' Section '+CONVERT(char,@sectionNo);
			INSERT INTO [WorkflowSequences] (Workflowid, SequenceNo, SectionNo, SectionId, IsActive)
			VALUES ( @WorkflowId, @sequenceNo, @sectionNo, NEWID(), 1);
		END
		
		-- get section id 
		SELECT @sectionId = [SectionId] 
		FROM [WorkflowSequences]
		WHERE [WorkflowId] = @WorkflowId AND [SequenceNo] = @sequenceNo AND [SectionNo] = @sectionNo;
	  
		If @sectionNo = 1 
		BEGIN
			PRINT 'Load Section '+CONVERT(char,@sectionNo);
			IF @LoadBLOB = 1
				SELECT @JSON = BulkColumn
				FROM OPENROWSET
				(BULK 'projects/epaoall/sections/section1.json', DATA_SOURCE = 'BlobStorage', SINGLE_CLOB) 
				AS qnaData;
			ELSE
				SELECT @JSON = BulkColumn
				FROM OPENROWSET 
				(BULK '$(ProjectPath)\projects\epaoall\sections\section1.json', SINGLE_CLOB) 
				AS qnaData;
		END
		IF @sectionNo = 2 
		BEGIN
			PRINT 'Load Section '+CONVERT(char,@sectionNo);
			IF @LoadBLOB = 1
				SELECT @JSON = BulkColumn
				FROM OPENROWSET
				(BULK 'projects/epaoall/sections/section2.json', DATA_SOURCE = 'BlobStorage', SINGLE_CLOB) 
				AS qnaData;
			ELSE
				SELECT @JSON = BulkColumn
				FROM OPENROWSET 
				(BULK '$(ProjectPath)\projects\epaoall\sections\section2.json', SINGLE_CLOB) 
				AS qnaData;
		END
		IF @sectionNo = 3 
		BEGIN
			PRINT 'Load Section '+CONVERT(char,@sectionNo);
			IF @LoadBLOB = 1
				SELECT @JSON = BulkColumn
				FROM OPENROWSET
				(BULK 'projects/epaoall/sections/section3.json', DATA_SOURCE = 'BlobStorage', SINGLE_CLOB) 
				AS qnaData;
			ELSE
				SELECT @JSON = BulkColumn
				FROM OPENROWSET 
				(BULK '$(ProjectPath)\projects\epaoall\sections\section3.json', SINGLE_CLOB) 
				AS qnaData;
		END
		IF @sectionNo = 4 
		BEGIN
			PRINT 'Load Section '+CONVERT(char,@sectionNo);
			IF @LoadBLOB = 1
				SELECT @JSON = BulkColumn
				FROM OPENROWSET
				(BULK 'projects/epaoall/sections/section4.json', DATA_SOURCE = 'BlobStorage', SINGLE_CLOB) 
				AS qnaData;
			ELSE
				SELECT @JSON = BulkColumn
				FROM OPENROWSET 
				(BULK '$(ProjectPath)\projects\epaoall\sections\section4.json', SINGLE_CLOB) 
				AS qnaData;
		END
		
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

		SET @sectionNo = @sectionNo + 1;
	END

-- tidyup
	IF @WorkflowExists = 0 
	-- have created a new workflow
	BEGIN
		UPDATE Workflows SET Status = 'Dead' WHERE [WorkflowId] != @WorkflowId
		UPDATE Workflows SET Status = 'Live' WHERE [WorkflowId] = @WorkflowId
	END

END


GO

