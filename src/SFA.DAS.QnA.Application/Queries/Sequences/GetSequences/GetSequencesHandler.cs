using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.Queries.Sequences.GetSequences
{
    public class GetSequencesHandler : IRequestHandler<GetSequencesRequest, HandlerResponse<List<Sequence>>>
    {
        private readonly QnaDataContext _dataContext;
        private readonly IMapper _mapper;

        public GetSequencesHandler(QnaDataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        //public async Task<HandlerResponse<List<Sequence>>> Handle(GetSequencesRequest request, CancellationToken cancellationToken)
        //{
        //    var sequences = await _dataContext.ApplicationSequences.AsNoTracking()
        //        .Where(seq => seq.ApplicationId == request.ApplicationId)
        //        .ToListAsync(cancellationToken: cancellationToken);

        //    if (!sequences.Any())
        //    {
        //        return new HandlerResponse<List<Sequence>>(false, "Application does not exist");
        //    }

        //    var mappedSequences = _mapper.Map<List<Sequence>>(sequences);

        //    return new HandlerResponse<List<Sequence>>(mappedSequences);
        //}

        public async Task<HandlerResponse<List<Sequence>>> Handle(GetSequencesRequest request, CancellationToken cancellationToken)
        {
            var application = _dataContext.Applications.Single(x => x.Id == request.ApplicationId);

            var workflowSequences = _dataContext.WorkflowSequences.Where(x => x.WorkflowId == application.WorkflowId);
            //var sectionIds = workflowSequences.Select(x => x.SectionId).ToList();
            //var workflowSections = await _dataContext.WorkflowSections.Where(x => sectionIds.Contains(x.Id)).ToListAsync(cancellationToken);

            var groupedSequences = workflowSequences.GroupBy(seq => new { seq.SequenceNo, seq.IsActive }).ToList();

            var newApplicationSequences = groupedSequences.Select(seq => new ApplicationSequence
            {
                Id = workflowSequences.Single(x => x.SequenceNo == seq.Key.SequenceNo && x.IsActive == seq.Key.IsActive).Id,
                ApplicationId = request.ApplicationId,
                SequenceNo = seq.Key.SequenceNo,
                IsActive = seq.Key.IsActive
            }).ToList();


            var mappedSequences = _mapper.Map<List<Sequence>>(newApplicationSequences);
            return new HandlerResponse<List<Sequence>>(mappedSequences);
        }
    }
}