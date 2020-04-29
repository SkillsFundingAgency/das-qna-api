-- Fix for APR-1583 - Some FHA Questions are not set to Active upon branching from FileUpload question
-- Date: 2020-04-16

-- Page 2200
  UPDATE [ApplicationSections]
  SET QnAData = REPLACE(QnAData,
                        '"Complete":true,"AllowMultipleAnswers":false,"Active":false,"NotRequiredConditions":[{"Field":"FHAHas12MonthsStatements","IsOneOf":["Yes"]},{"Field":"FHAPartialFinancialStatements","IsOneOf":["No"]}],"BodyText":"","Details":null,"DisplayType":null,"Feedback":null,"ActivatedByPageId":"2180"',
                        '"Complete":true,"AllowMultipleAnswers":false,"Active":true,"NotRequiredConditions":[{"Field":"FHAHas12MonthsStatements","IsOneOf":["Yes"]},{"Field":"FHAPartialFinancialStatements","IsOneOf":["No"]}],"BodyText":"","Details":null,"DisplayType":null,"Feedback":null,"ActivatedByPageId":"2180"')
  WHERE SequenceNo = 2 AND SectionNo = 2

-- Page 2260
  UPDATE [ApplicationSections]
  SET QnAData = REPLACE(QnAData,
                        '"Complete":true,"AllowMultipleAnswers":false,"Active":false,"NotRequiredConditions":[],"BodyText":"","ActivatedByPageId":"2160,2200,2220,2250"',
                        '"Complete":true,"AllowMultipleAnswers":false,"Active":true,"NotRequiredConditions":[],"BodyText":"","ActivatedByPageId":"2160,2200,2220,2250"')
  WHERE SequenceNo = 2 AND SectionNo = 2