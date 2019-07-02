using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.Qna.Api.Types.Page;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.Qna.Data
{
    public class QnaDataContext : DbContext
    {
        public QnaDataContext(DbContextOptions<QnaDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationSection>()
                .Property(c => c.QnAData)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v,
                        new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}),
                    v => JsonConvert.DeserializeObject<QnAData>(v,
                        new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));
            
            modelBuilder.Entity<WorkflowSection>()
                .Property(c => c.QnAData)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v,
                        new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}),
                    v => JsonConvert.DeserializeObject<QnAData>(v,
                        new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));
        }
        
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowSection> WorkflowSections { get; set; }
        public DbSet<WorkflowSequence> WorkflowSequences { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationSequence> ApplicationSequences { get; set; }
        public DbSet<ApplicationSection> ApplicationSections { get; set; }
    }
}