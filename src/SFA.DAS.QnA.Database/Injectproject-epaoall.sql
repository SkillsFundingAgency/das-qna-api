-- Load a project 
-- needs one for every project setup. (or we need to build Dynamic SQL - one for another day)

DECLARE @ProjectExists INT,
		@ProjectName VARCHAR(100),
		@ProjectDesc VARCHAR(100) ,
		@ProjectId UNIQUEIDENTIFIER,
		@ApplicationDataSchema VARCHAR(MAX),
		@JSON VARCHAR(MAX);

DECLARE @Workflows VARCHAR(MAX),
		@WorkflowExists INT,
		@WorkflowId UNIQUEIDENTIFIER,
		@WorkFlowDescription VARCHAR(100),
		@WorkFlowVersion VARCHAR(100),
		@WorkFlowType VARCHAR(100);

DECLARE @sectionNo INT,
		@sequenceNo INT = 1,
		@sequenceExists INT,
		@sectionId UNIQUEIDENTIFIER;

DECLARE @SectionTitle VARCHAR(200),
		@SectionLinkTitle VARCHAR(200),
		@SectionDisplayType VARCHAR(200);

DECLARE @statement NVARCHAR(4000),
		@parameterDefinition NVARCHAR(4000),
		@path VARCHAR(MAX) = '$(ProjectPath)';

-- inject project
-- get project file
	SET @parameterDefinition = '@projectData VARCHAR(MAX) OUTPUT';
	SET @statement  = 'SELECT @projectData = BulkColumn
	FROM OPENROWSET 
	(BULK '''+@path+'\projects\epaoall\project.json'', SINGLE_CLOB) 
	AS project';
	
	EXEC sp_executesql @statement, @parameterDefinition,  @projectData = @JSON OUTPUT;
	
	SELECT @ProjectName = JSON_VALUE(@JSON,'$.Name'),  @ProjectDesc = JSON_VALUE(@JSON,'$.Description'), @Workflows = JSON_QUERY(@JSON,'$.Workflows[0]')

	SELECT @WorkflowDescription = JSON_VALUE(@Workflows,'$.Description'), @WorkflowVersion = JSON_VALUE(@Workflows,'$.Version'), @WorkflowType = JSON_VALUE(@Workflows,'$.Type')
/*	
BEGIN
	SELECT @ProjectExists = COUNT(*) 
	FROM projects WHERE Name = @ProjectName
	
	IF @ProjectExists = 0 
	BEGIN
	-- Need to create the "Project"
		SELECT @ApplicationDataSchema = BulkColumn
		FROM OPENROWSET 
		(BULK '$(System.DefaultWorkingDirectory)\projects\epaoall\ApplicationDataSchema.json', SINGLE_CLOB) 
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
			(BULK '$(System.DefaultWorkingDirectory)\projects\epaoall\sections\section1.json', SINGLE_CLOB) 
			AS qnaData;
		IF @sectionNo = 2 
			SELECT @JSON = BulkColumn
			FROM OPENROWSET 
			(BULK '$(System.DefaultWorkingDirectory)\projects\epaoall\sections\section2.json', SINGLE_CLOB) 
			AS qnaData;
		IF @sectionNo = 3 
			SELECT @JSON = BulkColumn
			FROM OPENROWSET 
			(BULK '$(System.DefaultWorkingDirectory)\projects\epaoall\sections\section3.json', SINGLE_CLOB) 
			AS qnaData;
		IF @sectionNo = 4 
			SELECT @JSON = BulkColumn
			FROM OPENROWSET 
			(BULK '$(System.DefaultWorkingDirectory)\projects\epaoall\sections\section4.json', SINGLE_CLOB) 
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
*/

GO

