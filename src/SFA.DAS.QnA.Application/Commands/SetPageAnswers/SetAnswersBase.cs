using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.SetPageAnswers
{
    public class SetAnswersBase
    {
        protected readonly QnaDataContext _dataContext;

        public SetAnswersBase(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        protected List<Next> GetCheckboxListMatchingNextActionsForPage(Guid sectionId, string pageId)
        {
            var section = _dataContext.ApplicationSections.AsNoTracking().SingleOrDefault(sec => sec.Id == sectionId);
            var page = section?.QnAData?.Pages.SingleOrDefault(p => p.PageId == pageId);

            if (page is null)
            {
                return new List<Next>();
            }
            else if (page.Next is null || !page.Next.Any())
            {
                throw new ApplicationException($"Page {page.PageId}, in Sequence {page.SequenceId}, Section {page.SectionId} has no 'Next' instructions.");
            }
            else if (page.Questions.All(q => !"CheckboxList".Equals(q.Input.Type, StringComparison.InvariantCultureIgnoreCase)))
            {
                return new List<Next>();
            }

            var matchingNexts = new List<Next>();

            foreach (var next in page.Next)
            {
                var allConditionsSatisfied = true;

                if (next.Conditions != null && next.Conditions.Any())
                {
                    foreach (var condition in next.Conditions.Where(c => c.Contains != null))
                    {
                        var question = page.Questions.Single(q => q.QuestionId == condition.QuestionId);
                        var answers = page.PageOfAnswers?.FirstOrDefault()?.Answers;
                        var answer = answers?.FirstOrDefault(a => a.QuestionId == condition.QuestionId);

                        if ("CheckboxList".Equals(question.Input.Type, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (answer == null)
                            {
                                allConditionsSatisfied = false;
                                break;
                            }
                            else
                            {
                                var answerValueList = answer.Value.Split(",", StringSplitOptions.RemoveEmptyEntries);

                                if (answer.QuestionId != condition.QuestionId || !answerValueList.Contains(condition.Contains))
                                {
                                    allConditionsSatisfied = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (answer == null || answer.QuestionId != condition.QuestionId || answer.Value != condition.MustEqual)
                            {
                                allConditionsSatisfied = false;
                                break;
                            }
                        }
                    }
                }

                if (allConditionsSatisfied)
                {
                    // NOTE: In this version we add all of the matching conditions.
                    matchingNexts.Add(next);
                }
            }

            var application = _dataContext.Applications.AsNoTracking().SingleOrDefault(app => app.Id == section.ApplicationId);
            var applicationData = JObject.Parse(application?.ApplicationData ?? "{}");
            var matchingNextsToReturn = new List<Next>();

            foreach (var matchingNext in matchingNexts)
            {
                var nextAction = FindNextRequiredAction(section, matchingNext, applicationData);

                if (nextAction != null)
                {
                    matchingNextsToReturn.Add(nextAction);
                }
            }

            return matchingNextsToReturn;
        }

        protected Next GetNextActionForPage(Guid sectionId, string pageId)
        {
            var section = _dataContext.ApplicationSections.AsNoTracking().SingleOrDefault(sec => sec.Id == sectionId);
            var page = section?.QnAData?.Pages.SingleOrDefault(p => p.PageId == pageId);

            if (page is null)
            {
                return null;
            }
            else if (page.Next is null || !page.Next.Any())
            {
                throw new ApplicationException($"Page {page.PageId}, in Sequence {page.SequenceId}, Section {page.SectionId} has no 'Next' instructions.");
            }

            var application = _dataContext.Applications.AsNoTracking().SingleOrDefault(app => app.Id == section.ApplicationId);
            var applicationData = JObject.Parse(application?.ApplicationData ?? "{}");

            Next nextAction = null;

            if (page.Next.Count == 1)
            {
                nextAction = page.Next.Single();
            }

            foreach (var next in page.Next)
            {
                var allConditionsSatisfied = true;

                if (next.Conditions != null && next.Conditions.Any())
                {
                    foreach (var condition in next.Conditions)
                    {
                        if (!String.IsNullOrWhiteSpace(condition.QuestionTag))
                        {
                            var questionTagValue = applicationData[condition.QuestionTag];

                            if (questionTagValue == null )
                            {
                                allConditionsSatisfied = false;
                                break;
                            }
                            else if (!string.IsNullOrEmpty(condition.MustEqual) && questionTagValue.Value<string>() != condition.MustEqual)
                            {
                                allConditionsSatisfied = false;
                                break;
                            }
                            else if (!string.IsNullOrEmpty(condition.Contains))
                            {
                                var listOfAnswers = questionTagValue.Value<string>()
                                    .Split(",", StringSplitOptions.RemoveEmptyEntries);
                                if (!listOfAnswers.Contains(condition.Contains))
                                {
                                    allConditionsSatisfied = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            var question = page.Questions.Single(q => q.QuestionId == condition.QuestionId);
                            var answers = page.PageOfAnswers?.FirstOrDefault()?.Answers;
                            var answer = answers?.FirstOrDefault(a => a.QuestionId == condition.QuestionId);

                            if ("CheckboxList".Equals(question.Input.Type, StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (answer == null)
                                {
                                    allConditionsSatisfied = false;
                                    break;
                                }
                                else
                                {
                                    var answerValueList = answer.Value.Split(",", StringSplitOptions.RemoveEmptyEntries);

                                    if (answer.QuestionId != condition.QuestionId || !answerValueList.Contains(condition.Contains))
                                    {
                                        allConditionsSatisfied = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (answer == null || answer.QuestionId != condition.QuestionId || answer.Value != condition.MustEqual)
                                {
                                    allConditionsSatisfied = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (allConditionsSatisfied)
                {
                    // NOTE: In this version we return the first Next action that is satisfied. In the Checkbox version we return all of the matching ones.
                    // At some point we should check this intended behaviour works if multiple Next actions are satisfied BUT one of them are actually not required.
                    nextAction = next;
                    break;
                }
            }

            nextAction = FindNextRequiredAction(section, nextAction, applicationData);

            if (nextAction != null)
            {
                return nextAction;
            }

            throw new ApplicationException($"Page {page.PageId}, in Sequence {page.SequenceId}, Section {page.SectionId} is missing a matching 'Next' instruction for Application {section.ApplicationId}");
        }

        public Next FindNextRequiredAction(ApplicationSection section, Next nextAction, JObject applicationData)
        {
            if (section?.QnAData is null || nextAction is null || nextAction.Action != "NextPage") return nextAction;

            var isRequiredNextAction = true;

            // Check here for any NotRequiredConditions on the next page.
            var nextPage = section.QnAData?.Pages.FirstOrDefault(p => p.PageId == nextAction.ReturnId);

            if (nextPage is null || applicationData is null)
            {
                return nextAction;
            }
            else if (nextPage.NotRequiredConditions != null && nextPage.NotRequiredConditions.Any())
            {
                if (nextPage.NotRequiredConditions.Any(nrc => nrc.IsOneOf != null && nrc.IsOneOf.Contains(applicationData[nrc.Field]?.Value<string>())))
                {
                    isRequiredNextAction = false;
                }
            }

            if (isRequiredNextAction || nextPage.Next is null) return nextAction;

            // Get the next default action from this page.
            if (nextPage.Next.Count == 1)
            {
                nextAction = nextPage.Next.Single();
            }
            else if (nextPage.Next.Any(n => n.Conditions == null))
            {
                // For some reason null Conditions takes precedence over empty Conditions
                nextAction = nextPage.Next.Last(n => n.Conditions == null);
            }
            else if (nextPage.Next.Any(n => n.Conditions.Count == 0))
            {
                nextAction = nextPage.Next.Last(n => n.Conditions.Count == 0);
            }
            else
            {
                nextAction = nextPage.Next.Last();
            }

            // NOTE the recursion!
            return FindNextRequiredAction(section, nextAction, applicationData);
        }

        protected void MarkPageAsComplete(Page page)
        {
            page.Complete = true;
        }

        protected void MarkPageFeedbackAsComplete(Page page)
        {
            if (page.HasFeedback)
            {
                page.Feedback.ForEach(f => f.IsCompleted = true);
            }
        }

        protected void SetStatusOfNextPagesBasedOnDeemedNextActions(Guid sectionId, string pageId, Next deemedNextAction, List<Next> deemedCheckboxListNextActions)
        {
            var section = _dataContext.ApplicationSections.FirstOrDefault(sec => sec.Id == sectionId);

            if (section != null)
            {
                // Have to force QnAData a new object and reassign for Entity Framework to pick up changes
                var qnaData = new QnAData(section.QnAData);
                var page = qnaData?.Pages.SingleOrDefault(p => p.PageId == pageId);

                if (page != null)
                {
                    if (deemedCheckboxListNextActions != null && deemedCheckboxListNextActions.Any())
                    {
                        foreach (var checkboxNextAction in deemedCheckboxListNextActions)
                        {
                            DeactivateDependentPages(checkboxNextAction, page.PageId, qnaData, page);
                        }

                        foreach (var checkboxNextAction in deemedCheckboxListNextActions)
                        {
                            ActivateDependentPages(checkboxNextAction, page.PageId, qnaData);
                        }
                    }
                    else if(deemedNextAction != null)
                    {
                        DeactivateDependentPages(deemedNextAction, page.PageId, qnaData, page);
                        ActivateDependentPages(deemedNextAction, page.PageId, qnaData);
                    }

                    // Assign QnAData back so Entity Framework will pick up changes
                    section.QnAData = qnaData;
                    _dataContext.SaveChanges();
                }
            }
        }

        protected void DeactivateDependentPages(Next chosenAction, string branchingPageId, QnAData qnaData, Page page, bool subPages = false)
        {
            if (page != null)
            {
                // process all sub pages or those which are not the chosen action
                foreach (var nextAction in page.Next.Where(n => subPages || !(n.Action == chosenAction.Action && n.ReturnId == chosenAction.ReturnId)))
                {
                    if ("NextPage".Equals(nextAction.Action, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var nextPage = qnaData.Pages.FirstOrDefault(p => p.PageId == nextAction.ReturnId);
                        if (nextPage != null)
                        {
                            if (nextPage.ActivatedByPageId != null && nextPage.ActivatedByPageId.Split(",", StringSplitOptions.RemoveEmptyEntries).Contains(branchingPageId))
                            {
                                nextPage.Active = false;
                            }

                            foreach (var nextPagesAction in nextPage.Next)
                            {
                                DeactivateDependentPages(nextPagesAction, branchingPageId, qnaData, nextPage, true);
                            }
                        }
                    }
                }
            }
        }

        protected void ActivateDependentPages(Next chosenAction, string branchingPageId, QnAData qnaData)
        {
            if (chosenAction != null && "NextPage".Equals(chosenAction.Action, StringComparison.InvariantCultureIgnoreCase)  && qnaData != null)
            {
                var nextPage = qnaData.Pages.FirstOrDefault(p => p.PageId == chosenAction.ReturnId);
                if (nextPage != null)
                {
                    if (nextPage.ActivatedByPageId != null && nextPage.ActivatedByPageId.Split(",", StringSplitOptions.RemoveEmptyEntries).Contains(branchingPageId))
                    {
                        nextPage.Active = true;
                    }

                    foreach (var nextPagesAction in nextPage.Next)
                    {
                        ActivateDependentPages(nextPagesAction, branchingPageId, qnaData);
                    }
                }
            }
        }
    }
}