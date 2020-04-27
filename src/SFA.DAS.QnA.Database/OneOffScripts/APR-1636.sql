-- Fix for APR-1636 - Part of a group of companies page is not being deactivated upon changing the flow
-- Date: 2020-04-27

-- Page 10
  UPDATE [ApplicationSections]
  SET QnAData = REPLACE(QnAData,
                        '"AllowMultipleAnswers":false,"Active":false,"NotRequiredConditions":[{"Field":"OrganisationType","IsOneOf":["HEI"]}],"BodyText":"","ActivatedByPageId":"9"',
                        '"AllowMultipleAnswers":false,"Active":false,"NotRequiredConditions":[{"Field":"OrganisationType","IsOneOf":["HEI"]}],"BodyText":"","ActivatedByPageId":"8,9"')
  WHERE SequenceNo = 1 AND SectionNo = 1

  UPDATE [ApplicationSections]
  SET QnAData = REPLACE(QnAData,
                        '"AllowMultipleAnswers":false,"Active":true,"NotRequiredConditions":[{"Field":"OrganisationType","IsOneOf":["HEI"]}],"BodyText":"","ActivatedByPageId":"9"',
                        '"AllowMultipleAnswers":false,"Active":true,"NotRequiredConditions":[{"Field":"OrganisationType","IsOneOf":["HEI"]}],"BodyText":"","ActivatedByPageId":"8,9"')
  WHERE SequenceNo = 1 AND SectionNo = 1