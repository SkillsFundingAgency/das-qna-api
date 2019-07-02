DELETE FROM workflows where Description='Epao Workflow' and Version='1.0'

GO

if not exists(select * from workflows where Description='Epao Workflow' and Version='1.0')
	INSERT [dbo].[Workflows]
	  ([Id], [Description], [Version], [Type], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeletedAt], [DeletedBy], [ReferenceFormat])
	VALUES
	  (N'83b35024-8aef-440d-8f59-8c1cc459c350', N'EPAO Workflow', N'1.0', N'EPAO', N'Live', CAST(N'2018-12-12T14:41:46.0600000' AS DateTime2), N'Import', NULL, NULL, NULL, NULL,N'AAD')
else
begin
	UPDATE [dbo].[Workflows] SET  [Description] = N'EPAO Workflow', [Version] =  N'1.0', [Type] = N'EPAO', 
	[Status] = N'Live', [CreatedAt] = CAST(N'2018-12-12T14:41:46.0600000' AS DateTime2), 
	[CreatedBy] = N'Import', [UpdatedBy] = N'Import', [UpdatedAt] = GETDATE(), [ReferenceFormat] = N'AAD' where [Id] = '83b35024-8aef-440d-8f59-8c1cc459c350'
end
GO

DELETE WorkflowSections 

INSERT [dbo].[WorkflowSections]
  ([Id], [QnAData], [Title], [LinkTitle], [Status], [DisplayType])
VALUES
  (N'b9c09252-3fee-455f-bc54-12c8788398b7', N'
{
  "Pages": [
    {
      "PageId": "1",
      
      
      "Title": "Trading name",
      "LinkTitle": "Trading name",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-30",
          "QuestionTag": "trading-name",
          "Label": "Does your organisation have a trading name?",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "CD-30.1",
                    "Label": "What is your trading name?",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "text",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter a trading name"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if your organisation has a trading name"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "2",
          "Condition": {
            "QuestionId": "CD-30",
            "MustEqual": "Yes"
          },
          "ConditionMet": false
        },
        {
          "Action": "NextPage",
          "ReturnId": "3",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "2",
      
      
      "Title": "Trading name",
      "LinkTitle": "Name to use on the register",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-01",
          "QuestionTag": "use-trading-name",
          "Label": "Do you want to use your trading name on the register?",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "Radio",
            "Options": [
              {
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if you want to use your trading name on the register"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "3",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": false,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": "",
      "ActivatedByPageId": "2"
    },
    {
      "PageId": "3",
      
      
      "Title": "Enter contact details",
      "LinkTitle": "Contact details",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-02",
          "QuestionTag": "contact-given-name",
          "Label": "Full name",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter given name"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-02_1",
          "QuestionTag": "contact-family-name",
          "Label": "SQ-1-SE-1-PG-3-CD-02_1-46",
          "ShortLabel": "SQ-1-SE-1-PG-3-CD-02_1-47",
          "QuestionBodyText": "SQ-1-SE-1-PG-3-CD-02_1-48",
          "Hint": "SQ-1-SE-1-PG-3-CD-02_1-45",
          "Input": {
            "Type": "text",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter family name"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-03_1",
          "QuestionTag": "contact-address1",
          "Label": "Building and street",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter building and street"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-03_2",
          "QuestionTag": "contact-address2",
          "Label": "",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "Validations": []
          }
        },
        {
          "QuestionId": "CD-03_3",
          "QuestionTag": "contact-address3",
          "Label": "Town or city",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "InputClasses": "govuk-!-width-two-thirds",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter town or city"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-03_4",
          "QuestionTag": "contact-address4",
          "Label": "County",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "InputClasses": "govuk-!-width-two-thirds",
            "Validations": []
          }
        },
        {
          "QuestionId": "CD-04",
          "QuestionTag": "contact-postcode",
          "Label": "Postcode",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "Postcode",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter postcode"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-05",
          "QuestionTag": "contact-email",
          "Label": "Email address",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "Email",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter email address"
              },
              {
                "Name": "EmailAddressIsValid",
                "ErrorMessage": "Enter a valid email address"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-06",
          "QuestionTag": "contact-phone-number",
          "Label": "Telephone",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "InputClasses": "govuk-input--width-20",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter telephone number"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "4",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": "<p class=\"govuk-body\">This information will be published on the Register of end point assessment organisations and will be made available to the public.</p>"
    },
    {
      "PageId": "4",
      
      
      "Title": "Who should we send the contract notice to?",
      "LinkTitle": "Contract notice contact details",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-07",
          "Label": "Full name",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter given name"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-07_1",
          "Label": "SQ-1-SE-1-PG-4-CD-07_1-70",
          "ShortLabel": "SQ-1-SE-1-PG-4-CD-07_1-71",
          "QuestionBodyText": "SQ-1-SE-1-PG-4-CD-07_1-72",
          "Hint": "SQ-1-SE-1-PG-4-CD-07_1-69",
          "Input": {
            "Type": "text",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter family name"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-08_1",
          "Label": "Building and street",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter building and street"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-08_2",
          "Label": "",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "Validations": []
          }
        },
        {
          "QuestionId": "CD-08_3",
          "Label": "Town and city",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "InputClasses": "govuk-!-width-two-thirds",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter town or city"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-08_4",
          "Label": "County",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "InputClasses": "govuk-!-width-two-thirds",
            "Validations": []
          }
        },
        {
          "QuestionId": "CD-09",
          "Label": "Postcode",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "Postcode",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter postcode"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-10",
          "Label": "Email address",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "Email",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter email address"
              },
              {
                "Name": "EmailAddressIsValid",
                "ErrorMessage": "Enter a valid email address"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-11",
          "Label": "Telephone",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "InputClasses": "govuk-input--width-20",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter telephone number"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "5",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "5",
      
      
      "Title": "UK provider registration number (UKPRN)",
      "LinkTitle": "UK provider registration number (UKPRN)",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-12",
          "QuestionTag": "company-ukprn",
          "Label": "Do you have a UK provider registration number (UKPRN)?",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "CD-12.1",
                    "Label": "Provide your UKPRN",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "number",
                      "InputClasses": "govuk-input--width-10",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter your UKPRN"
                        },
                        {
                          "Name": "Regex",
                          "Value": "^[0-9]{8}$",
                          "ErrorMessage": "Enter your UKPRN (must be 8 digits)"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if you have a UKPRN"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "6",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "6",
      
      
      "Title": "Who has responsibility for the overall executive management of your organisation?",
      "LinkTitle": "Overall executive management",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-13",
          "Label": "Full name",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter name of person who has overall control"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-14",
          "Label": "Do they hold any other positions or directorships of other organisations?",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "CD-14.1",
                    "Label": "Provide details of other positions or directorships ",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Textarea",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter other organisation details"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if they hold any other positions or directorships of other organisations"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "7",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "7",
      
      
      "Title": "Ofqual recognition number",
      "LinkTitle": "Ofqual recognition number",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-15",
          "Label": "Do you have an Ofqual recognition number?",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "CD-15.1",
                    "Label": "Provide us with your Ofqual recognition number",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "text",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter Ofqual recognition number"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if you have an Ofqual recognition number"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "8",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "8",
      
      
      "Title": "Trading status",
      "LinkTitle": "Trading status",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-16",
          "Label": "What''s your trading status?",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "Value": "Public limited company",
                "Label": "Public limited company"
              },
              {
                "Value": "Limited company",
                "Label": "Limited company"
              },
              {
                "Value": "Limited liability partnership",
                "Label": "Limited liability partnership"
              },
              {
                "Value": "Other partnership",
                "Label": "Other partnership"
              },
              {
                "Value": "Sole trader",
                "Label": "Sole trader"
              },
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "CD-16.1",
                    "Label": "Describe your trading status",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "text",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter your trading status"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Other",
                "Label": "Other"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select your trading status"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "9",
          "Condition": {
            "QuestionId": "CD-16",
            "MustEqual": "Public limited company"
          },
          "ConditionMet": false
        },
        {
          "Action": "NextPage",
          "ReturnId": "9",
          "Condition": {
            "QuestionId": "CD-16",
            "MustEqual": "Limited company"
          },
          "ConditionMet": false
        },
        {
          "Action": "NextPage",
          "ReturnId": "9",
          "Condition": {
            "QuestionId": "CD-16",
            "MustEqual": "Limited liability partnership"
          },
          "ConditionMet": false
        },
        {
          "Action": "NextPage",
          "ReturnId": "11",
          "Condition": {
            "QuestionId": "CD-16",
            "MustEqual": "Other partnership"
          },
          "ConditionMet": false
        },
        {
          "Action": "NextPage",
          "ReturnId": "11",
          "Condition": {
            "QuestionId": "CD-16",
            "MustEqual": "Sole trader"
          },
          "ConditionMet": false
        },
        {
          "Action": "NextPage",
          "ReturnId": "11",
          "Condition": {
            "QuestionId": "CD-16",
            "MustEqual": "Other"
          },
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "9",
      
      
      "Title": "Company number",
      "LinkTitle": "Company number",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-17",
          "QuestionTag": "company-number",
          "Label": "Do you have a company number?",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "CD-17.1",
                    "Label": "What is your number",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "text",
                      "InputClasses": "govuk-input--width-10",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter your company number"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if you have a company number"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "10",
          "Condition": {
            "QuestionId": "CD-17",
            "MustEqual": "No"
          },
          "ConditionMet": false
        },
        {
          "Action": "NextPage",
          "ReturnId": "11",
          "Condition": {
            "QuestionId": "CD-17",
            "MustEqual": "Yes"
          },
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "10",
      
      
      "Title": "Part of a group of companies",
      "LinkTitle": "Part of a group of companies",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-18",
          "Label": "Is your parent company registered overseas?",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "CD-18.1",
                    "Label": "Which country?",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "text",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter the country your parent company is registered"
                        }
                      ]
                    }
                  },
                  {
                    "QuestionId": "CD-18.2",
                    "Label": "Registration number",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "text",
                      "InputClasses": "govuk-input--width-10",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter the parent company registration number"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if your parent company registered overseas"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "11",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "11",
      
      
      "Title": "Director details",
      "LinkTitle": "Directors",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-19",
          "Label": "Full name",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "Text",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter a name"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-20",
          "Label": "Date of birth",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "Date",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter a date"
              },
              {
                "Name": "Date",
                "Value": "",
                "ErrorMessage": "Date must be correct"
              },
              {
                "Name": "DateNotInFuture",
                "Value": "",
                "ErrorMessage": "Date cannot be in the future"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-21",
          "Label": "How many shares does the director hold?",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "number",
            "InputClasses": "govuk-input--width-5",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter number of shares"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "12",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": true,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "12",
      
      
      "Title": "Director data",
      "LinkTitle": "Directors data",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-22",
          "Label": "Directors data",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "Radio",
            "Options": [
              {
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if a director, or any person with significant control of your organisation have had any of the listed issues"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "13",
          "Condition": {
            "QuestionId": "CD-22",
            "MustEqual": "Yes"
          },
          "ConditionMet": false
        },
        {
          "Action": "NextPage",
          "ReturnId": "14",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": "<p class=\"govuk-body\">Has any director, or any other person with significant control of your organisation, had one or more of the following?</p><ul class=\"govuk-list govuk-list--bullet\"><li>Un-discharged bankruptcy</li><li>Composition with creditors</li><li>Any form of dispute</li></ul>"
    },
    {
      "PageId": "13",
      
      
      "Title": "Further detail of incident",
      "LinkTitle": "Further detail of incident",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-23",
          "Label": "Date of incident",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "Date",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter a date"
              },
              {
                "Name": "Date",
                "Value": "",
                "ErrorMessage": "Date must be correct"
              },
              {
                "Name": "DateNotInFuture",
                "Value": "",
                "ErrorMessage": "Date cannot be in the future"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-24",
          "Label": "Brief summary",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Field must not be empty"
              }
            ]
          }
        },
        {
          "QuestionId": "CD-25",
          "Label": "Any outstanding court action or legal proceedings",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Field must not be empty"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "14",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": true,
      "Active": false,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": "",
      "ActivatedByPageId": "12"
    },
    {
      "PageId": "14",
      
      
      "Title": "Registered charity",
      "LinkTitle": "Registered charity",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-26",
          "QuestionTag": "charity-number",
          "Label": "Is your organisation a registered charity?",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "CD-26.1",
                    "Label": "What is the registered charity number?",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "text",
                      "InputClasses": "govuk-input--width-10",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter registered charity number"
                        },
                        {
                          "Name": "RegisteredCharityNumber",
                          "ErrorMessage": "Enter valid registered charity number"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if your organisation is a registered charity"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "15",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "15",
      
      
      "Title": "Register of removed trustees",
      "LinkTitle": "Register of removed trustees",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "CD-27",
          "Label": "Register of removed trustees",
          "ShortLabel": "",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "Radio",
            "Options": [
              {
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if any removals apply"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "ReturnToSection",
          "ReturnId": "1",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": "<p class=\"govuk-body\">Has any director, or any other person with significant control of your organisation, been removed from the charities commission or appear on the register of removed trustees?</p>"
    }
  ]
}
', N'Organisation details', N'Organisation details', N'Draft', N'Pages')
GO



