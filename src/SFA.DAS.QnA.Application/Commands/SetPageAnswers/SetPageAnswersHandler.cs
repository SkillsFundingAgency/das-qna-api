using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.SetPageAnswers
{
    public class SetPageAnswersHandler : SetAnswersBase, IRequestHandler<SetPageAnswersRequest, HandlerResponse<SetPageAnswersResponse>>
    {
        public SetPageAnswersHandler(QnaDataContext dataContext, IAnswerValidator answerValidator, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService) : base(dataContext, notRequiredProcessor, tagProcessingService, answerValidator)
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

            var workflowSection = _dataContext.WorkflowSections.Single(x => x.Id == request.SectionId);

            var page = workflowSection.QnAData.Pages.Single(x => x.PageId == request.PageId);

            //todo: save page answers somewhere

            var applicationSection = new ApplicationSection
            {
                ApplicationId = request.ApplicationId,
                DisplayType = workflowSection.DisplayType,
                Id = workflowSection.Id,
                LinkTitle = workflowSection.LinkTitle,
                QnAData = workflowSection.QnAData,
                SectionNo = 0, // ?????
                SequenceNo = 0, // ?????
                Title = workflowSection.Title
            };
            
            
            //SaveAnswersIntoPage(section, request.PageId, request.Answers);

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