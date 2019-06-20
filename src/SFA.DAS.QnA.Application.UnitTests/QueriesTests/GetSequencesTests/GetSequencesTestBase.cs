using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sequences.GetCurrentSequence;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequences;
using SFA.DAS.Qna.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetSequencesTests
{
    [TestFixture]
    public class GetSequencesTestBase
    {
        protected GetSequencesHandler Handler;
        protected Guid ApplicationId;
        
        [SetUp]
        public async Task SetUp()
        {
            var dbContextOptions = new DbContextOptionsBuilder<QnaDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new QnaDataContext(dbContextOptions);

            ApplicationId = Guid.NewGuid();
            
            context.ApplicationSequences.AddRange(new[]
            {
                new ApplicationSequence {ApplicationId = ApplicationId, IsActive = false, SequenceId = 1},
                new ApplicationSequence {ApplicationId = ApplicationId, IsActive = true, SequenceId = 2},
                new ApplicationSequence {ApplicationId = Guid.NewGuid(), IsActive = true, SequenceId = 1}
            });

            await context.SaveChangesAsync();
            
            var mapper = new Mapper(new MapperConfiguration(config => { config.AddMaps(AppDomain.CurrentDomain.GetAssemblies()); }));
            
            Handler = new GetSequencesHandler(context, mapper);
        }
    }
}