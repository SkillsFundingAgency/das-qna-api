-- APR-2511 Change content to 'Full Time Equivalent (FTE) employees for the last financial year'
-- WORKFLOW: ROATP
-- DATE: 2021-05-21

UPDATE appSection
SET appSection.QnAData = REPLACE(appSection.QnAData, 'Full Time Equivalent Employees (FTE) for the last financial year.', 'Full Time Equivalent (FTE) employees for the last financial year')
FROM ApplicationSections appSection
INNER JOIN Applications app ON app.Id = appSection.ApplicationId
INNER JOIN Workflows wf ON wf.Id = app.WorkflowId
WHERE appSection.SequenceNo = 2 AND appSection.SectionNo = 2 AND wf.Type = 'roatp'