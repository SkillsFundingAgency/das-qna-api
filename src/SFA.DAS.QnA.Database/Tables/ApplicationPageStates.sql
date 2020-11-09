CREATE TABLE [dbo].[ApplicationPageStates]
(
	[Id] BigInt NOT NULL Identity(1,1) Constraint PK_ApplicationPageStates PRIMARY KEY
	,ApplicationId uniqueidentifier not null
	,SectionId uniqueidentifier not null
	,PageId varchar(255) not null
	,Complete bit not null
	,Active bit not null
	,NotRequired bit not null
	,Index IX_ApplicationPageStates_ByApplicationId (ApplicationId)
	,Index IX_ApplicationPageStates_BySectionId (ApplicationId, SectionId)
	,Index IX_ApplicationPageStates_ByPageId (ApplicationId, SectionId, PageId)
)
