using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowSection> WorkflowSections { get; set; }
        public DbSet<WorkflowSequence> WorkflowSequences { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationSequence> ApplicationSequences { get; set; }
        public DbSet<ApplicationSection> ApplicationSections { get; set; }
    }
    
    public class QnAData
    {
        public bool? RequestedFeedbackAnswered { get; set; }
        public List<Page> Pages { get; set; }
        public FinancialApplicationGrade FinancialApplicationGrade { get; set; }
    }
    
    public class FinancialApplicationGrade
    {
        public string SelectedGrade { get; set; }
        public string InadequateMoreInformation { get; set; }
        public string GradedBy { get; set; }
        public DateTime GradedDateTime { get; set; }
        public DateTime? FinancialDueDate { get; set; }
    }
    
    public class PageOfAnswers
    {
        public Guid Id { get; set; }
        public List<Answer> Answers { get; set; }
    }
    
    public class Answer
    {
        public string QuestionId { get; set; }
        public string Value { get; set; }
    }
    
    public class Next
    {
        public string Action { get; set; }
        public string ReturnId { get; set; }
        public Condition Condition { get; set; }
        public bool ConditionMet { get; set; }
    }
    
    public class Condition
    {
        public string QuestionId { get; set; }
        public string MustEqual { get; set; }
    }
    
    public class PageDetails
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }
    
    public class Feedback
    {
        public Guid Id { get; set; }
        public string From { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsNew { get; set; }
    }
    
    public class Page
    {
        public string PageId { get; set; }
        public string SequenceId { get; set; }
        public string SectionId { get; set; }
        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public string InfoText { get; set; }
        public List<Question> Questions { get; set; }
        public List<PageOfAnswers> PageOfAnswers { get; set; }
        public List<Next> Next { get; set; }
        public bool Complete { get; set; }
        public bool AllowMultipleAnswers { get; set; }
        public int? Order { get; set; }
        public bool Active { get; set; }
        public List<string> NotRequiredOrgTypes { get; set; }
        public bool NotRequired { get; set; }

        public string BodyText { get; set; }

        public PageDetails Details { get; set; }

        public string DisplayType { get; set; }
        public bool IsQuestionAnswered(string questionId)
        {
            var allAnswers = PageOfAnswers.SelectMany(poa => poa.Answers).ToList();
            return allAnswers.Any(a => a.QuestionId == questionId);
        }

        public List<Feedback> Feedback { get; set; }

        [JsonIgnore]
        public bool HasFeedback => Feedback?.Any() ?? false;

        [JsonIgnore]
        public bool HasNewFeedback => HasFeedback && Feedback.Any(f => f.IsNew || !f.IsCompleted);

        [JsonIgnore]
        public bool AllFeedbackIsCompleted => HasFeedback ? Feedback.All(f => f.IsCompleted) : true;
    }
    
    public class Question
    {
        public string QuestionId { get; set; }
        public string QuestionTag { get; set; }
        public string Label { get; set; }
        public string ShortLabel { get; set; }
        public string InputClasses { get; set; }
        public string QuestionBodyText { get; set; }
        public string Hint { get; set; }
        public Input Input { get; set; }
        public int? Order { get; set; }
        public string Value { get; set; }
        public IEnumerable<dynamic> ErrorMessages { get; set; }
    }
    
    public class Input
    {
        public string Type { get; set; }
        public string InputClasses { get; set; }
        public List<Option> Options { get; set; }
        public List<ValidationDefinition> Validations { get; set; }
        public string DataEndpoint { get; set; }
    }

    public class Option
    {
        public List<Question> FurtherQuestions { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
    }
    
    public class ValidationDefinition
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string ErrorMessage { get; set; }
    }
}