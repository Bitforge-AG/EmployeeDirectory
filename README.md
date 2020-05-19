---
name: Employee Directory
description: This contacts application is based on the Xamarin Forms example app Employee Directory with enhancements in UX, Design and functionality
languages:
- csharp
products:
- xamarin
urlFragment: employeedirectory
---
# Employee Directory

This Employee Directory has been created as a response to COVID-19 as a simple to use and implement solution.
It is a fully offline phonebook application and requires no backend integration. Just replace the data source (XLSX Files) and adapt the code to the new fields.

Additionally the app has a CallDirectoryExtension on iOS that supplies the Caller ID (Name, Department...) to identify incoming calls. Using Appcenter and giving your customer access to the git repository, you can implement a build pipeline where your customer can replace the data without your involvement.

## Getting started
* Replace or Update the Data Sources (Departments.xlsx, People.xlsx) in EmployeeDirectory.iOS/Resources and EmployeeDirectory.Android/Resources/raw
* Update the Data field names to match the Excel headers in Person.cs
* Update PersonViewModel with the new fields
### Call Directory Extension
* Create App ID's with an App Group in Apple Developer portal
* Update Bundle Identifiers
* Search for com.example and replace with your bundle identifiers and group name (in CallDirectoryHandler.cs and CallDirectoryStore.cs)

This project is based on the Employee Directory Example [https://docs.microsoft.com/en-us/samples/xamarin/xamarin-forms-samples/employeedirectory/] by Microsoft.

## Noteable changes:
* Data Source is XLSX instead of CVS
* Support for multiple data sources (Departments, People)
* Quick Selection of Search Filters (Main Menu)
* Improved Search
* Favourites with Nav Bar Icon
* Call Directory Extension (tested with more than 10'000 entries)
* Help Section (for Call Directory Extension)
* Blank Splash Screen (prevents discussions about Design)

## Known issues
* Favourites are not updated when the app is updated with new data
* Tested on iOS only
* No Caller ID functionality for Android implemented
