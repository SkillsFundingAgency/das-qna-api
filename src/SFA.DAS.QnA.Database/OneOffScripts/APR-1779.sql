-- Fix for APR-1779 - Remove conditions that would move onto Course Directory Question (Page 6700)
-- Date: 2020-05-27

-- Page 6800
  UPDATE [ApplicationSections]
  SET QnAData = REPLACE(QnAData,
                        '{"Action":"NextPage","ReturnId":"6700","Conditions":[{"QuestionTag":"ApplyProviderRoute","MustEqual":"1"}],"ConditionMet":false},',
                        '')
  WHERE SequenceNo = 6 AND SectionNo = 2

-- Page 6900
  UPDATE [ApplicationSections]
  SET QnAData = REPLACE(QnAData,
                        '{"Action":"NextPage","ReturnId":"6700","Conditions":[{"QuestionTag":"ApplyProviderRoute","MustEqual":"1"},{"QuestionTag":"PATTypeOfTrainingMainEmployer","Contains":"Apprenticeship standards"}],"ConditionMet":false},',
                        '')
  WHERE SequenceNo = 6 AND SectionNo = 2
  
				
				
				