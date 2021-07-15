-- APR-2563 - Fix QuestionId format in Page 2150 for FHA question 320
-- WORKFLOW: ROATP
-- DATE: 2021-07-15

UPDATE appSection
SET appSection.QnAData = REPLACE(appSection.QnAData, 'FHA-320, 'FH-320')
FROM ApplicationSections appSection
INNER JOIN Applications app ON app.Id = appSection.ApplicationId
INNER JOIN Workflows wf ON wf.Id = app.WorkflowId
WHERE appSection.SequenceNo = 2 AND appSection.SectionNo = 2 AND wf.Type = 'roatp';