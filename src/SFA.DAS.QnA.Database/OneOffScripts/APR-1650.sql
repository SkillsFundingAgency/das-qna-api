-- Fix for APR-1650 - Display 'Full name' correctly for Overall Manager
-- Date: 2020-04-16

-- Question DAT-754.1
  UPDATE [ApplicationSections]
  SET QnAData = REPLACE(QnAData,
                        '"Label":"","ShortLabel":"","QuestionBodyText":"<p class=''govuk-body'' id=''dat-section_7_5_overall_manager''>This must be someone who is accountable for the team responsible for developing and delivering training.</p><p class=''govuk-body''>Full name</p>"',
                        '"Label":"<h1 class=''govuk-heading-l''>Full name</h1>","ShortLabel":"","QuestionBodyText":"<p class=''govuk-body'' id=''dat-section_7_5_overall_manager''>This must be someone who is accountable for the team responsible for developing and delivering training.</p>"')
  WHERE SequenceNo = 7 AND SectionNo = 5

-- Question DAT-759.1
  UPDATE [ApplicationSections]
  SET QnAData = REPLACE(QnAData,
                        '"Label":"","ShortLabel":"","QuestionBodyText":"<p class=''govuk-body'' id=''dat-section_7_5_overall_manager_supporting''>This must be someone who is accountable for the team responsible for developing and delivering training.</p><p class=''govuk-body''>Full name</p>",',
                        '"Label":"<h1 class=''govuk-heading-l''>Full name</h1>","ShortLabel":"","QuestionBodyText":"<p class=''govuk-body'' id=''dat-section_7_5_overall_manager_supporting''>This must be someone who is accountable for the team responsible for developing and delivering training.</p>"')
  WHERE SequenceNo = 7 AND SectionNo = 5