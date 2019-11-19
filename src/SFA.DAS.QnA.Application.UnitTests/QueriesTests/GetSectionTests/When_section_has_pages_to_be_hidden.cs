using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.QnA.Application.Queries.Sections.GetSection;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetSectionTests
{
    [TestFixture]
    public class When_section_has_pages_to_be_hidden
    {
        [TestCase("HEI", null, 2)]
        [TestCase("TEST", null, 3)]
        [TestCase(null, "HEI", 3)]
        [TestCase(null, "TEST", 2)]
        [TestCase("HEI", "HEI", 2)]
        public async Task Then_pages_are_not_returned_in_section(string oneOf, string notOneOf, int expectedPageCount)
        {
            var sectionId = Guid.NewGuid();
            var applicationId = Guid.NewGuid();
            var dataContext = DataContextHelpers.GetInMemoryDataContext();

            var applicationData = new { OrganisationType = "HEI" };

            dataContext.Applications.Add(new Data.Entities.Application()
            {
                Id = applicationId,
                ApplicationData = JsonConvert.SerializeObject(applicationData)
            });

            var isOneOfs = oneOf == null ? null : new string[] { oneOf };
            var isNotOneOf = notOneOf == null ? null : new string[] { notOneOf };

            dataContext.ApplicationSections.Add(new ApplicationSection()
            {
                Id = sectionId,
                ApplicationId = applicationId,
                QnAData = new QnAData(){Pages = new List<Page>()
                {
                    new Page() {PageId = "1", Active = true},
                    new Page() {PageId = "2", NotRequiredConditions = new List<NotRequiredCondition>(){new NotRequiredCondition(){Field = "OrganisationType", IsOneOf = isOneOfs, IsNotOneOf = isNotOneOf}}, Active = true},
                    new Page() {PageId = "3", Active = true}
                }}
            });
            
            dataContext.SaveChanges();
            
            var mapperConfig = new MapperConfiguration(options => { options.CreateMap<ApplicationSection, Section>(); });

            var handler = new GetSectionHandler(dataContext, mapperConfig.CreateMapper());

            var section = await handler.Handle(new GetSectionRequest(applicationId, sectionId), CancellationToken.None);

            section.Value.QnAData.Pages.Count.Should().Be(expectedPageCount);
        }
    }
}