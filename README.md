#  Question and Answer API
<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status%2FEndpoint%20Assessment%20Organisation%2Fdas-qna-api?repoName=SkillsFundingAgency%2Fdas-qna-api&branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=1654&repoName=SkillsFundingAgency%2Fdas-qna-api&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-qna-api&metric=alert_status)](https://sonarcloud.io/project/overview?id=SkillsFundingAgency_das-qna-api)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

# About

This service is a Web API service which allows question sets to be organised and presented and their answers collected by exposing HTTP REST end points.

Nuget packages that go with this solution include:
* A .Net Core client library for QnA API HTTP REST end points [![QnA API Client](https://buildstats.info/nuget/SFA.DAS.QnA.Api.Client)](https://www.nuget.org/packages/SFA.DAS.QnA.Api.Client)
* Common types to interact with the QnA Api [![QnA API Types](https://buildstats.info/nuget/SFA.DAS.QnA.Api.Types)](https://www.nuget.org/packages/SFA.DAS.QnA.Api.Types)
* Asp.Net Core Views using [GOV UK Design System](https://design-system.service.gov.uk/get-started/) [![QnA API Views](https://buildstats.info/nuget/SFA.DAS.QnA.Api.Views)](https://www.nuget.org/packages/SFA.DAS.QnA.Api.Views)

## 🚀 Installation

### Pre-Requisites

* A clone of this repository
* Azurite or similar local storage emulator
* SQL Database
* Visual studio or similar IDE

### Environment Setup

* Clone this repository
* Open Visual Studio as an administrator
* **Database** - Build the solution `SFA.DAS.QnA.sln`. Either use Visual Studio's `Publish Database` tool to publish the database project `SFA.DAS.QnA.Database` to the database name `{{database name}}` on `{{local instance name}}`.
	* To include the latest question sets when publishing a database to your local SQL Server, you need to ensure that the `ProjectPath` variable contains the full path in the format  `{{drive}}:\{{project-folders}}\das-qna-api\src\SFA.DAS.QnA.Database\` 
	eg. `C:\Source\Repos\SFA\das-qna-api\src\SFA.DAS.QnA.Database\`
	**Note**: The required trailing backslash on the path in the example above.
* **or** create a database manually named `{{database name}}` on `{{local instance name}}` and run each of the `.sql` scripts in the `SFA.DAS.QnA.Database` project.
* **json file** - Get the `das-qna-api` configuration json file from [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-qna-api/SFA.DAS.QnA.Api.json) which is a non-public repository.
* **Azure Table Storage Config** - Add the following to your Azure Table Storage Config.

PartitionKey: LOCAL

RowKey: SFA.DAS.QnA.Api_1.0 

Data: 
```
{{The contents of the local config json file}}
```

* Update Configuration `SFA.DAS.QnA.API_1.0` with data `{ "SqlConnectionstring":"Server={{local instance name}};Initial Catalog={{database name}};Trusted_Connection=True;" }`

### Running

* Set `SFA.DAS.QnA.API` as the startup project
* Running the solution will launch the API.
	
* **NB** - To run a local copy you may also need:
	* To create a JSON structure required to author updates and create new question sets: [Config Tool](https://github.com/SkillsFundingAgency/das-qna-config)
	* To view how the question sets will be presented when integrated into a client application using [GOV UK Design System](https://design-system.service.gov.uk/get-started/) and [Config Preview](https://github.com/SkillsFundingAgency/das-qna-config-preview)

	When launching with the QnA Config Preview solution, the starting web page will display 3 free text input fields. These are
	* Project Type - this should be populated with "roatp"
	* Sequence Number - this should be populated with an integer that is incremented each time you start the journey
	* Application Data.Json - this should be populated with the following seed json:
	```	
	{
	"OrganisationReferenceId": "10088539",
	"OrganisationName": "SAMPLE EDUCATION LIMITED",
	"ApplyProviderRoute": "1",
	"UKPRN": "10088539",
	"UKRLPLegalName": "SAMPLE EDUCATION LIMITED",
	"UKRLPWebsite": "www.sample.education",
	"UKRLPNoWebsite": "",
	"CHManualEntryRequired": "",
	"UKRLPVerificationCompanyNumber": "09950816",
	"UKRLPVerificationCompany": "TRUE
	"CCTrusteeManualEntry": 
	"UKRLPVerificationCharityRegNumber": null,
	"UKRLPVerificationCharity": "",
	"UKRLPVerificationSoleTraderPartnership": "",
	"UKRLPPrimaryVerificationSource": "Companies House",
	"RouteAndOnRoatp": "MainNotOnRoatp",
	"OnROATP": "",
	"COAStage1Application": "TRUE",
	"CompaniesHouseDirectors": "{\"Caption\":\"Company directors\",\"HeadingTitles\":[\"Name\",\"Date of birth\"],\"DataRows\":[{\"Id\":\"_-wFtGEW6y8j9sXdwECdMHXetYM\",\"Columns\":[\"FLINTSTONE, Timothy John\",\"Oct 1964\"]},{\"Id\":\"WHCXVIL2dmyAQmIaVn6p9hHOo_A\",\"Columns\":[\"FLINTSTONE, Mark\",\"Aug 1965\"]},{\"Id\":\"lXX8igijzpyJ9NGW4qvrMJnD_3M\",\"Columns\":[\"FLINTSTONE, Bethany\",\"Oct 1995\"]},{\"Id\":\"gjb5TukqEALaPUAch06snifZ_2Q\",\"Columns\":[\"FLINTSTONE, Thomas George\",\"Nov 1974\"]}]}",
	"CompaniesHousePSCs": "{\"Caption\":\"People with significant control (PSCs)\",\"HeadingTitles\":[\"Name\",\"Date of birth\"],\"DataRows\":[{\"Id\":\"/company/09950816/persons-with-significant-control/individual/iUYwRxO1eMOiTX7tRD9n7pSi5F4\",\"Columns\":[\"Mr Mark Flintstone\",\"Aug 1965\"]}]}",
	"DirectorsPSCsConfirmed": "Y",
	"HasParentCompany": "No",
	"OrganisationTypeMainSupporting": "An Independent Training Provider",
	"OFSFunded": "No",
	"ITTAccredited": "No",
	"OfstedFESkills": "No",
	"FHATurnoverOver75m": "No",
	"FHATurnover": "3629927",
	"FHADepreciation": "146099",
	"FHAProfitLoss": "77825",
	"FHADividends": "0",
	"FHAAssets": "640742",
	"FHALiabilities": "559888",
	"FHABorrowings": "586798",
	"FHAShareholderFunds": "3390",
	"FHAIntangibleAssets": "0",
	"FHAAverageNumberofFTEEmployees": "63",
	"FHAHas12MonthsStatements": "Yes",
	"FHAAccountingPeriod": "12",
	"CCOrganisationComposition": "No",
	"CC-Organisation-RoTO": "No",
	"CC-Organisation-RoTO-Information": null,
	"FHAAccountingReferenceDate": "31,08,2025",
	"AddManagementHierarchy": "{\"Caption\":null,\"HeadingTitles\":[\"First Name\",\"Last Name\",\"Job role\",\"Years in role\",\"Months in role\",\"Part of another organisation\",\"Organisation details\",\"Month\",\"Year\",\"Email\",\"Contact number\"],\"DataRows\":[{\"Id\":\"e6385e66-50a1-4225-9f23-40216415d419\",\"Columns\":[\"Mark\",\"Flintstone\",\"CEO \",\"14\",\"0\",\"No\",\"\",\"08\",\"1965\",\"mark@sample.com\",\"01179428429\"]},{\"Id\":\"bbcbc81e-e138-4edd-aec4-15283b4a040a\",\"Columns\":[\"Tim\",\"Flintstone\",\"Chief Financial Officer \",\"1\",\"0\",\"No\",\"\",\"10\",\"1964\",\"tim.flintstone@sample.com\",\"01179428429\"]},{\"Id\":\"3da75bde-a872-4d9d-bb55-cce50814763f\",\"Columns\":[\"Beth\",\"Flintstone \",\"Operations Director \",\"1\",\"9\",\"No\",\"\",\"10\",\"1995\",\"beth@sample.com\",\"01179428429\"]},{\"Id\":\"74ec14d2-b754-4709-b1ff-3cf8373807d2\",\"Columns\":[\"Joanna\",\"Flintstone\",\"Head of Apprenticeships\",\"0\",\"6\",\"No\",\"\",\"03\",\"1971\",\"jo.flintstone@sample.com\",\"01179428429\"]}]}",
	"DeliveringTeamResponsible": "Yes",
	"SectorsChosen": "Creative and design",
	"DATCreativeWhatStandardsOffered": "Photographer \r\nContent Creator ",
	"DATCreativeHowManyStarts": "40",
	"DATCreativeHowManyEmployees": "2",
	"DATCreativeFirstName": "Freya",
	"DATCreativeLastName": "Flintstone",
	"FinishPermissionPersonalDetails": "Yes",
	"FinishAccuratePersonalDetails": "Yes",
	"FinishPermissionSubmitApp": "Yes",
	"FinishCommercialConfidence": "Yes",
	"COAPart2MainEmployer": "Yes"
	}
	```


## QnA Structure

A Workflow is made up of multiple sequences. Each sequence may have multiple sections.

The QnAData within each section defines the flow and logic. There are multiple pages consisting of questions and relevant answers. Depending on the answers provided, it will decide which pages are active.

##### Question

Questions should have a unique Id, an optional `QuestionTag` and a particular input type. If the input has different options to select from, you may want to include `FutherQuestions` to allow a related/follow up question (i.e. Yes selected, so now need to provide more details).

##### Question Type

Most types are self explanatory and in most cases have they will have built-in validators.

* `TabularData` is a JSON structure that represents a table storage format (i.e. header and rows).
* `FileUpload` is for uploading files. Note that this type should POST the Answer/Files to the File Upload endpoint and not the Save Answers end point.
* `ComplexRadio` & `ComplexCheckboxList` enable the use of `FutherQuestions` based on the option being selected.

##### Next Conditions

These are the primary mechanism to dictate logic flow within a section. Should a `QuestionId` or `QuestionTag` match the `Next` condition then that specified page will be made active. Any conditions that do not match will make that specified page inactive.

##### Next Actions

`NextPage` - Go to the next page

Any other value can be specified. This is intended to allow the calling application to decide the logic. Other actions could be `ReturnToSection` or `TaskList`.

##### NotRequired Conditions

There are situations where `Next` Conditions cannot control the page flow (i.e. determining the entry point to the first page within a section based on a particular value).

`NotRequiredConditions` is a way for `QnA Api` to remove pages from the response payload back to the user.


## Technologies

* .Net 10
* Azure Table Storage
* NUnit
* Moq
* FluentAssertions