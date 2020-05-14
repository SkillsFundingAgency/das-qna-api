using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sections.GetSections;
using SFA.DAS.QnA.Application.Services;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetSectionsTests
{
    [TestFixture]
    public class GetSectionsTestBase
    {
        protected GetSectionsHandler Handler;
        protected Guid ApplicationId;

        [SetUp]
        public async Task SetUp()
        {
            var dbContextOptions = new DbContextOptionsBuilder<QnaDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new QnaDataContext(dbContextOptions);

            ApplicationId = Guid.NewGuid();

            context.Applications.Add(new Data.Entities.Application { Id = ApplicationId });

            context.ApplicationSequences.AddRange(new[]
            {
                new ApplicationSequence {ApplicationId = ApplicationId, IsActive = false, SequenceNo = 1},
                new ApplicationSequence {ApplicationId = ApplicationId, IsActive = true, SequenceNo = 2}
            });

            context.ApplicationSections.AddRange(new[]
            {
                new ApplicationSection {ApplicationId = ApplicationId, SequenceNo = 1, SectionNo = 1},
                new ApplicationSection {ApplicationId = ApplicationId, SequenceNo = 1, SectionNo = 2},
                new ApplicationSection {ApplicationId = ApplicationId, SequenceNo = 2, SectionNo = 1}
            });

            await context.SaveChangesAsync();

            var mapper = new Mapper(new MapperConfiguration(config => { config.AddMaps(AppDomain.CurrentDomain.GetAssemblies()); }));

            Handler = new GetSectionsHandler(context, mapper, new NotRequiredProcessor());
        }
    }
}