INSERT [dbo].[WorkflowSections]
  ([Id], [QnAData], [Title], [LinkTitle], [Status], [DisplayType])
VALUES
  (N'5da45e68-d4fd-4fb6-9b04-4038d7adb4df', N'
{
  "Pages": [
    {
      "PageId": "15",
      
      
      "Title": "Authoriser details",
      "LinkTitle": "Name and job title",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "W_DEL-01",
          "Label": "Name",
          "ShortLabel": "Name",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter the name of the person named as authoriser"
              }
            ]
          }
        },
        {
          "QuestionId": "W_DEL-02",
          "Label": "Job title",
          "ShortLabel": "Job title",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter the job title of the person named as authoriser"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "193",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": "<p class=\"govuk-body\">Who is signing your application?</p><p class=\"govuk-body\">Include the name and job title of the person named as authoriser.</p>"
    },
    {
      "PageId": "193",
      
      
      "Title": "Grounds for Mandatory exclusion",
      "LinkTitle": "Criminal convictions",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "M_DEL-09",
          "Label": "Criminal convictions",
          "ShortLabel": "Criminal convictions",
          "QuestionBodyText": "<p class=\"govuk-body\">Has any person of significant control (PSC) in the last 5 years, been convicted of:</p><ul class=\"govuk-list govuk-list--bullet\"><li>any offence under sections 44 to 46 of the Serious Crime Act 2007 which relates to an offence covered by subparagraph (f)</li><li>conspiracy within the meaning of section 1 or 1A of the Criminal Law Act 1977</li><li>conspiracy within article 9 or 9A of the criminal attempts and conspiracy (Northern Ireland) order 1983, relating to participation in a criminal organisation</li></ul>",
          "Hint": "",
          "Input": {
            "Type": "Radio",
            "Options": [
              {
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if any convictions apply"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "194",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": "",
      "Details": {
        "Title": "What classifies as a person with significant control?",
        "Body": "<p class=\"govuk-body\">A <a class=\"govuk-link\" target=\"_blank\" href=\"https://www.gov.uk/government/news/people-with-significant-control-psc-who-controls-your-company\">person with significant control (PSC)</a> is someone who owns or controls your organisation. They''re sometimes called ''beneficial owners''</p>"
      }
    },
    {
      "PageId": "194",
      
      
      "Title": "Grounds for Mandatory exclusion",
      "LinkTitle": "Financial convictions",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "M_DEL-10",
          "Label": "Financial convictions",
          "ShortLabel": "Financial convictions",
          "QuestionBodyText": "<p class=\"govuk-body\">Has any person of significant control (PSC) in the last 5 years, been convicted of fraud affecting European communities, including:</p><ul class=\"govuk-list govuk-list--bullet\"><li>the common law offence of cheating the Revenue (HMRC)</li><li>the common law offence of conspiracy to defraud</li><li>fraud or theft within the meaning of the Theft Act 1968, the Theft Act (Northern Ireland) 1969, the Theft Act 1978 or the Theft (Northern Ireland) Order 1978</li><li>fraudulent trading within the meaning of section 458 of the Companies Act 1985, article 451 of the Companies (Northern Ireland) Order 1986 or section 993 of the Companies Act 2006</li><li>fraudulent evasion within the meaning of section 170 of the Customs and Excise Management Act 1979 or section 72 of the Value Added Tax Act 1994</li><li>an offence in connection with taxation in the European Union within the meaning of section 71 of the Criminal Justice Act 1993</li><li>destroying, defacing or concealing of documents or procuring the execution of a valuable security within the meaning of section 20 of the Theft Act 1968 or section 19 of the Theft Act (Northern Ireland) 1969</li><li>fraud within the meaning of section 2, 3 or 4 of the Fraud Act 2006</li><li>the possession of articles for use in frauds within the meaning of section 6 of the Fraud Act 2006, or the making, adapting, supplying or offering to supply articles for use in frauds within the meaning of section 7 of that Act</li></ul>",
          "Hint": "",
          "Input": {
            "Type": "Radio",
            "Options": [
              {
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if any convictions apply"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "195",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "195",
      
      
      "Title": "Grounds for Mandatory exclusion",
      "LinkTitle": "Counter terrorism",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "M_DEL-11",
          "Label": "Counter terrorism",
          "ShortLabel": "Counter terrorism",
          "QuestionBodyText": "<p class=\"govuk-body\">Has any person with significant control in the last 5 years been convicted of:</p><ul class=\"govuk-list govuk-list--bullet\"><li>section 41 of the <a class=\"govuk-link\" target=\"_blank\" href=\"https://www.gov.uk/government/collections/counter-terrorism-and-security-bill\">Counter Terrorism Act 2008</a></li><li>schedule 2 of the Counter Terrorism Act 2008 where the court has determined that there is a terrorist connection</li><li>any offence under sections 44 to 46 of the Serious Crime Act 2007</li></ul>",
          "Hint": "",
          "Input": {
            "Type": "Radio",
            "Options": [
              {
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if any convictions apply"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "196",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "196",
      
      
      "Title": "Grounds for Mandatory exclusion",
      "LinkTitle": "Other criminal convictions",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "M_DEL-12",
          "Label": "Other criminal convictions",
          "ShortLabel": "Other criminal convictions",
          "QuestionBodyText": "<p class=\"govuk-body\">Has any person of significant control (PSC) in the last 5 years, been convicted of:</p><ul class=\"govuk-list govuk-list--bullet\"><li>money laundering within the meaning of sections 340(11) and 415 of the Proceeds of Crime Act 2002</li><li>an offence in connection with the proceeds of criminal conduct within the meaning of section 93A, 93B or 93C of the Criminal Justice Act 1988 or article 45, 46 or 47 of the Proceeds of Crime (Northern Ireland) Order 1996</li><li>an offence under section 4 of the Asylum and Immigration (Treatment of Claimants etc) Act 2004</li><li>an offence under section 59A of the Sexual Offences Act 2003</li><li>an offence under section 71 of the Coroners and Justice Act 2009</li><li>an offence in connection with the proceeds of drug trafficking within the meaning of section 49, 50 and 51 of the Drug Trafficking Act 1994</li></ul>",
          "Hint": "",
          "Input": {
            "Type": "Radio",
            "Options": [
              {
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if any convictions apply"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "20",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "20",
      
      
      "Title": "Grounds for discretionary exclusion",
      "LinkTitle": "Tax and social security irregularities",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "D_DEL-13",
          "Label": "Tax and social security irregularities",
          "ShortLabel": "Cessation of trading",
          "QuestionBodyText": "<p class=\"govuk-body\">Has any person of significant control (PSC) breached tax payments or social security contributions?</p>",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "D_DEL-13.1",
                    "Label": "Provide details whether you''ve paid, or have entered into a binding arrangement with a view to paying, including, where applicable, any accrued interest and/or fines.",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Textarea",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter details of payments or payment agreements"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if any convictions apply"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "201",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "201",
      
      
      "Title": "Grounds for discretionary exclusion",
      "LinkTitle": "Bankruptcy and insolvency",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "D_DEL-14",
          "Label": "Bankruptcy and insolvency",
          "ShortLabel": "Tax and social security irregularities",
          "QuestionBodyText": "<p class=\"govuk-body\">In the past 3 years has your organisation or partner organisations been:</p><ul class=\"govuk-list govuk-list--bullet\"><li>made bankrupt</li><li>subject of insolvency</li><li>winding-up proceedings</li></ul>",
          "Hint": "",
          "Input": {
            "Type": "Radio",
            "Options": [
              {
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if any bankruptcy or insolvency proceedings apply"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "20.1",
          "Condition": {
            "QuestionId": "D_DEL-14",
            "MustEqual": "Yes"
          },
          "ConditionMet": false
        },
        {
          "Action": "NextPage",
          "ReturnId": "202",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "20.1",
      
      
      "Title": "Grounds for discretionary exclusion",
      "LinkTitle": "Bankruptcy and insolvency details",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "D_DEL-13-1",
          "Label": "Type of proceeding",
          "ShortLabel": "Type of proceeding",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter the type of proceeding"
              }
            ]
          }
        },
        {
          "QuestionId": "D_DEL-13-2",
          "Label": "Date of proceedings",
          "ShortLabel": "Date of proceedings",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "Date",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter a date"
              },
              {
                "Name": "Date",
                "Value": "",
                "ErrorMessage": "Date must be correct"
              }
            ]
          }
        },
        {
          "QuestionId": "D_DEL-13-3",
          "Label": "If repaying debts, how you are repaying the debt?",
          "ShortLabel": "If repaying debts, how you are repaying the debt?",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "Textarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter how you are repaying the debts"
              }
            ]
          }
        },
        {
          "QuestionId": "D_DEL-13-4",
          "Label": "Date the debt will be cleared",
          "ShortLabel": "Date the debt will be cleared",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "Date",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter a date"
              },
              {
                "Name": "Date",
                "Value": "",
                "ErrorMessage": "Date must be correct"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "202",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": true,
      "Active": false,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": "",
      "ActivatedByPageId": "201"
    },
    {
      "PageId": "202",
      
      
      "Title": "Grounds for discretionary exclusion",
      "LinkTitle": "Cessation of trading",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "A_DEL-21",
          "Label": "Cessation of trading",
          "ShortLabel": "Cessation of trading",
          "QuestionBodyText": "<p class=\"govuk-body\">Confirm whether your organisation or any of your partner organisations is in:</p><ul class=\"govuk-list govuk-list--bullet\"><li>voluntary administration or company voluntary arrangement</li><li>compulsory winding up</li><li>receivership</li><li>composition with creditors</li></ul>",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "A_DEL-21.1",
                    "Label": "Provide details of any mitigating factors that you think should be taken into consideration",
                    "ShortLabel": "Provide details of any mitigating factors that you think should be taken into consideration",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Textarea",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter details of any mitigating factors"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if any cessation of trading proceedings apply"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "203",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "203",
      
      
      "Title": "Grounds for discretionary exclusion",
      "LinkTitle": "Incorrect tax returns",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "A_DEL-22",
          "Label": "Incorrect tax returns",
          "ShortLabel": "Incorrect tax returns",
          "QuestionBodyText": "<p class=\"govuk-body\">Has your organisation or partner organisations tax returns on or after the 1 October 2013:</p><ul class=\"govuk-list govuk-list--bullet\"><li>been found to be incorrect as of 1 April 2013</li><li>resulted in a criminal conviction for tax-related offences which are unspent, or to a civil penalty for fraud or evasion</li></ul>",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "A_DEL-22.1",
                    "Label": "Provide details of any mitigating factors that you think should be taken into consideration",
                    "ShortLabel": "Provide details of any mitigating factors that you think should be taken into consideration",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Textarea",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter details of any mitigating factors"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if any tax returns have been found to be incorrect"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "204",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "204",
      
      
      "Title": "Grounds for discretionary exclusion",
      "LinkTitle": "HMRC challenges to tax returns",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "A_DEL-23",
          "Label": "HMRC challenges to tax returns",
          "ShortLabel": "HMRC challenges to tax returns",
          "QuestionBodyText": "<p class=\"govuk-body\">Has your organisation or your partner organisations tax returns between 1 October 2012 and 1 April 2013 been:</p><ul class=\"govuk-list govuk-list--bullet\"><li>successfully challenged by HMRC under the general anti-abuse rule (GAAR) or the Halifax abuse principle</li><li>challenged by a tax authority in the jurisdiction your organisation is based with rules or legislation similar to GAAR or Halifax</li><li>resulted in criminal convictions for tax-related offences which are unspent, or to a civil penalty for fraud or evasion</li></ul>",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "A_DEL-23.1",
                    "Label": "Provide details of any mitigating factors that you think should be taken into consideration",
                    "ShortLabel": "Provide details of any mitigating factors that you think should be taken into consideration",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Textarea",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter details of any mitigating factors"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if any tax returns have been found to be incorrect"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "205",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "205",
      
      
      "Title": "Grounds for discretionary exclusion",
      "LinkTitle": "Contracts withdrawn from you",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "A_DEL-24",
          "Label": "Contracts withdrawn from you",
          "ShortLabel": "Contracts withdrawn from you",
          "QuestionBodyText": "<p class=\"govuk-body\">Has your organisation had any contract for the delivery of services withdrawn within the last 3 financial years?</p>",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "A_DEL-24.1",
                    "Label": "Provide details of any mitigating factors that you think should be taken into consideration",
                    "ShortLabel": "Provide details of any mitigating factors that you think should be taken into consideration",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Textarea",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter details of any mitigating factors"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if your organisation has had any contracts withdrawn"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "206",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "206",
      
      
      "Title": "Grounds for discretionary exclusion",
      "LinkTitle": "Contracts you have withdrawn from",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "A_DEL-25",
          "Label": "Contracts you have withdrawn from",
          "ShortLabel": "Contracts you have withdrawn from",
          "QuestionBodyText": "<p class=\"govuk-body\">Has your organisation withdrawn from a contract for the delivery of services within the last 3 years?</p>",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "A_DEL-25.1",
                    "Label": "Provide details of any mitigating factors that you think should be taken into consideration",
                    "ShortLabel": "Provide details of any mitigating factors that you think should be taken into consideration",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Textarea",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter details of any mitigating factors"
                        }
                      ]
                    }
                  },
                  {
                    "QuestionId": "A_DEL-25.2",
                    "Label": "Full name",
                    "ShortLabel": "Full name",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Text",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter a full name"
                        }
                      ]
                    }
                  },
                  {
                    "QuestionId": "A_DEL-25.3",
                    "Label": "Dates involved",
                    "ShortLabel": "Dates involved",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Date",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter a date"
                        },
                        {
                          "Name": "Date",
                          "Value": "",
                          "ErrorMessage": "Date must be correct"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if your organisation has withdrawn from any contracts"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "207",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "207",
      
      
      "Title": "Grounds for discretionary exclusion",
      "LinkTitle": "Organisation removal from registers",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "A_DEL-26",
          "Label": "Organisation removal from registers",
          "ShortLabel": "Organisation removal from registers",
          "QuestionBodyText": "<p class=\"govuk-body\">Has you organisation been removed from:</p><ul class=\"govuk-list govuk-list--bullet\"><li>Education and skills funding agency''s register of training organisations (RoATP)</li><li>End-point assessor organisations (EPAO) register</li><li>Ofqual''s register</li><li>other professional or trade registers</li></ul>",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "A_DEL-26.1",
                    "Label": "Date of removal",
                    "ShortLabel": "Date of removal",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Date",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter a date"
                        },
                        {
                          "Name": "Date",
                          "Value": "",
                          "ErrorMessage": "Date must be correct"
                        }
                      ]
                    }
                  },
                  {
                    "QuestionId": "A_DEL-26.2",
                    "Label": "Reasons why your organisation was removed",
                    "ShortLabel": "Reasons why your organisation was removed",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Textarea",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter the reasons why your organisation was removed from those registers"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if your organisation been removed from the listed registers"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "208",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "208",
      
      
      "Title": "Grounds for discretionary exclusion",
      "LinkTitle": "Directions and sanctions",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "A_DEL-27",
          "Label": "Directions and sanctions",
          "ShortLabel": "Directions and sanctions",
          "QuestionBodyText": "<p class=\"govuk-body\">Has your organisation received direction or sanctions from any of the following?</p><ul class=\"govuk-list govuk-list--bullet\"><li>Ofqual</li><li>Quality Assurance Agency (QAA) for higher education</li><li>awarding organisations</li><li>other similar bodies</li></ul>",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "A_DEL-27.1",
                    "Label": "Date of sanction",
                    "ShortLabel": "Date of sanction",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Date",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter a date"
                        },
                        {
                          "Name": "Date",
                          "Value": "",
                          "ErrorMessage": "Date must be correct"
                        }
                      ]
                    }
                  },
                  {
                    "QuestionId": "A_DEL-27.2",
                    "Label": "Reasons why your organisation received direction or sanctions",
                    "ShortLabel": "Reasons why your organisation received direction or sanctions",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Textarea",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter the reasons why your organisation received direction or sanctions"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if your organisation has received direction or sanctions"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "209",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "209",
      
      
      "Title": "Grounds for discretionary exclusion",
      "LinkTitle": "Repayment of public money",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "A_DEL-28",
          "Label": "Repayment of public money",
          "ShortLabel": "Repayment of public money",
          "QuestionBodyText": "<p class=\"govuk-body\">Has your organisation ever had to repay public money?</p>",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "A_DEL-28.1",
                    "Label": "Provide details of any mitigating factors that you think should be taken into consideration",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Textarea",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter details of any mitigating factors"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if your organisation has ever had to repay public money"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "210",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "210",
      
      
      "Title": "Grounds for discretionary exclusion",
      "LinkTitle": "Public body funds and contracts",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "A_DEL-29",
          "Label": "Public body funds and contracts",
          "ShortLabel": "Public body funds and contracts",
          "QuestionBodyText": "<p class=\"govuk-body\">Has any person of significant control in your organisation had:</p><ul class=\"govuk-list govuk-list--bullet\"><li>a failure to repay funding due to any public body</li><li>early termination of a contract with a public body</li><li>early withdrawal from a contract with a public body</li></ul>",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "A_DEL-29.1",
                    "Label": "Provide details of any mitigating factors that you think should be taken into consideration",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Textarea",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter details of any mitigating factors"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if any of the listed apply"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "211",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "211",
      
      
      "Title": "Grounds for discretionary exclusion",
      "LinkTitle": "Legal dispute",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "A_DEL-30",
          "Label": "Legal dispute",
          "ShortLabel": "Legal dispute",
          "QuestionBodyText": "<p class=\"govuk-body\">Does your organisation have any outstanding or ongoing legal dispute that could prevent you from conducting end-point assessments?</p>",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "A_DEL-30.1",
                    "Label": "Date",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Date",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter a date"
                        },
                        {
                          "Name": "Date",
                          "Value": "",
                          "ErrorMessage": "Date must be correct"
                        }
                      ]
                    }
                  },
                  {
                    "QuestionId": "A_DEL-30.2",
                    "Label": "Details of the dispute",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Textarea",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter details of the dispute"
                        }
                      ]
                    }
                  },
                  {
                    "QuestionId": "A_DEL-30.3",
                    "Label": "Current status of the dispute",
                    "ShortLabel": "",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Textarea",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter current status of the dispute"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if your organisation has an outstanding or ongoing legal dispute"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "22",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "22",
      
      
      "Title": "Application accuracy",
      "LinkTitle": "False declarations",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "A_DEL-28",
          "Label": "False declarations",
          "ShortLabel": "False declarations",
          "QuestionBodyText": "<p class=\"govuk-body\">I certify that the information provided is accurate and accept the conditions and undertakings requested in this application. It''s understood that false information may result in:</p><ul class=\"govuk-list govuk-list--bullet\"><li>exclusion from this and future registers</li><li>the removal from the Register of End-point Assessments Organisations</li><li>the withdrawal of contracts with employers</li><li>civil or criminal proceedings</li></ul>",
          "Hint": "",
          "Input": {
            "Type": "Radio",
            "Options": [
              {
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if you understand the false declarations statement"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "221",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "221",
      
      
      "Title": "Application accuracy",
      "LinkTitle": "Accurate and true representation",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "A_DEL-29",
          "Label": "Accurate and true representation",
          "ShortLabel": "Accurate and true representation",
          "QuestionBodyText": "<p class=\"govuk-body\">Will your applications to deliver end-point assessments for standards be accurate and true representations?</p>",
          "Hint": "",
          "Input": {
            "Type": "Radio",
            "Options": [
              {
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if your applications to deliver end-point assessments will be accurate and true representations"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "222",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "222",
      
      
      "Title": "Application accuracy",
      "LinkTitle": "Agreement to appear on the register",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "A_DEL-30",
          "Label": "Agreement to appear on the register",
          "ShortLabel": "Agreement to appear on the register",
          "QuestionBodyText": "<p class=\"govuk-body\">Do you agree your organisation details will be added to the register of end-point assessment organisations if your application is successful?</p>",
          "Hint": "",
          "Input": {
            "Type": "Radio",
            "Options": [
              {
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if you agree for your organisation details to be added to the register"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "ReturnToSection",
          "ReturnId": "2",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    }
  ]
}   
', N'Declarations', N'Declarations', N'Draft', N'PagesWithSections')
GO


INSERT [dbo].[WorkflowSections]
  ([Id], [QnAData], [Title], [LinkTitle], [Status], [DisplayType])
VALUES
  (N'580aa30f-f65b-4c05-808f-f8eb3d539459', N'
{
  "Pages": [
    {
      "PageId": "23",
      
      
      "Title": "Financial health",
      "LinkTitle": "Financial health assessment",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "FHA-01",
          "Label": "Upload your accounts",
          "ShortLabel": "Upload your accounts",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "FileUpload",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select a file containing your accounts"
              },
              {
                "Name": "FileType",
                "Value": "pdf,application/pdf",
                "ErrorMessage": "The selected file must be a PDF"
              }
            ]
          }
        },
        {
          "QuestionId": "FHA-02",
          "Label": "Upload your parent company''s accounts (optional)",
          "ShortLabel": "Upload your parent company''s accounts (optional)",
          "QuestionBodyText": "",
          "Hint": "If you have a parent company, upload your parent company''s statutory accounts.",
          "Input": {
            "Type": "FileUpload",
            "Validations": [
              {
                "Name": "FileType",
                "Value": "pdf,application/pdf",
                "ErrorMessage": "The selected file must be a PDF"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "ReturnToSection",
          "ReturnId": "3",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": "<p class=\"govuk-body\">If you have been trading for more than 12 months, upload your last financial years'' statutory accounts and your parent company''s accounts (if you have a parent company).</p><p class=\"govuk-body\">If you have been trading for less than 12 months, upload your management accounts.</p>",
      "Details": {
        "Title": "SQ-1-SE-2-PG-23-DT-1",
        "Body": "SQ-1-SE-2-PG-23-DB-1"
      }
    }
  ]
}
', N'Financial health assessment', N'Financial health assessment', N'Draft', N'Pages')
GO

INSERT [dbo].[WorkflowSections]
  ([Id], [QnAData], [Title], [LinkTitle], [Status], [DisplayType])
VALUES
  (N'b4951ead-ee4a-49f2-a31e-3a658605e32a', N'
{
  "Pages": [
    {
      "PageId": "24",
      
      
      "Title": "Your policies and procedures",
      "LinkTitle": "Information commissioner''s office registration",
      "InfoText": "SQ-2-SE-4-PG-24-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-01",
          "Label": "Information commissioner''s office (ICO) registration number",
          "ShortLabel": "Information commissoner''s office registration",
          "QuestionBodyText": "",
          "Hint": "Provide your ICO registration number",
          "Input": {
            "Type": "text",
            "InputClasses": "govuk-input--width-10",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter your ICO registration number"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "25",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "25",
      
      
      "Title": "Your policies and procedures",
      "LinkTitle": "Internal audit policy",
      "InfoText": "SQ-2-SE-4-PG-24-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-02",
          "Label": "Internal audit policy",
          "ShortLabel": "Internal audit policy",
          "QuestionBodyText": "<p class=\"govuk-body\">Upload a PDF of your organisation''s internal audit policy in respect to fraud and financial irregularity</p>",
          "Hint": "",
          "Input": {
            "Type": "FileUpload",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select an internal audit policy document"
              },
              {
                "Name": "FileType",
                "Value": "pdf,application/pdf",
                "ErrorMessage": "The selected file must be a PDF"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "26",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "26",
      
      
      "Title": "Your policies and procedures",
      "LinkTitle": "Public liability insurance",
      "InfoText": "SQ-2-SE-4-PG-24-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-03",
          "Label": "Public liability insurance",
          "ShortLabel": "Public liability insurance",
          "QuestionBodyText": "<p class=\"govuk-body\">Upload a PDF of your public liability certificate of insurance</p>",
          "Hint": "If you are providing any form of training or consultancy, you must have public liability insurance.",
          "Input": {
            "Type": "FileUpload",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select a public liability certificate of insurance"
              },
              {
                "Name": "FileType",
                "Value": "pdf,application/pdf",
                "ErrorMessage": "The selected file must be a PDF"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "27",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "27",
      
      
      "Title": "Your policies and procedures",
      "LinkTitle": "Professional indemnity insurance",
      "InfoText": "SQ-2-SE-4-PG-24-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-04",
          "Label": "Professional indemnity insurance",
          "ShortLabel": "Professional indemnity insurance",
          "QuestionBodyText": "<p class=\"govuk-body\">Upload a PDF of your professional indemnity certificate of insurance</p>",
          "Hint": "If you are providing any form of training or consultancy, you must have professional indemnity insurance.",
          "Input": {
            "Type": "FileUpload",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select a professional indemnity certificate of insurance"
              },
              {
                "Name": "FileType",
                "Value": "pdf,application/pdf",
                "ErrorMessage": "The selected file must be a PDF"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "28",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "28",
      
      
      "Title": "Your policies and procedures",
      "LinkTitle": "Employers liability insurance",
      "InfoText": "SQ-2-SE-4-PG-24-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-05",
          "Label": "Employers liability insurance",
          "ShortLabel": "Employers liability insurance",
          "QuestionBodyText": "<p class=\"govuk-body\">Upload a PDF of your employers liability certificate of insurance (optional)</p>",
          "Hint": "If you have any employees, you must have employers liability insurance. ",
          "Input": {
            "Type": "FileUpload",
            "Validations": [
              {
                "Name": "FileType",
                "Value": "pdf,application/pdf",
                "ErrorMessage": "The selected file must be a PDF"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "29",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "29",
      
      
      "Title": "Your policies and procedures",
      "LinkTitle": "Safeguarding policy",
      "InfoText": "SQ-2-SE-4-PG-24-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-06",
          "Label": "Safeguarding policy",
          "ShortLabel": "Safeguarding policy",
          "QuestionBodyText": "<p class=\"govuk-body\">Upload a PDF of your safeguarding policy</p>",
          "Hint": "",
          "Input": {
            "Type": "FileUpload",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select a safeguarding policy document"
              },
              {
                "Name": "FileType",
                "Value": "pdf,application/pdf",
                "ErrorMessage": "The selected file must be a PDF"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "30",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "30",
      
      
      "Title": "Your policies and procedures",
      "LinkTitle": "Prevent agenda policy",
      "InfoText": "SQ-2-SE-4-PG-24-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-07",
          "Label": "Prevent agenda policy",
          "ShortLabel": "Prevent agenda policy",
          "QuestionBodyText": "<p class=\"govuk-body\">Upload your PDF conflict of interest policy document</p>",
          "Hint": "",
          "Input": {
            "Type": "FileUpload",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select a prevent agenda policy document"
              },
              {
                "Name": "FileType",
                "Value": "pdf,application/pdf",
                "ErrorMessage": "The selected file must be a PDF"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "31",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "31",
      
      
      "Title": "Your policies and procedures",
      "LinkTitle": "Conflict of interest policy",
      "InfoText": "SQ-2-SE-4-PG-24-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-08",
          "Label": "Conflict of interest policy",
          "ShortLabel": "Conflict of interest policy",
          "QuestionBodyText": "<p class=\"govuk-body\">Upload a PDF of your conflict of interest policy</p>",
          "Hint": "",
          "Input": {
            "Type": "FileUpload",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select a conflict of interest policy document"
              },
              {
                "Name": "FileType",
                "Value": "pdf,application/pdf",
                "ErrorMessage": "The selected file must be a PDF"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "32",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "32",
      
      
      "Title": "Your policies and procedures",
      "LinkTitle": "Monitoring procedures",
      "InfoText": "SQ-2-SE-4-PG-24-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-09",
          "Label": "Monitoring procedures",
          "ShortLabel": "Monitoring procedures",
          "QuestionBodyText": "<p class=\"govuk-body\">Upload a PDF of your procedures for monitoring assessor practices and decisions</p>",
          "Hint": "",
          "Input": {
            "Type": "FileUpload",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select a file describing your procedures for monitoring assessor practices and decisions"
              },
              {
                "Name": "FileType",
                "Value": "pdf,application/pdf",
                "ErrorMessage": "The selected file must be a PDF"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "33",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "33",
      
      
      "Title": "Your policies and procedures",
      "LinkTitle": "Monitoring processes",
      "InfoText": "SQ-2-SE-4-PG-24-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-10",
          "Label": "Moderation processes",
          "ShortLabel": "Moderation processes",
          "QuestionBodyText": "<p class=\"govuk-body\">Upload a PDF describing your standardisation and moderation activities, including how you sample assessment decisions</p>",
          "Hint": "",
          "Input": {
            "Type": "FileUpload",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select an file describing your standardisation and moderation activities"
              },
              {
                "Name": "FileType",
                "Value": "pdf,application/pdf",
                "ErrorMessage": "The selected file must be a PDF"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "34",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "34",
      
      
      "Title": "Your policies and procedures",
      "LinkTitle": "Complaints and appeals policy",
      "InfoText": "SQ-2-SE-4-PG-24-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-11",
          "Label": "Complaints and appeals policy",
          "ShortLabel": "Employers liability insurance",
          "QuestionBodyText": "<p class=\"govuk-body\">Upload a PDF of your complaints and appeals policy</p>",
          "Hint": "",
          "Input": {
            "Type": "FileUpload",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select a compalints and appeals policy document"
              },
              {
                "Name": "FileType",
                "Value": "pdf,application/pdf",
                "ErrorMessage": "The selected file must be a PDF"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "340",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "340",
      
      
      "Title": "Your policies and procedures",
      "LinkTitle": "Fair access",
      "InfoText": "SQ-2-SE-4-PG-24-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-12",
          "Label": "Fair access",
          "ShortLabel": "Fair access",
          "QuestionBodyText": "<p class=\"govuk-body\">Upload a PDF of your fair access policy</p>",
          "Hint": "",
          "Input": {
            "Type": "FileUpload",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select a fair access policy document"
              },
              {
                "Name": "FileType",
                "Value": "pdf,application/pdf",
                "ErrorMessage": "The selected file must be a PDF"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "35",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "35",
      
      
      "Title": "Your policies and procedures",
      "LinkTitle": "Consistency assurance",
      "InfoText": "SQ-2-SE-4-PG-24-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-13",
          "Label": "Consistency assurance",
          "ShortLabel": "Consistency assurance",
          "QuestionBodyText": "<p class=\"govuk-body\">Upload a PDF of your strategy for ensuring comparability and consistency of assessment decisions</p>",
          "Hint": "",
          "Input": {
            "Type": "FileUpload",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select a file describing your comparability and consistency decisions"
              },
              {
                "Name": "FileType",
                "Value": "pdf,application/pdf",
                "ErrorMessage": "The selected file must be a PDF"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "36",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "36",
      
      
      "Title": "Your policies and procedures",
      "LinkTitle": "Continuous quality assurance",
      "InfoText": "SQ-2-SE-4-PG-24-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-14",
          "Label": "How do you continuously improve the quality of your assessment practice?",
          "ShortLabel": "How do you continuously improve the quality of your assessment practice?",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter how you continuously improve the quality of your assessment practice"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "How you continuously improve the quality of your assessment practice must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "37",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "37",
      
      
      "Title": "Your occupational competence",
      "LinkTitle": "Engagement with trailblazers and employers",
      "InfoText": "SQ-2-SE-4-PG-25-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-16",
          "Label": "Engagement with trailblazers and employers",
          "ShortLabel": "Engagement with trailblazers and employers",
          "QuestionBodyText": "<p class=\"govuk-body\">Give evidence of engagement with trailblazers and employers to demonstrate your organisation''s occupational competence to assess [StandardName].</p><p class=\"govuk-body\">Your evidence must demonstrate your organisation''s relevant experience of working with employers or working in the specific occupational area.</p><p class=\"govuk-body\">Your evidence must not be over three years old and must not relate to the development and implementation of qualifications.</p><p class=\"govuk-body\">You should give answers that relate to the assessment plan for the standard you are applying for.</p>",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter your evidence of engagement with trailblazers and employers"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "Evidence of engagement with trailblazers and employers must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "38",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "38",
      
      
      "Title": "Your occupational competence",
      "LinkTitle": "Professional organisation membership",
      "InfoText": "SQ-2-SE-4-PG-25-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-19",
          "Label": "Give details of membership of professional organisations",
          "ShortLabel": "Professional organisation membership",
          "QuestionBodyText": "<p class=\"govuk-body\">Give details of why this supports best practice and skills for this assessment plan. Show how it demonstrates your competence to assess this standard.</p>",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter details of membership to professional organisations"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "Details of membership to professional organisations must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "39",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [
        "HEI"
      ],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "39",
      
      
      "Title": "Your assessors",
      "LinkTitle": "Number of assessors",
      "InfoText": "SQ-2-SE-4-PG-26-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-20",
          "Label": "How many assessors do you have?",
          "ShortLabel": "Number of assessors",
          "QuestionBodyText": "",
          "Hint": "This should include invigilators where the end-point assessment is an examination",
          "Input": {
            "Type": "number",
            "InputClasses": "govuk-input--width-3",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Provide the number of assessors your organisation has"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "40",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "40",
      
      
      "Title": "Your assessors",
      "LinkTitle": "Assessment capacity",
      "InfoText": "SQ-2-SE-4-PG-26-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-21",
          "Label": "What''s the volume of end-point assessments you can deliver?",
          "ShortLabel": "Assessment capacity",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "number",
            "InputClasses": "govuk-input--width-5",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter the volume of end-point assessments you can deliver"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "41",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "41",
      
      
      "Title": "Your assessors",
      "LinkTitle": "Ability to deliver assessments",
      "InfoText": "SQ-2-SE-4-PG-26-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-22",
          "Label": "How will the volume of end-point assessments be achieved with the number of assessors you have?",
          "ShortLabel": "How will the volume of end-point assessments be achieved with the number of assessors you have?",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter how the volume of end-point assessments be achieved"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "How the volume of end-point assessments be achieved must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "42",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "42",
      
      
      "Title": "Your professional standards",
      "LinkTitle": "Recruitment and training",
      "InfoText": "SQ-2-SE-4-PG-27-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-23",
          "Label": "How do you recruit and train assessors?",
          "ShortLabel": "Recruitment and training",
          "QuestionBodyText": "<p class=\"govuk-body\">All assessors must be qualified to undertake assessments in line with the requirements laid out in the assessment plan.</p><p class=\"govuk-body\">They must have expertise and experience in designing and developing assessment products and tools where this is a requirement of the assessment plan</p>",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter how you recruit and train assessors"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "How you recruit and train assessors must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "43",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "43",
      
      
      "Title": "Your professional standards",
      "LinkTitle": "Skills and qualifications",
      "InfoText": "SQ-2-SE-4-PG-27-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-24",
          "Label": "What experience, skills and qualifications do your assessors have?",
          "ShortLabel": "Skills and qualifications",
          "QuestionBodyText": "<p class=\"govuk-body\">You need to give examples of how, when and where the assessor has demonstrated their capability. Assessors must have current and relevant occupational and assessment experience. Give details, evidence and context for this experience of current and future assessors.</p>",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter the experience, skills and qualifications your assessors have"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "Details of experience, skills and qualifications must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "44",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "44",
      
      
      "Title": "Your professional standards",
      "LinkTitle": "Continuous professional development",
      "InfoText": "SQ-2-SE-4-PG-27-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-25",
          "Label": "How do you ensure your assessors'' occupational expertise is maintained and kept current? ",
          "ShortLabel": "Continuous professional development",
          "QuestionBodyText": "<p class=\"govuk-body\">Give examples, of up to 500 words, for current professional development and recent experience of where and how they have demonstrated their suitability.</p><p class=\"govuk-body\">Give details, evidence and context for this experience, for the assessors you have now or will have in place by the time you start delivery.</p>",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter how you ensure your assessors'' occupational expertise is maintained"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "How occupational expertise is maintained must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "45",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "45",
      
      
      "Title": "Your end-point assessment delivery model",
      "LinkTitle": "End-point assessment delivery model",
      "InfoText": "SQ-2-SE-4-PG-28-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-26",
          "Label": "How will you conduct an end-point assessment for this standard?",
          "ShortLabel": "End-point assessment delivery model",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter how you will deliver an end-point assessment for this standard"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "How you will deliver an end-point assessment must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "46",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "46",
      
      
      "Title": "Your end-point assessment delivery model",
      "LinkTitle": "Outsourcing of end-point assessments",
      "InfoText": "SQ-2-SE-4-PG-28-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-27",
          "Label": "Do you intend to outsource any of your end-point assessments?",
          "ShortLabel": "Outsourcing of end-point assessments",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "CC-27.1",
                    "Label": "Quality assurance of outsourced services",
                    "ShortLabel": "SQ-1-SE-1-PG-28-CD-28-SL-1",
                    "QuestionBodyText": "SQ-1-SE-1-PG-28-CD-28-QB-1",
                    "Hint": "",
                    "Input": {
                      "Type": "Textarea",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter the quality assurance of outsourced services"
                        }
                      ]
                    }
                  }
                ],
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if you intend to outsource any of your end-point assessments"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "47",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "47",
      
      
      "Title": "Your end-point assessment delivery model",
      "LinkTitle": "Engaging with employers and training providers",
      "InfoText": "SQ-2-SE-4-PG-28-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-29",
          "Label": "How will you engage with employers and training organisations?",
          "ShortLabel": "How will you engage with employers and training organisations?",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter how you will engage with employers and training organisations"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "How you will engage with employers and training organisations must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "48",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "48",
      
      
      "Title": "Your end-point assessment delivery model",
      "LinkTitle": "Managing conflicts of interest",
      "InfoText": "SQ-2-SE-4-PG-28-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-30",
          "Label": "How will you manage any potential conflict of interest, particular to other functions your organisation may have?",
          "ShortLabel": "How will you manage any potential conflict of interest, particular to other functions your organisation may have?",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter how you will manage any potential conflict of interest"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "How you will manage any potential conflict of interest must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "49",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "49",
      
      
      "Title": "Your end-point assessment delivery model",
      "LinkTitle": "Assessment locations",
      "InfoText": "SQ-2-SE-4-PG-28-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-31",
          "QuestionTag": "delivery-areas",
          "Label": "Where will you conduct end-point assessments?",
          "ShortLabel": "Assessment locations",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "DataFed_CheckboxList",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter where you will conduct end-point assessments"
              }
            ],
            "DataEndpoint": "DeliveryAreas"
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "18",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "18",
      
      
      "Title": "Your end-point assessment delivery model",
      "LinkTitle": "Providing services straight away",
      "InfoText": "",
      "Questions": [
        {
          "QuestionId": "W_DEL-04",
          "QuestionTag": "effective-from",
          "Label": "If your application is successful, can you start an end-point assessment on the day you join the register?",
          "ShortLabel": "Providing services straight away",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "ComplexRadio",
            "Options": [
              {
                "Value": "Yes",
                "Label": "Yes"
              },
              {
                "FurtherQuestions": [
                  {
                    "QuestionId": "W_DEL-04.1",
                    "Label": "When will you be ready to do your first assessments?",
                    "ShortLabel": "When will you be ready to do your first assessments?",
                    "QuestionBodyText": "",
                    "Hint": "",
                    "Input": {
                      "Type": "Date",
                      "Validations": [
                        {
                          "Name": "Required",
                          "ErrorMessage": "Enter a date"
                        },
                        {
                          "Name": "Date",
                          "Value": "",
                          "ErrorMessage": "Date must be correct"
                        }
                      ]
                    }
                  }
                ],
                "Value": "No",
                "Label": "No"
              }
            ],
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Select yes if you can start an end-point assessment on the day you join the register"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "50",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "50",
      
      
      "Title": "Your end-point assessment delivery model",
      "LinkTitle": "Assessment methods",
      "InfoText": "SQ-2-SE-4-PG-28-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-32",
          "Label": "How will you conduct end-point assessments?",
          "ShortLabel": "Assessment methods",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter how you will conduct end-point assessments"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "How you will conduct end-point assessments must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "51",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "51",
      
      
      "Title": "Your end-point assessment delivery model",
      "LinkTitle": "Continuous resource development",
      "InfoText": "SQ-2-SE-4-PG-28-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-33",
          "Label": "How will you develop and maintain the required resources and assessment tools?",
          "ShortLabel": "How will you develop and maintain the required resources and assessment tools?",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter how you will develop and maintain the required resources and assessment tools"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "How you will develop and maintain the required resources and assessment tools must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "52",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "52",
      
      
      "Title": "Your end-point assessment competence",
      "LinkTitle": "Secure IT infrastructure",
      "InfoText": "SQ-2-SE-4-PG-29-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-34",
          "Label": "Secure IT infrastructure",
          "ShortLabel": "Secure IT infrastructure",
          "QuestionBodyText": "<p class=\"govuk-body\">Give full details of the secure IT infrastructure you will implement before providing a complete end-point assessment.</p>",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter details of the secure IT infrastructure you will implement"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "Details of the secure IT infrastructure must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "53",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "53",
      
      
      "Title": "Your end-point assessment competence",
      "LinkTitle": "Assessment administration",
      "InfoText": "SQ-2-SE-4-PG-29-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-35",
          "Label": "Assessment administration",
          "ShortLabel": "Assessment administration",
          "QuestionBodyText": "<p class=\"govuk-body\">Give full details of processes in place for administration of assessments before providing a complete end-point assessment.</p>",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter details of processes in place for administration of assessments"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "Details of processes in place for administration of assessments must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "54",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "54",
      
      
      "Title": "Your end-point assessment competence",
      "LinkTitle": "Assessment products and tools",
      "InfoText": "SQ-2-SE-4-PG-29-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-36",
          "Label": "Assessment products and tools",
          "ShortLabel": "Assessment products and tools",
          "QuestionBodyText": "<p class=\"govuk-body\">Give full details of the strategies in place for development of assessment products and tools.</p>",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter the strategies in place for development of assessment products and tools"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "Strategies in place for development of assessment products and tools must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "55",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "55",
      
      
      "Title": "Your end-point assessment competence",
      "LinkTitle": "Assessment content",
      "InfoText": "SQ-2-SE-4-PG-29-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-37",
          "Label": "Assessment content",
          "ShortLabel": "Assessment content",
          "QuestionBodyText": "<p class=\"govuk-body\">Give full details of the actions you will take and the processes you will implement as part of delivering a complete end-point assessment.</p>",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter the actions you will take and the processes you will implement"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "The actions you will take and the processes you will implement must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "56",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "56",
      
      
      "Title": "Your end-point assessment competence",
      "LinkTitle": "Collation & confirmation of assessment outcomes",
      "InfoText": "SQ-2-SE-4-PG-29-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-38",
          "Label": "Collation & confirmation of assessment outcomes",
          "ShortLabel": "Collation & confirmation of assessment outcomes",
          "QuestionBodyText": "<p class=\"govuk-body\">Give full details of how you collate and confirm assessment outcomes to employers, training providers and apprentices</p>",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter details of how you collate and confirm assessment outcomes to employers, training providers and apprentices"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "Details of how you collate and confirm assessment outcomes must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "57",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "57",
      
      
      "Title": "Your end-point assessment competence",
      "LinkTitle": "Recording assessment results",
      "InfoText": "SQ-2-SE-4-PG-29-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-39",
          "Label": "Recording assessment results",
          "ShortLabel": "Recording assessment results",
          "QuestionBodyText": "<p class=\"govuk-body\">Give full details of the processes in place for recording and issuing assessment results and certificates.</p>",
          "Hint": "",
          "Input": {
            "Type": "LongTextarea",
            "Validations": [
              {
                "Name": "Required",
                "ErrorMessage": "Enter details of the processes in place for recording and issuing assessment results and certificates"
              },
              {
                "Name": "MaxWordCount",
                "Value": 500,
                "ErrorMessage": "Details of the processes in place for recording and issuing assessment results and certificates must be 500 words or less"
              }
            ]
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "NextPage",
          "ReturnId": "58",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    },
    {
      "PageId": "58",
      
      
      "Title": "Your online information",
      "LinkTitle": "Web address",
      "InfoText": "SQ-2-SE-4-PG-30-IT-1",
      "Questions": [
        {
          "QuestionId": "CC-40",
          "QuestionTag": "standard-website",
          "Label": "Enter your web address",
          "ShortLabel": "Web address",
          "QuestionBodyText": "",
          "Hint": "",
          "Input": {
            "Type": "text",
            "InputClasses": "govuk-!-width-two-thirds",
            "Validations": []
          }
        }
      ],
      "PageOfAnswers": [],
      "Next": [
        {
          "Action": "ReturnToSection",
          "ReturnId": "4",
          "ConditionMet": false
        }
      ],
      "Complete": false,
      "AllowMultipleAnswers": false,
      "Active": true,
      "NotRequiredOrgTypes": [],
      "NotRequired": false,
      "BodyText": ""
    }
  ]
}
', N'Apply to assess a standard', N'Apply to assess a standard', N'Draft', N'PagesWithSections')
GO

DELETE from WorkFlowSequences 

INSERT [dbo].[WorkflowSequences]
  ([Id], [WorkflowId], [SequenceNo], [SectionNo], [SectionId], [Status], [IsActive])
VALUES
  (NEWID(), N'83b35024-8aef-440d-8f59-8c1cc459c350', 1,1,'b9c09252-3fee-455f-bc54-12c8788398b7', N'Draft', 1)
GO

INSERT [dbo].[WorkflowSequences]
  ([Id], [WorkflowId], [SequenceNo], [SectionNo], [SectionId], [Status], [IsActive])
VALUES
  (NEWID(), N'83b35024-8aef-440d-8f59-8c1cc459c350', 1,2,'5da45e68-d4fd-4fb6-9b04-4038d7adb4df', N'Draft', 1)
GO

INSERT [dbo].[WorkflowSequences]
  ([Id], [WorkflowId], [SequenceNo], [SectionNo], [SectionId], [Status], [IsActive])
VALUES
  (NEWID(), N'83b35024-8aef-440d-8f59-8c1cc459c350', 1,3,'580aa30f-f65b-4c05-808f-f8eb3d539459', N'Draft', 1)
GO

INSERT [dbo].[WorkflowSequences]
  ([Id], [WorkflowId], [SequenceNo], [SectionNo], [SectionId], [Status], [IsActive])
VALUES
  (NEWID(), N'83b35024-8aef-440d-8f59-8c1cc459c350', 2,1,'b4951ead-ee4a-49f2-a31e-3a658605e32a', N'Draft', 1)
GO