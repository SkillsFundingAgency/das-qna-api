using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.QnA.Application.Queries.Sequences;
using SFA.DAS.QnA.Application.Queries.Sequences.GetCurrentSequence;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequences;

namespace SFA.DAS.QnA.Api.Controllers
{
    [Route("applications")]
    [Produces("application/json")]
    public class SequencesController : Controller
    {
        private readonly IMediator _mediator;
        
        public SequencesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Returns all of the Sequences for the Application
        /// </summary>
        /// <returns>An array of Sequences</returns>
        /// <response code="200">Returns the Application's Sequences</response>
        /// <response code="204">If there are no Sequences for the given Application Id</response>
        [HttpGet("{applicationId}/sequences")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        public async Task<ActionResult<List<SequenceResponse>>> GetSequences(Guid applicationId)
        {
            var sequences = await _mediator.Send(new GetSequencesRequest(applicationId), CancellationToken.None);
            if (sequences.Count == 0) return NoContent();
            return sequences;
        }
        
        /// <summary>
        /// Returns the Application's currently active Sequence
        /// </summary>
        /// <returns>The active sequence</returns>
        /// <response code="200">Returns the active sequence</response>
        /// <response code="204">If there is no current sequence for the given Application Id</response>
        [HttpGet("{applicationId}/sequences/current")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        public async Task<ActionResult<SequenceResponse>> GetCurrentSequence(Guid applicationId)
        {
            var sequences = await _mediator.Send(new GetCurrentSequenceRequest(applicationId), CancellationToken.None);
            
            return sequences;
        }
    }
}