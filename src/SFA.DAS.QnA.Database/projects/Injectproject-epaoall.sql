-- Load a project 
-- needs one for every project setup. (or we need to build Dynamic SQL - one for another day)

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


-- inject project
-- get project file
	SELECT @JSON = BulkColumn
	FROM OPENROWSET 
	(BULK '.\projects\epaoall\project-1.json', SINGLE_CLOB) 
	AS project;
	
	SELECT @ProjectName = JSON_VALUE(@JSON,'$.Name'),  @ProjectDesc = JSON_VALUE(@JSON,'$.Description'), @Workflows = JSON_QUERY(@JSON,'$.Workflows[0]')

	SELECT @WorkflowDescription = JSON_VALUE(@Workflows,'$.Description'), @WorkflowVersion = JSON_VALUE(@Workflows,'$.Version'), @WorkflowType = JSON_VALUE(@Workflows,'$.Type')
	
BEGIN
	SELECT @ProjectExists = COUNT(*) 
	FROM projects WHERE Name = @ProjectName
	
	IF @ProjectExists = 0 
	BEGIN
	-- Need to create the "Project"
		SELECT @ApplicationDataSchema = BulkColumn
		FROM OPENROWSET 
		(BULK '.\ApplicationDataSchema\Assessor.json', SINGLE_CLOB) 
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
		WHERE [WorkflowId] = WorkflowId AND [SequenceNo] = @sequenceNo AND [SectionNo] = @sectionNo;

		IF @sequenceExists = 0
			INSERT INTO [WorkflowSequences] (Workflowid, SequenceNo, SectionNo, SectionId, IsActive)
			VALUES ( @WorkflowId, @sequenceNo, @sectionNo, NEWID(), 1);
		
		-- get section id 
		SELECT @sectionId = [SectionId] 
		FROM [WorkflowSequences]
		WHERE [WorkflowId] = WorkflowId AND [SequenceNo] = @sequenceNo AND [SectionNo] = @sectionNo;
	  
		If @sectionNo = 1 
			SELECT @JSON = BulkColumn
			FROM OPENROWSET 
			(BULK '.\projects\epaoall\sections\section1.json', SINGLE_CLOB) 
			AS qnaData;
		IF @sectionNo = 2 
			SELECT @JSON = BulkColumn
			FROM OPENROWSET 
			(BULK '.\projects\epaoall\sections\section2.json', SINGLE_CLOB) 
			AS qnaData;
		IF @sectionNo = 3 
			SELECT @JSON = BulkColumn
			FROM OPENROWSET 
			(BULK '.\projects\epaoall\sections\section3.json', SINGLE_CLOB) 
			AS qnaData;
		IF @sectionNo = 4 
			SELECT @JSON = BulkColumn
			FROM OPENROWSET 
			(BULK '.\projects\epaoall\sections\section4.json', SINGLE_CLOB) 
			AS qnaData;
		
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

END


GO

