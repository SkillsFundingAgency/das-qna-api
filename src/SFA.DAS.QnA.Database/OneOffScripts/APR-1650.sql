-- Fix for APR-1650 - Display 'Full name' correctly for Overall Manager
-- Date: 2020-05-28

-- Question DAT-754.1
  UPDATE [ApplicationSections]
  SET QnAData = REPLACE(QnAData,
                        '"QuestionId":"DAT-754.1","Label":"","ShortLabel":"","QuestionBodyText":"<p class=''govuk-body'' id=''dat-section_7_5_overall_manager''>This must be someone who is accountable for the team responsible for developing and delivering training.</p><p class=''govuk-body''>Full name</p>"',
                        '"QuestionId":"DAT-754.1","Label":"Full name","ShortLabel":"","QuestionBodyText":""')
  WHERE SequenceNo = 7 AND SectionNo = 5

  UPDATE [ApplicationSections]
  SET QnAData = REPLACE(QnAData,
                        '"BodyText":"","ActivatedByPageId":"7500,7510,7520,7530,7560,7570,7591,7592,7593,7954,7955"},{"PageId":"7560"',
                        '"BodyText":"<p class=''govuk-body'' id=''dat-section_7_5_overall_manager''>This must be someone who is accountable for the team responsible for developing and delivering training.</p>","ActivatedByPageId":"7500,7510,7520,7530,7560,7570,7591,7592,7593,7954,7955"},{"PageId":"7560"')
  WHERE SequenceNo = 7 AND SectionNo = 5


-- Question DAT-759.1
  UPDATE [ApplicationSections]
  SET QnAData = REPLACE(QnAData,
                        '"QuestionId":"DAT-759.1","Label":"","ShortLabel":"","QuestionBodyText":"<p class=''govuk-body'' id=''dat-section_7_5_overall_manager_supporting''>This must be someone who is accountable for the team responsible for developing and delivering training.</p><p class=''govuk-body''>Full name</p>"',
                        '"QuestionId":"DAT-759.1","Label":"Full name","ShortLabel":"","QuestionBodyText":""')
  WHERE SequenceNo = 7 AND SectionNo = 5
  
  UPDATE [ApplicationSections]
  SET QnAData = REPLACE(QnAData,
                        '"BodyText":"","ActivatedByPageId":"7500,7510,7520,7530,7560,7570,7591,7592,7593,7954,7955"},{"PageId":"7591"',
                        '"BodyText":"<p class=''govuk-body'' id=''dat-section_7_5_overall_manager_supporting''>This must be someone who is accountable for the team responsible for developing and delivering training.</p>","ActivatedByPageId":"7500,7510,7520,7530,7560,7570,7591,7592,7593,7954,7955"},{"PageId":"7591"')
  WHERE SequenceNo = 7 AND SectionNo = 5