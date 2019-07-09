using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.Qna.Api.Types;
using SFA.DAS.Qna.Api.Types.Page;
using SFA.DAS.Qna.Data;
using SFA.DAS.QnA.Data.Entities;
using Workflow = SFA.DAS.QnA.Data.Entities.Workflow;

namespace SFA.DAS.QnA.Application.Commands.StartApplication
{
    public class StartApplicationHandler : IRequestHandler<StartApplicationRequest, HandlerResponse<StartApplicationResponse>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IApplicationDataValidator _applicationDataValidator;
        private bool applicationDataIsInvalid;

        public StartApplicationHandler(QnaDataContext dataContext, IApplicationDataValidator applicationDataValidator)
        {
            _dataContext = dataContext;
            _applicationDataValidator = applicationDataValidator;
        }

        public async Task<HandlerResponse<StartApplicationResponse>> Handle(StartApplicationRequest request, CancellationToken cancellationToken)
        {
            var latestWorkflow = await _dataContext.Workflows.SingleOrDefaultAsync(w => w.Type == request.WorkflowType && w.Status == "Live", cancellationToken);
            if (latestWorkflow is null) return null;

            var project = await _dataContext.Projects.SingleOrDefaultAsync(p => p.Id == latestWorkflow.ProjectId, cancellationToken);
            try
            {
                applicationDataIsInvalid = !_applicationDataValidator.IsValid(project.ApplicationDataSchema, request.ApplicationData);
            }
            catch (JsonReaderException)
            {
                return new HandlerResponse<StartApplicationResponse>(false, $"Supplied ApplicationData is not valid JSON.");
            }
            
            if (applicationDataIsInvalid)
            {
                return new HandlerResponse<StartApplicationResponse>(false, $"Supplied ApplicationData is not valid using Project's Schema.");
            }
            
            var newApplication = await CreateNewApplication(request, latestWorkflow, cancellationToken, request.ApplicationData);

            if (newApplication is null) return new HandlerResponse<StartApplicationResponse>(false, $"WorkflowType '{request.WorkflowType}' does not exist.");

            await CopyWorkflows(cancellationToken, newApplication);

            return new HandlerResponse<StartApplicationResponse>(new StartApplicationResponse {ApplicationId = newApplication.Id});
        }

        private async Task<Data.Entities.Application> CreateNewApplication(StartApplicationRequest request, Workflow latestWorkflow, CancellationToken cancellationToken, string applicationData)
        {
            var newApplication = new Data.Entities.Application
            {
                ApplicationStatus = ApplicationStatus.InProgress,
                WorkflowId = latestWorkflow.Id,
                Reference = request.UserReference,
                CreatedAt = SystemTime.UtcNow(),
                ApplicationData = applicationData
            };

            _dataContext.Applications.Add(newApplication);

            await _dataContext.SaveChangesAsync(cancellationToken);
            return newApplication;
        }

        private async Task CopyWorkflows(CancellationToken cancellationToken, Data.Entities.Application newApplication)
        {
            var workflowSequences = await _dataContext.WorkflowSequences.Where(seq => seq.WorkflowId == newApplication.WorkflowId).ToListAsync(cancellationToken);

            var groupedSequences = workflowSequences.GroupBy(seq => new {seq.SequenceNo, seq.IsActive}).ToList();

            var newApplicationSequences = groupedSequences.Select(seq => new ApplicationSequence
            {
                ApplicationId = newApplication.Id,
                SequenceNo = seq.Key.SequenceNo,
                Status = "Draft",
                IsActive = seq.Key.IsActive
            }).ToList();

            await _dataContext.ApplicationSequences.AddRangeAsync(newApplicationSequences, cancellationToken);


            await _dataContext.SaveChangesAsync(cancellationToken);

            var sectionIds = groupedSequences.SelectMany(seq => seq).Select(seq => seq.SectionId).ToList();

            var workflowSections = await _dataContext.WorkflowSections
                .Where(sec => sectionIds.Contains(sec.Id)).ToListAsync(cancellationToken: cancellationToken);

            var newApplicationSections = new List<ApplicationSection>();
            foreach (var sequence in groupedSequences)
            {
                var applicationSequence = newApplicationSequences.Single(appSeq => appSeq.SequenceNo == sequence.Key.SequenceNo);

                foreach (var sectionDetails in sequence)
                {
                    var workflowSection = workflowSections.Single(wSec => wSec.Id == sectionDetails.SectionId);

                    var newSection = new ApplicationSection
                    {
                        SequenceId = applicationSequence.Id,
                        Title = workflowSection.Title,
                        Status = workflowSection.Status,
                        LinkTitle = workflowSection.LinkTitle,
                        ApplicationId = newApplication.Id,
                        DisplayType = workflowSection.DisplayType,
                        QnAData = workflowSection.QnAData,
                        SectionNo = sectionDetails.SectionNo,
                        SequenceNo = sectionDetails.SequenceNo
                    };

                    newApplicationSections.Add(newSection);
                }
            }

            await _dataContext.ApplicationSections.AddRangeAsync(newApplicationSections, cancellationToken);

            await _dataContext.SaveChangesAsync(cancellationToken);
            
            foreach (var applicationSection in newApplicationSections)
            {
                foreach (var page in applicationSection.QnAData.Pages)
                {
                    page.SectionId = applicationSection.Id.ToString();
                    page.SequenceId = applicationSection.SequenceId.ToString();
                }
            }

            await _dataContext.SaveChangesAsync(cancellationToken);
        }
    }
}