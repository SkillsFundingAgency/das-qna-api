using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands.ResetPageAnswers;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.ResetPageAnswersBySequenceSectionNumberHandlerTests
    {
        public class When_page_not_found : ResetPageAnswersBySequenceSectionNumberHandlerTests
        {
            [Test]
            public async Task Then_validation_error_occurs()
            {
                var response = await Handler.Handle(new ResetPageAnswersBySequenceSectionNumberRequest(ApplicationId, SequenceNo, SectionNo, "NOT_FOUND"), CancellationToken.None);

                response.Success.Should().BeFalse();
            }
        }
    }


