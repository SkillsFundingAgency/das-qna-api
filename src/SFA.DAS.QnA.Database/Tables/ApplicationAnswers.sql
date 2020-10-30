CREATE TABLE [dbo].[ApplicationAnswers] (
	Id BigInt not null Identity(1,1) Constraint PK_ApplicationAnswers Primary Key Clustered
	,ApplicationId UniqueIdentifier not null
	,SectionId UniqueIdentifier not null
	,PageId varchar(255) not null
	,QuestionId varchar(255) not null
	,[Value] varchar(max) null
	,Index IX_ApplicationAnswers_ByPage (ApplicationId, SectionId, PageId)
	,Index IX_ApplicationAnswers_BySection (ApplicationId, SectionId)
	,Index IX_ApplicationAnswers_ByApplication (ApplicationId)
)