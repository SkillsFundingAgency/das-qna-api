using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Qna.Api.Types;
using SFA.DAS.Qna.Api.Types.Page;
using SFA.DAS.Qna.Data;
using SFA.DAS.QnA.Application.Queries.Sections.GetPage;
using SFA.DAS.QnA.Application.Validators;

namespace SFA.DAS.QnA.Application.Commands.SavePageAnswers
{
    public class SetPageAnswersHandler : IRequestHandler<SetPageAnswersRequest, HandlerResponse<SetPageAnswersResponse>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IValidatorFactory _validatorFactory;

        public SetPageAnswersHandler(QnaDataContext dataContext, IValidatorFactory validatorFactory)
        {
            _dataContext = dataContext;
            _validatorFactory = validatorFactory;
        }

        public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(SetPageAnswersRequest request, CancellationToken cancellationToken)
        {
            var section = await _dataContext.ApplicationSections.FirstOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
            var qnaData = new QnAData(section.QnAData);
            var page = qnaData.Pages.FirstOrDefault(p => p.PageId == request.PageId);

            var existingAnswers = page.PageOfAnswers.Select(poa => new PageOfAnswers {Answers = poa.Answers}).ToList();

            if (page.AllowMultipleAnswers) return new HandlerResponse<SetPageAnswersResponse>(success: false, message: "This endpoint cannot be used for Multiple Answers pages. Use AddAnswer / RemoveAnswer instead.");

            var validationErrors = Validate(request, page, existingAnswers);
            if (validationErrors.Any())
            {
                return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(validationErrors));
            }

            page.Complete = true;

            if (page.HasFeedback)
            {
                page.Feedback.ForEach(f => f.IsCompleted = true);
            }

            // If validation passes.....
            // Get next action...
            // set answers
            qnaData.Pages.Single(p => p.PageId == request.PageId).PageOfAnswers = new List<PageOfAnswers>(new[] {new PageOfAnswers() {Answers = request.Answers}});
            section.QnAData = qnaData;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse("REPLACETHIS", "REPLACETHIS"));

            // return response
        }

        private List<KeyValuePair<string, string>> Validate(SetPageAnswersRequest request, Page page, List<PageOfAnswers> existingAnswers)
        {
            // Validation
            var validationErrors = new List<KeyValuePair<string, string>>();
            foreach (var question in page.Questions)
            {
                var answerToThisQuestion = request.Answers.Single(a => a.QuestionId == question.QuestionId);
                var existingAnswer = existingAnswers.SelectMany(poa => poa.Answers).FirstOrDefault(a => a.QuestionId == question.QuestionId);

                ValidateQuestion(question, validationErrors, answerToThisQuestion);

                if (question.Input.Options != null)
                {
                    foreach (var option in question.Input.Options)
                    {
                        if (answerToThisQuestion?.Value == option.Value && option.FurtherQuestions != null)
                        {
                            foreach (var furtherQuestion in option.FurtherQuestions)
                            {
                                var furtherAnswer = request.Answers.FirstOrDefault(a => a.QuestionId == furtherQuestion.QuestionId);
                                ValidateQuestion(furtherQuestion, validationErrors, furtherAnswer);
                            }
                        }
                    }
                }
            }

            return validationErrors;
        }

        private void ValidateQuestion(Question question, List<KeyValuePair<string, string>> validationErrors, Answer answerToThisQuestion)
        {
            var validators = _validatorFactory.Build(question);

            foreach (var validator in validators)
            {
                var errors = validator.Validate(question, answerToThisQuestion);

                if (errors.Any())
                {
                    validationErrors.AddRange(errors);
                }
            }
        }
    }
}