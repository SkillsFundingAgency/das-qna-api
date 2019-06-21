using MediatR;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Controllers;

namespace SFA.DAS.QnA.Api.UnitTests.SequencesControllerTests.GetCurrentSequence
{
    [TestFixture]
    public class GetCurrentSequenceTestBase
    {
        protected SequencesController Controller;
        protected IMediator Mediator;

        [SetUp]
        public void SetUp()
        {
            Mediator = Substitute.For<IMediator>();
            Controller = new SequencesController(Mediator);
        }   
    }
}