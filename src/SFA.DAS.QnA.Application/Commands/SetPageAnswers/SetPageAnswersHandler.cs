using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.SetPageAnswers
{
    public class SetPageAnswersHandler : SetAnswersBase, IRequestHandler<SetPageAnswersRequest, HandlerResponse<SetPageAnswersResponse>>
    {
        private readonly IAnswerValidator _answerValidator;

        public SetPageAnswersHandler(QnaDataContext dataContext, IAnswerValidator answerValidator) : base(dataContext)
        {
            _answerValidator = answerValidator;
        }

        public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(SetPageAnswersRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            var qnaData = new QnAData(section.QnAData);
            var page = qnaData.Pages.FirstOrDefault(p => p.PageId == request.PageId);

            if (page.AllowMultipleAnswers) return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for Multiple Answers pages. Use AddAnswer / RemoveAnswer instead.");

            if (page.Questions.Count > 0)
            {
                if (page.Questions.Count > request.Answers.Count)
                {
                    return new HandlerResponse<SetPageAnswersResponse>(success: false, message: $"Number of Answers supplied ({request.Answers.Count}) does not match number of first level Questions on page ({page.Questions.Count}).");
                }
                
                var validationErrors = _answerValidator.Validate(request.Answers, page);
                if (validationErrors.Any())
                {
                    return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(validationErrors));
                }
            }

            page.Complete = true;

            MarkFeedbackComplete(page);
            
            await SaveAnswersIntoPage(request, cancellationToken, qnaData, section);

            await UpdateApplicationData(request.ApplicationId, page, request.Answers);

            var nextAction = GetNextAction(page, request.Answers, section);

            var checkboxListAllNexts = GetCheckboxListMatchingNextActions(page, request.Answers, section);
            
            SetStatusOfNextPagesBasedOnAnswer(section.Id, page.PageId, request.Answers, nextAction, checkboxListAllNexts);
            
            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));
        }

        private async Task UpdateApplicationData(Guid applicationId, Page page, List<Answer> answers)
        {
            var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == applicationId);
            var applicationData = JObject.Parse(application.ApplicationData);
            var questionTags = new List<string>();

            foreach (var question in page.Questions)
            {
                SetApplicationDataField(answers, applicationData, question);
                if (!string.IsNullOrWhiteSpace(question.QuestionTag)) questionTags.Add(question.QuestionTag);

                if (question.Input.Options == null) continue;

                foreach (var option in question.Input.Options.Where(o => o.FurtherQuestions != null))
                {
                    foreach (var furtherQuestion in option.FurtherQuestions)
                    {
                        SetApplicationDataField(answers, applicationData, furtherQuestion);
                        if (!string.IsNullOrWhiteSpace(question.QuestionTag)) questionTags.Add(question.QuestionTag);
                    }
                }
            }

            application.ApplicationData = applicationData.ToString(Formatting.None);

            await _dataContext.SaveChangesAsync();

            await SetStatusOfAllPagesBasedOnUpdatedQuestionTags(applicationId, questionTags);
        }

        private async Task SetStatusOfAllPagesBasedOnUpdatedQuestionTags(Guid applicationId, List<string> questionTags)
        {
            if (questionTags == null || questionTags.Count < 1) return;

            var sections = await _dataContext.ApplicationSections.Where(sec => sec.ApplicationId == applicationId).ToListAsync();

            // Go through each section in the application
            foreach (var section in sections)
            {
                var qnaData = new QnAData(section.QnAData);

                // Get the list of pages that contain one of QuestionTags in the next condition
                var pages = new List<Page>();
                foreach (var questionTag in questionTags.Distinct())
                {
                   var questionTagPages = qnaData.Pages.Where(p => !p.AllowMultipleAnswers && p.Next.SelectMany(n => n.Conditions).Select(c => c.QuestionTag).Contains(questionTag));
                   pages.AddRange(questionTagPages);
                }

                // Deactivate & Activate affected pages accordingly
                foreach (var page in pages)
                {
                    var nextAction = GetNextAction(page, new List<Answer>(), section);
                    if(nextAction?.Conditions != null)
                    {
                        DeactivateDependentPages(page.PageId, qnaData, page, nextAction);
                        ActivateDependentPages(nextAction, page.PageId, qnaData);
                    }
                }

                // Assign the updated QnAData back to the section
                section.QnAData = qnaData;
            }

            await _dataContext.SaveChangesAsync();
        }

        private static void SetApplicationDataField(List<Answer> answers, JObject applicationData, Question question)
        {
            if (string.IsNullOrWhiteSpace(question.QuestionTag)) return;

            if (applicationData.ContainsKey(question.QuestionTag))
            {
                applicationData[question.QuestionTag] = answers.Single(a => a.QuestionId == question.QuestionId).Value;
            }
            else
            {
                applicationData.Add(question.QuestionTag, new JValue(answers.Single(a => a.QuestionId == question.QuestionId).Value));
            }
        }


        private async Task SaveAnswersIntoPage(SetPageAnswersRequest request, CancellationToken cancellationToken, QnAData qnaData, ApplicationSection section)
        {
            qnaData.Pages.Single(p => p.PageId == request.PageId).PageOfAnswers = new List<PageOfAnswers>(new[] {new PageOfAnswers() {Answers = request.Answers}});
            section.QnAData = qnaData;

            await _dataContext.SaveChangesAsync(cancellationToken);
        }

        
    }
}