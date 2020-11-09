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
        public SetPageAnswersHandler(QnaDataContext dataContext,   IAnswerValidator answerValidator, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService, IApplicationAnswersRepository applicationAnswersRepository, IWorkflowRepository workflowRepository, IApplicationRepository applicationRepository) :
            base(dataContext, notRequiredProcessor, tagProcessingService, answerValidator, applicationAnswersRepository, workflowRepository, applicationRepository)
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
            var application = await ApplicationRepository.GetApplication(request.ApplicationId);
            if (application is null) return new HandlerResponse<SetPageAnswersResponse>(false, "Application does not exist");


            var workflowSection = await WorkflowRepository.GetWorkflowSection(request.SectionId);
                
            var page = workflowSection.QnAData.Pages.FirstOrDefault(x => x.PageId == request.PageId);

            var applicationSection = new ApplicationSection
            {
                ApplicationId = request.ApplicationId,
                DisplayType = workflowSection.DisplayType,
                Id = workflowSection.Id,
                LinkTitle = workflowSection.LinkTitle,
                QnAData = new QnAData(workflowSection.QnAData),
                //SectionNo = workflowSection.Sequence.SectionNo,
                //SequenceNo = workflowSection.Sequence.SequenceNo, 
                Title = workflowSection.Title
            };

            await SaveAnswersIntoPage(applicationSection, request.PageId, request.Answers);

            UpdateApplicationData(request.PageId, request.Answers, applicationSection, application);

            var nextAction = GetNextActionForPage(applicationSection, application, request.PageId);
            var checkboxListAllNexts = GetCheckboxListMatchingNextActionsForPage(applicationSection, application, request.PageId);

            SetStatusOfNextPagesBasedOnDeemedNextActions(applicationSection, request.PageId, nextAction, checkboxListAllNexts);
            await ApplicationRepository.StoreApplicationSectionPageStates(applicationSection.ApplicationId,
                applicationSection.Id, applicationSection.QnAData.Pages, false);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));
        }
    }
}