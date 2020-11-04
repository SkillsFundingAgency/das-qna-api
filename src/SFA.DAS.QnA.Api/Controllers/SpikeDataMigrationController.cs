using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("migration")]
    [Produces("application/json")]
    [ApiController]
    public class SpikeDataMigrationController : ControllerBase
    {
        private readonly QnaDataContext _dataContext;
        private readonly ILogger<ApplicationController> _logger;

        public SpikeDataMigrationController(QnaDataContext dataContext, ILogger<ApplicationController> logger)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("answers")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> MigrateAnswers()
        {
            return Ok("");
        }

        [HttpPost("answers/{applicationId}")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> MigrateAnswers(Guid applicationId)
        {
            var application = await _dataContext.Applications.FirstOrDefaultAsync(a => a.Id == applicationId);
            if (application == null)
                return NotFound();

            var workflow = await _dataContext.Workflows.FirstOrDefaultAsync(wf => wf.Id == application.WorkflowId);
            if (workflow == null)
                return NotFound($"Workflow {application.WorkflowId} not found");

            //Get the sequences and sections for the workflow
            var workflowSequences = await _dataContext.WorkflowSequences.Where(wfs => wfs.WorkflowId == workflow.Id)
                .Join(_dataContext.WorkflowSections, sequence => sequence.SectionId, section => section.Id, (sequence, section) => new { Sequence = sequence, Section = section })
                .ToListAsync();

            //get the application sections
            var applicationSections = await _dataContext.ApplicationSections
                .Where(section => section.ApplicationId == applicationId)
                .ToListAsync();

            //match workflow sections to application sections via sequence no and section no
            foreach (var workflowSequence in workflowSequences)
            {
                //get the matching application section
                var applicationSection = applicationSections.FirstOrDefault(asec =>
                    asec.SequenceNo == workflowSequence.Sequence.SequenceNo &&
                    asec.SectionNo == workflowSequence.Sequence.SectionNo);
                if (applicationSection == null)
                    return NotFound(
                        $"Application section not found for workflow section.  Sequence NO: {workflowSequence.Sequence.SequenceNo}, Section no: {workflowSequence.Sequence.SectionNo}, Sequence id: {workflowSequence.Sequence.Id}");

                if (!workflowSequence.Section.Title.Equals(applicationSection.Title))
                {
                    _logger.LogWarning(
                        $"Application section mismatch with workflow section.  Sequence NO: {workflowSequence.Sequence.SequenceNo}, Section no: {workflowSequence.Sequence.SectionNo}, Sequence id: {workflowSequence.Sequence.Id}");
                    //    continue;
                }


                foreach (var page in applicationSection.QnAData.Pages)
                {
                    var answers = page.PageOfAnswers
                        .SelectMany(pageOfAnswers => pageOfAnswers.Answers)
                        .Select(answer => new ApplicationAnswer
                        {
                            ApplicationId = applicationId,
                            PageId = page.PageId,
                            SectionId = workflowSequence.Section.Id,
                            QuestionId = answer.QuestionId,
                            Value = answer.Value
                        })
                        .ToList();
                    await _dataContext.ApplicationAnswers.AddRangeAsync(answers);
                }
            }

            var deleteSql = $"Delete from ApplicationAnswers where ApplicationId = '{applicationId}'";
            await _dataContext.Database.ExecuteSqlCommandAsync(deleteSql);
            await _dataContext.SaveChangesAsync();
            return Ok("");
        }

        [HttpPost("workflows/{workflowId}")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> MigrateWorkflowPageConfigurations(Guid workFlowId)
        {
            var workflowSequences = await _dataContext.WorkflowSequences
                .Where(wfs => wfs.WorkflowId == workFlowId)
                .ToListAsync();

            var sectionIDs = workflowSequences.Select(seq => seq.SectionId)
                .ToArray();

            var workflowSections = await _dataContext.WorkflowSections
                .Where(sec => sectionIDs.Contains(sec.Id))
                .ToListAsync();

            foreach (var workflowSection in workflowSections)
            {
                workflowSection.ConfigurationData = new QnAData();
                workflowSection.ConfigurationData.Pages = workflowSection.QnAData.Pages.Select(page => new Page
                {
                    SectionId = page.SectionId,
                    Title = page.Title,
                    ActivatedByPageId = page.ActivatedByPageId,
                    AllowMultipleAnswers = page.AllowMultipleAnswers,
                    //Details = page.Details,
                    DisplayType = page.DisplayType,
                    Next = page.Next,
                    NotRequiredConditions = page.NotRequiredConditions,
                    Order = page.Order,
                    PageId = page.PageId,
                    SequenceId = page.SequenceId
                }).ToList();
            }


            await _dataContext.SaveChangesAsync();
            return Ok("");
        }
    }
}