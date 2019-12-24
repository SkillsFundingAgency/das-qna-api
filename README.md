# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service

##  Question and Answer API (das-qna-api)
![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/Endpoint%20Assessment%20Organisation/das-qna-api)

## License
Licensed under the [MIT license](https://github.com/SkillsFundingAgency/das-assessor-service-external-apiclient/blob/master/LICENSE)

### Developer Setup

#### Requirements
- [.NET Core 2.2 SDK](https://dotnet.microsoft.com/download/dotnet-core/2.2)
- An Azure Storage account (or the [Storage Emulator](https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409))
- [Azure Storage Explorer](http://storageexplorer.com/)

#### Setup
- Obtain the appropriate JSON configuration file for this application.
- Create a Configuration table in your storage account.
- Add a row to the Configuration table with fields: PartitionKey: LOCAL, RowKey: SFA.DAS.QnA.Api_1.0, Data: {The contents of the JSON Configuration file}.

##### Open the solution
- Navigate to src/SFA.DAS.QnA.Api/
- run `dotnet build`
- run `dotnet test`
- run `dotnet run`

#### Swagger Documentation
- https://localhost:5555/swagger/index.html

#### QnA Config Tool
- https://qna-config.apprenticeships.education.gov.uk/

####  SFA.DAS.AssessorService.QnA
Web API project exposing HTTP REST end points
- Refer to Swagger documentation

#### SFA.DAS.QnA.Api.Types
Common types to interact with the QnA Api
- Published to Nuget: https://www.nuget.org/packages/SFA.DAS.QnA.Api.Types

#### SFA.DAS.QnA.Api.Views
Common views to display questions
- Published to Nuget: https://www.nuget.org/packages/SFA.DAS.QnA.Api.Views

#### SFA.DAS.QnA.Application
Contains all of the application logic to handle requests

- All requests are handled via MediatR
	- https://www.nuget.org/packages/MediatR
	- https://github.com/jbogard/MediatR

- Requests are either:
	- Queries, which will perform Read action on data
	- Commands, which perform Create / Update / Delete action on data
	
- Validators
	- A library of validators that may be used to validate Question input
	- `ValidatorFactory` will load the validators for each Question in the Page
	- Each validator should implement `IValidator`
	- `AnswerValidator` will execute each validator and return any validation errors

#### SFA.DAS.QnA.Configuration
Enables functionality to store and read configuration from Microsoft Azure storage

- AuthenticationConfig
	- JWT authentication
	
- FileStorageConfig
	- Information relating to storage of files
	
- QnAConfig
	- Connection string for the QnA Database

#### SFA.DAS.QnA.Data
Contains all repositories which interact with the QnA Database

#### SFA.DAS.QnA.Database
Database project containing setup in order to the create the QnA Database

- projects
	- A subfolder should be created per Project
	
- projects/{subfolder}/project.json
	- Contains information on how to setup the Workflow information
	
- projects/{subfolder}/ApplicationDataSchema.json
	- Contains JSON schema to validate ApplicationData
	
- projects/{subfolder}/sections
	- Holds QnAData for each WorkflowSection
	
_NOTE:_ To publish the database to your local SQL Server, you need to ensure that `ProjectPath` contains the full path.
- {drive}:\{project-folders}\das-qna-api\src\SFA.DAS.QnA.Database\
- For example: *C:\projects\efsa\das-qna-api\src\SFA.DAS.QnA.Database\*

#### SFA.DAS.QnA.Api.UnitTests
Contains a collection of nUnit tests to verify functionality of QnA API HTTP REST end points

#### SFA.DAS.QnA.Application.UnitTests
Contains a collection of nUnit tests to verify functionality of QnA API application logic

