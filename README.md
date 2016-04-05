# TfsCruiser

TFS Cruiser is a great big visible information radiator to show off:

 - build status
 - test run status
 - basic code forensics (churn)

UPDATED for v2.0 of the Build REST API.

## Forensics

### Folder Churn

Details the folders with the most changes over a defined period.

    /Forensics/FolderChurn

The default looks at the past three months.

Specify Dates:

    /Forensics/FolderChurn?from=2016-01-01&to=2016-02-29

Specify Folder Depth (default is 4):

You can specify the depth of the folders to be used for folder churn, this allows you to aggregate churn at different levels. Depending on your code 
organisation, you will normally find that decreasing the depth shows you "modules", and increasing it shows "projects", for example.

    /Forensics/FolderChurn?folderDepth=4

### File Churn

Details the files with the most changes over a defined period.

    /Forensics/FileChurn

The default looks at the past three months.

Specify Dates:

    /Forensics/FileChurn?from=2016-01-01&to=2016-02-29

## General

It has been designed to work on a big screen, or projected onto the side of your building if you really want to show off.

Hook it up to your build server and TFS Cruiser will display the current status and a history of your builds.

TFS Cruiser was inspired by CruiseControl.NET monitor, [Cruiser](https://github.com/Steve-Fenton/Cruiser)

Config... you just need to tell TFS Cruiser the address of your build server...

    <add key="TeamAccount" value="Fabrikam" />
    <add key="TeamProject" value="CoolProject" />
    <add key="TeamUsername" value="fenton" />
    <add key="TeamPassword" value="your-generated-special-key-password" />

