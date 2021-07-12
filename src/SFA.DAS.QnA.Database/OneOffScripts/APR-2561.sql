-- APR-2561 - Add NotRequiredConditions for employer provider only questions
-- WORKFLOW: ROATP
-- DATE: 2021-06-17

UPDATE appSection
SET appSection.QnAData = REPLACE(appSection.QnAData,
							'"BodyText":"","ActivatedByPageId":"6400"},{"PageId":"6420",',
							'"BodyText":"","ActivatedByPageId":"6400","NotRequiredConditions":[{"Field":"ApplyProviderRoute","IsOneOf":["1","3"]}]},{"PageId":"6420",')
FROM ApplicationSections appSection
INNER JOIN Applications app ON app.Id = appSection.ApplicationId
INNER JOIN Workflows wf ON wf.Id = app.WorkflowId
WHERE appSection.SequenceNo = 6 AND appSection.SectionNo = 4 AND wf.Type = 'roatp';


UPDATE appSection
SET appSection.QnAData = REPLACE(appSection.QnAData,
							'"NotRequiredConditions":[],"BodyText":"","ActivatedByPageId":"6410"}]}',
							'"NotRequiredConditions":[{"Field":"ApplyProviderRoute","IsOneOf":["1","3"]}],"BodyText":"","ActivatedByPageId":"6410"}]}')
FROM ApplicationSections appSection
INNER JOIN Applications app ON app.Id = appSection.ApplicationId
INNER JOIN Workflows wf ON wf.Id = app.WorkflowId
WHERE appSection.SequenceNo = 6 AND appSection.SectionNo = 4 AND wf.Type = 'roatp';