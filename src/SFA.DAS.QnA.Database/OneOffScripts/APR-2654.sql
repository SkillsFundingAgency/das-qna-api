-- APR-2654 - Increase website domain length from upto 5 to upto 13 in Question YO-41
-- WORKFLOW: ROATP
-- DATE: 2021-08-10

UPDATE appSection
SET appSection.QnAData = REPLACE(appSection.QnAData, '.[a-z]{2,5}(:[0-9]{1,5})', '.[a-z]{2,13}(:[0-9]{1,5})')
FROM ApplicationSections appSection
INNER JOIN Applications app ON app.Id = appSection.ApplicationId
INNER JOIN Workflows wf ON wf.Id = app.WorkflowId
WHERE appSection.SequenceNo = 1 AND appSection.SectionNo = 2 AND wf.Type = 'roatp';
