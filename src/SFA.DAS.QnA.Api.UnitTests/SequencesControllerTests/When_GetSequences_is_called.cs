using System.Threading.Tasks;
using MediatR;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;

namespace SFA.DAS.QnA.Api.UnitTests.SequencesControllerTests
{
    [TestFixture]
    public class When_GetSequences_is_called
    {
        [Test]
        public async Task Then_a_list_of_sequences_is_returned()
        {
            var mediator = Substitute.For<IMediator>();
            var controller = new SequencesController(mediator);
            
        }
    }
}