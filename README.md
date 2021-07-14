# Digital Apprenticeships Service

##  Question and Answer API
Licensed under the [MIT license](https://github.com/SkillsFundingAgency/das-qna-api/blob/master/LICENSE)

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)|QnA API|
| Info | A Web API service which allows question sets to be organised and presented and their answers collected by exposing HTTP REST end points.  |
| Build | [![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/Endpoint%20Assessment%20Organisation/das-qna-api?branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=1654&branchName=master) |
| Web  | https://localhost:5555/swagger/index.html  |

|               | <div style="width:500px"></div>              |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)| QnA API Client |
| Info  | A .Net Core client library for QnA API HTTP REST end points |
| Build  | [![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.QnA.Api.Client)](https://www.nuget.org/packages/SFA.DAS.QnA.Api.Client)  |

|               | <div style="width:500px"></div>              |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)| QnA API Types |
| Info  | Common types to interact with the QnA Api  |
| Build  | [![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.QnA.Api.Types)](https://www.nuget.org/packages/SFA.DAS.QnA.Api.Types)  |

|               | <div style="width:500px"></div>              |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)| QnA API Views |
| Info  | Asp.Net Core Views using [GOV UK Design System](https://design-system.service.gov.uk/get-started/)  |
| Build  | [![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.QnA.Api.Views)](https://www.nuget.org/packages/SFA.DAS.QnA.Api.Views)  |


See [Support Site](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/1686274228/QnA+API+-+Developer+Overview) for EFSA developer details.

### Developer Setup

#### Requirements
- Install [.NET Core 2.2 SDK](https://www.microsoft.com/net/download)
- Install [Visual Studio 2019](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
- Install [SQL Server 2017 Developer Edition](https://go.microsoft.com/fwlink/?linkid=853016)
- Install [SQL Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- Install [Azure Storage Emulator](https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409) (Make sure you are on atleast v5.3)
- Install [Azure Storage Explorer](http://storageexplorer.com/) 
- Administrator Access

##### Alternatives (caveat emptor)

- [Visual Studio Code](https://code.visualstudio.com/download)

#### Setup

- Clone this repository
- Open Visual Studio as an administrator

##### Publish Database

- Build the solution SFA.DAS.QnA.sln
- Either use Visual Studio's `Publish Database` tool to publish the database project SFA.DAS.QnA.Database to name {{database name}} on {{local instance name}}
- To include the latest question sets when publishing a database to your local SQL Server, you need to ensure that `ProjectPath` variable contains the full path in the format  `{{drive}}:\{{project-folders}}\das-qna-api\src\SFA.DAS.QnA.Database\` 
  - eg. `C:\Source\Repos\SFA\das-qna-api\src\SFA.DAS.QnA.Database\`

    **Note**: The required trailing backslash on the path in the example above.

or

- Create a database manually named {{database name}} on {{local instance name}} and run each of the `.sql` scripts in the SFA.DAS.QnA.Database project.

##### Config

- Get the das-qna-api configuration json file from [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-qna-api/SFA.DAS.QnA.Api.json); which is a non-public repository.
- Create a Configuration table in your (Development) local Azure Storage account.
- Add a row to the Configuration table with fields: PartitionKey: LOCAL, RowKey: SFA.DAS.QnA.Api_1.0, Data: {{The contents of the local config json file}}.
- Update Configuration SFA.DAS.QnA.API_1.0, Data { "SqlConnectionstring":"Server={{local instance name}};Initial Catalog={{database name}};Trusted_Connection=True;" }

##### Run the solution

JSON configuration was created to work with dotnet run.

- Navigate to src/SFA.DAS.QnA.API/
- run `dotnet restore`
- run `dotnet run`

or

- Set SFA.DAS.QnA.API as the startup project
- Running the solution will launch the API in your browser
	
#### To run a local copy you may also require 
To create a JSON structure required to author updates and create new question sets:
- [Config Tool](https://github.com/SkillsFundingAgency/das-qna-config)

To view how the question sets will be presented when integrated into a client application using [GOV UK Design System](https://design-system.service.gov.uk/get-started/):
- [Config Preview](https://github.com/SkillsFundingAgency/das-qna-config-preview)

### Important Concepts

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

- AzureActiveDirectoryConfiguration
  - Azure Managed Identity authentication
	
- FileStorageConfig
	- Information relating to storage of files
	
- QnAConfig
	- Connection string for the QnA Database

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
	
### QnA Structure

A Workflow is made up of multiple Sequences. Each Sequence may have multiple Sections.

The QnAData within each Section defines the flow and logic. There are multiple Pages consisting of Questions and relevant Answers. Depending on Answers provided, it will decide which Pages are activate.

#### Question

Questions should an unique Id, an optional QuestionTag and a particular Input type. If the Input has different Options to select from, you may want to include FutherQuestions to allow a related/follow up Question (i.e. Yes selected, so now need to provide more details).

#### Question Type
Most are self explanatory and in most cases have built in validators.

- TabularData is a JSON structure that represents a Table storage format (i.e. header and rows)

- FileUpload is for uploading files. Note that this type should POST the Answer/Files to the File Upload endpoint and not the Save Answers end point

- ComplexRadio & ComplexCheckboxList enables the use of FutherQuestions based on an Option being selected

#### Next Conditions

These are the primary mechanism to dictate logic flow within a Section. Should a QuestionId or QuestionTag match the Next condition then that specified Page will be made active. Any conditions that not match will make that specified Page inactive.

#### Next Actions

NextPage - Go to the next page

Any other value can be specified. This is intended to allows the calling application to decide the logic. Such actions could be ReturnToSection or TaskList

#### NotRequiredConditions

There are situations where Next Conditions cannot control the Page flow (i.e. determining the entry point to the first Page within a Section based on a particular value).

NotRequiredConditions is a way for QnA Api to remove Pages from the response payload back to the user.
