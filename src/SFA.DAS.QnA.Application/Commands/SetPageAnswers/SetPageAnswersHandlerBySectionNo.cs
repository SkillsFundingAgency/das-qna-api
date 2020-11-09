using System.Linq;
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
    public class SetPageAnswersBySectionNoHandler : SetAnswersBase, IRequestHandler<SetPageAnswersBySectionNoRequest, HandlerResponse<SetPageAnswersResponse>>
    {
        public SetPageAnswersBySectionNoHandler(QnaDataContext dataContext, IAnswerValidator answerValidator, INotRequiredProcessor notRequiredProcessor, ITagProcessingService tagProcessingService, IApplicationAnswersRepository applicationAnswersRepository, IWorkflowRepository workflowRepository, IApplicationRepository applicationRepository) 
            : base(dataContext, notRequiredProcessor, tagProcessingService, answerValidator, applicationAnswersRepository, workflowRepository, applicationRepository)
        {
        }

        public async Task<HandlerResponse<SetPageAnswersResponse>> Handle(SetPageAnswersBySectionNoRequest request, CancellationToken cancellationToken)
        {
            var application = await ApplicationRepository.GetApplication(request.ApplicationId);
            if (application is null) return new HandlerResponse<SetPageAnswersResponse>(false, "Application does not exist");



            var sequenceSection =
                await WorkflowRepository.GetWorkflowSection(application.WorkflowId, request.SectionNo,
                    request.SequenceNo);
                
            var page = sequenceSection.QnAData.Pages.FirstOrDefault(x => x.PageId == request.PageId);

            var section = new ApplicationSection
            {
                ApplicationId = request.ApplicationId,
                DisplayType = sequenceSection.DisplayType,
                Id = sequenceSection.Id,
                LinkTitle = sequenceSection.LinkTitle,
                QnAData = new QnAData(sequenceSection.QnAData),
                SectionNo = request.SectionNo,
                SequenceNo = request.SequenceNo,
                Title = sequenceSection.Title
            };

            var validationErrorResponse = ValidateSetPageAnswersRequest(request.PageId, request.Answers, section);

            if (validationErrorResponse != null)
            {
                return validationErrorResponse;
            }

            await SaveAnswersIntoPage(section, request.PageId, request.Answers);

            UpdateApplicationData(request.PageId, request.Answers, section, application);

            var nextAction = GetNextActionForPage(section, application, request.PageId);
            var checkboxListAllNexts = GetCheckboxListMatchingNextActionsForPage(section, application, request.PageId);

            SetStatusOfNextPagesBasedOnDeemedNextActions(section, request.PageId, nextAction, checkboxListAllNexts);
            await ApplicationRepository.StoreApplicationSectionPageStates(section.ApplicationId,
                section.Id, section.QnAData.Pages, false);

            await _dataContext.SaveChangesAsync(cancellationToken);

            return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));



            //var section = await _dataContext.ApplicationSections.SingleOrDefaultAsync(sec => sec.SequenceNo == request.SequenceNo && sec.SectionNo == request.SectionNo && sec.ApplicationId == request.ApplicationId, cancellationToken);
            //var validationErrorResponse = ValidateSetPageAnswersRequest(request.PageId, request.Answers, section);

            //if(validationErrorResponse != null)
            //{
            //    return validationErrorResponse;
            //}

            //await SaveAnswersIntoPage(section, request.PageId, request.Answers);

            //var application = await _dataContext.Applications.SingleOrDefaultAsync(app => app.Id == request.ApplicationId, cancellationToken);
            //UpdateApplicationData(request.PageId, request.Answers, section, application);

            //var nextAction = GetNextActionForPage(section, application, request.PageId);
            //var checkboxListAllNexts = GetCheckboxListMatchingNextActionsForPage(section, application, request.PageId);

            //SetStatusOfNextPagesBasedOnDeemedNextActions(section, request.PageId, nextAction, checkboxListAllNexts);

            //await _dataContext.SaveChangesAsync(cancellationToken);

            //return new HandlerResponse<SetPageAnswersResponse>(new SetPageAnswersResponse(nextAction.Action, nextAction.ReturnId));
        }
    }
}