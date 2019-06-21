using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.Qna.Api.Types;
using SFA.DAS.Qna.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Commands.StartApplication
{
    public class StartApplicationHandler : IRequestHandler<StartApplicationRequest, HandlerResponse<StartApplicationResponse>>
    {
        private readonly QnaDataContext _dataContext;

        public StartApplicationHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public async Task<HandlerResponse<StartApplicationResponse>> Handle(StartApplicationRequest request, CancellationToken cancellationToken)
        {
            var newApplication = await CreateNewApplication(request, cancellationToken);

            if (newApplication is null) return new HandlerResponse<StartApplicationResponse>(false, $"WorkflowType '{request.WorkflowType}' does not exist.");
            
            await CopyWorkflows(cancellationToken, newApplication);

            return new HandlerResponse<StartApplicationResponse>(new StartApplicationResponse {ApplicationId = newApplication.Id});
        }

        private async Task<Data.Entities.Application> CreateNewApplication(StartApplicationRequest request, CancellationToken cancellationToken)
        {
            var latestWorkflow = await _dataContext.Workflows.FirstOrDefaultAsync(w => w.Type == request.WorkflowType && w.Status == "Live", cancellationToken);
            if (latestWorkflow is null) return null;
            
            var newApplication = new Data.Entities.Application
            {
                ApplicationStatus = ApplicationStatus.InProgress,
                CreatedFromWorkflowId = latestWorkflow.Id,
                CreatedBy = request.UserReference,
                CreatedAt = SystemTime.UtcNow()
            };

            _dataContext.Applications.Add(newApplication);

            await _dataContext.SaveChangesAsync(cancellationToken);
            return newApplication;
        }

        private async Task CopyWorkflows(CancellationToken cancellationToken, Data.Entities.Application newApplication)
        {
            var workflowSequences = await _dataContext.WorkflowSequences.Where(seq => seq.WorkflowId == newApplication.CreatedFromWorkflowId).ToListAsync(cancellationToken);
            var newApplicationSequences = workflowSequences.Select(seq => new ApplicationSequence
            {
                ApplicationId = newApplication.Id,
                SequenceId = seq.SequenceId,
                Status = seq.Status,
                IsActive = seq.IsActive
            });
            await _dataContext.ApplicationSequences.AddRangeAsync(newApplicationSequences, cancellationToken);

            var workflowSections = await _dataContext.WorkflowSections
                .Where(sec => sec.WorkflowId == newApplication.CreatedFromWorkflowId)
                .ToListAsync(cancellationToken);

            var newApplicationSections = workflowSections.Select(sec => new ApplicationSection
            {
                ApplicationId = newApplication.Id,
                SequenceId = sec.SequenceId,
                SectionId = sec.SectionId,
                QnAData = sec.QnAData,
                Title = sec.Title,
                LinkTitle = sec.LinkTitle,
                Status = sec.Status,
                DisplayType = sec.DisplayType
            }).ToList();
            
            var assets = await _dataContext.Assets.ToListAsync(cancellationToken: cancellationToken);
            
            foreach (var applicationSection in newApplicationSections)
            {
                var qnADataJson = JsonConvert.SerializeObject(applicationSection.QnAData);
                foreach (var asset in assets)
                {
                    qnADataJson = qnADataJson.Replace(asset.Reference, HttpUtility.JavaScriptStringEncode(asset.Text));
                }

                applicationSection.QnAData = JsonConvert.DeserializeObject<QnAData>(qnADataJson);
            }
            
            await _dataContext.ApplicationSections.AddRangeAsync(newApplicationSections, cancellationToken);
            
            await _dataContext.SaveChangesAsync(cancellationToken);
        }
    }
}