using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Repositories;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.SetPageAnswers
{
    public class SetPageAnswersHandler : SetAnswersBase, IRequestHandler<SetPageAnswersRequest, HandlerResponse<SetPageAnswersResponse>>
    {
        //public SetPageAnswersHandler(QnaDataContext dataContext, IAnswerValidator answerValidator, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService) : base(dataContext, notRequiredProcessor, tagProcessingService, answerValidator)
        //{
        //}
        public SetPageAnswersHandler(QnaDataContext dataContext,   IAnswerValidator answerValidator, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService, IApplicationAnswersRepository applicationAnswersRepository) :
            base(dataContext, notRequiredProcessor, tagProcessingService, answerValidator, applicationAnswersRepository)
        {
        }

        //public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(SetPageAnswersRequest request, CancellationToken cancellationToken)
        //{
        //    var section = await _dataContext.ApplicationSections.SingleOrDefaultAsync(sec => sec.Id == request.SectionId && sec.ApplicationId == request.ApplicationId, cancellationToken);
        //    var validationErrorResponse = ValidateSetPageAnswersRequest(request.PageId, request.Answers, section);

        //    if(validationErrorResponse != null)
        //    {
        //        return validationErrorResponse;
        //    }

        //    SaveAnswersIntoPage(section, request.PageId, request.Answers);

        //    var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
        //    UpdateApplicationData(request.PageId, request.Answers, section, application);

        //    var nextAction = GetNextActionForPage(section, application, request.PageId);
        //    var checkboxListAllNexts = GetCheckboxListMatchingNextActionsForPage(section, application, request.PageId);

        //    SetStatusOfNextPagesBasedOnDeemedNextActions(section, request.PageId, nextAction, checkboxListAllNexts);

        //    await _dataContext.SaveChangesAsync(cancellationToken);

        //    return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));
        //}


        public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(SetPageAnswersRequest request, CancellationToken cancellationToken)
        {
            var application = await _dataContext.Applications.AsNoTracking().FirstOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken: cancellationToken);
            if (application is null) return new HandlerResponse<SetPageAnswersResponse>(false, "Application does not exist");


            var workflowSection = await _dataContext.WorkflowSections.Where(x => x.Id == request.SectionId)
                .Join(_dataContext.WorkflowSequences.Where(seq => seq.SectionId == request.SectionId),
                    section => section.Id, sequence => sequence.SectionId,
                    (section, sequence) => new {Sequence = sequence, Section = section})
                .FirstOrDefaultAsync(cancellationToken);
                
            var page = workflowSection.Section.QnAData.Pages.FirstOrDefault(x => x.PageId == request.PageId);

            var applicationSection = new ApplicationSection
            {
                ApplicationId = request.ApplicationId,
                DisplayType = workflowSection.Section.DisplayType,
                Id = workflowSection.Section.Id,
                LinkTitle = workflowSection.Section.LinkTitle,
                QnAData = workflowSection.Section.QnAData,
                SectionNo = workflowSection.Sequence.SectionNo,
                SequenceNo = workflowSection.Sequence.SequenceNo, 
                Title = workflowSection.Section.Title
            };

            await SaveAnswersIntoPage(applicationSection, request.PageId, request.Answers);

            //var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
            //UpdateApplicationData(request.PageId, request.Answers, section, application);

            var nextAction = GetNextActionForPage(applicationSection, application, request.PageId);
            //var checkboxListAllNexts = GetCheckboxListMatchingNextActionsForPage(section, application, request.PageId);

            //SetStatusOfNextPagesBasedOnDeemedNextActions(section, request.PageId, nextAction, checkboxListAllNexts);

            //await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));
        }
    }
}