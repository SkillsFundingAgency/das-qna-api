using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.SetPageAnswers
{
    public class SetAnswersBase
    {

        protected List<Next> GetCheckboxListMatchingNextActions(Page page, List<Answer> answers, ApplicationSection section, QnaDataContext qnaDataContext)
        {
            if (page.Next is null || !page.Next.Any())
            {
                throw new ApplicationException($"Page {page.PageId}, in Sequence {page.SequenceId}, Section {page.SectionId} has no 'Next' instructions.");
            }

            if (page.Questions.All(q => q.Input.Type != "CheckboxList"))
            {
                return new List<Next>();
            }

            Next nextAction = null;
            
            if (page.Next.Count == 1)
            {
                nextAction = page.Next.Single();
            }

            var matchingNexts = new List<Next>();
            
            foreach (var next in page.Next)
            {
                if (next.Conditions != null && next.Conditions.Any())
                {
                    var someConditionsNotSatisfied = false;
                    
                    foreach (var condition in next.Conditions.Where(c=> c.Contains != null))
                    {
                       
                            var question = page.Questions.Single(q => q.QuestionId == condition.QuestionId);
                            var answer = answers.FirstOrDefault(a => a.QuestionId == condition.QuestionId);

                            if (question.Input.Type == "CheckboxList")
                            {
                                if (answer == null)
                                {
                                    someConditionsNotSatisfied = true;
                                }
                                else
                                {
                                    var answerValueList = answer.Value.Split(",", StringSplitOptions.RemoveEmptyEntries);
                                
                                    if (answer.QuestionId != condition.QuestionId || !answerValueList.Contains(condition.Contains))
                                    {
                                        someConditionsNotSatisfied = true;
                                    }    
                                }
                            }
                            else
                            {
                                if (answer == null || answer.QuestionId != condition.QuestionId || answer.Value != condition.MustEqual)
                                {
                                    someConditionsNotSatisfied = true;
                                }    
                            }
                    }

                    if (!someConditionsNotSatisfied)
                    {
                        matchingNexts.Add(next);
                    }
                }
            }

            var matchingNextsToReturn = new List<Next>();

            foreach (var matchingNext in matchingNexts)
            {
                nextAction = FindNextRequiredAction(section, qnaDataContext, matchingNext);
            
                if (nextAction != null)
                {
                    matchingNextsToReturn.Add(nextAction);
                }
            }

            return matchingNextsToReturn;
        }
        
        protected Next GetNextAction(Page page, List<Answer> answers, ApplicationSection section, QnaDataContext qnaDataContext)
        {
            if (page.Next is null || !page.Next.Any())
            {
                throw new ApplicationException($"Page {page.PageId}, in Sequence {page.SequenceId}, Section {page.SectionId} has no 'Next' instructions.");
            }

            Next nextAction = null;
            
            if (page.Next.Count == 1)
            {
                nextAction = page.Next.Single();
            }

            foreach (var next in page.Next)
            {
                if (next.Conditions != null)
                {
                    var someConditionsNotSatisfied = false;
                    
                    foreach (var condition in next.Conditions)
                    {
                        if (!String.IsNullOrWhiteSpace(condition.QuestionTag))
                        {
                            var application =
                                qnaDataContext.Applications.FirstOrDefault(app => app.Id == section.ApplicationId);
                            var applicationData = JObject.Parse(application.ApplicationData);
                            var questionTag = applicationData[condition.QuestionTag];

                            if (questionTag == null || questionTag.Value<string>() != condition.MustEqual)
                            {
                                someConditionsNotSatisfied = true;
                            }
                        }
                        else
                        {
                            var question = page.Questions.Single(q => q.QuestionId == condition.QuestionId);
                            var answer = answers.FirstOrDefault(a => a.QuestionId == condition.QuestionId);

                            if (question.Input.Type == "CheckboxList")
                            {
                                if (answer == null)
                                {
                                    someConditionsNotSatisfied = true;
                                }
                                else
                                {
                                    var answerValueList = answer.Value.Split(",", StringSplitOptions.RemoveEmptyEntries);
                                
                                    if (answer.QuestionId != condition.QuestionId || !answerValueList.Contains(condition.Contains))
                                    {
                                        someConditionsNotSatisfied = true;
                                    }    
                                }
                            }
                            else
                            {
                                if (answer == null || answer.QuestionId != condition.QuestionId || answer.Value != condition.MustEqual)
                                {
                                    someConditionsNotSatisfied = true;
                                }    
                            }
                        }   
                    }

                    if (!someConditionsNotSatisfied)
                    {
                        nextAction = next;
                        break;
                    }
                }
                else
                {
                    nextAction = next;
                }
            }

            nextAction = FindNextRequiredAction(section, qnaDataContext, nextAction);
            
            if (nextAction != null)
            {
                return nextAction;
            }
            
            throw new ApplicationException($"Page {page.PageId}, in Sequence {page.SequenceId}, Section {page.SectionId} is missing a matching 'Next' instruction for Application {section.ApplicationId}");
        }

        public Next FindNextRequiredAction(ApplicationSection section, QnaDataContext qnaDataContext, Next nextAction)
        {
            if (nextAction is null || nextAction.Action != "NextPage") return nextAction;
            
            // Check here for any NotRequiredConditions on the next page.

            var application = qnaDataContext.Applications.Single(app => app.Id == section.ApplicationId);
            var applicationData = JObject.Parse(application.ApplicationData);

            var nextPage = section.QnAData.Pages.FirstOrDefault(p => p.PageId == nextAction.ReturnId);
            var isRequired = true;
            if (nextPage != null && nextPage.NotRequiredConditions != null && nextPage.NotRequiredConditions.Any())
            {
                if (nextPage.NotRequiredConditions.Any(nrc => nrc.IsOneOf.Contains(applicationData[nrc.Field].Value<string>())))
                {
                    isRequired = false;
                }
            }

            if (isRequired) return nextAction;
            
            // Get the next default action from this page.
            if (nextPage != null && nextPage.Next.Count == 1)
            {
                nextAction = nextPage.Next.First();
            }
            else if (nextPage.Next.Any(n => n.Conditions == null))
            {
                nextAction = nextPage.Next.Single(n => n.Conditions == null);
            }
            else
            {
                nextAction = nextPage.Next.Last();
            }

            nextAction = FindNextRequiredAction(section, qnaDataContext, nextAction);

            return nextAction;
        }

        protected void MarkFeedbackComplete(Page page)
        {
            if (page.HasFeedback)
            {
                page.Feedback.ForEach(f => f.IsCompleted = true);
            }
        }
    }
}