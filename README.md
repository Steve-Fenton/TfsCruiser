# TfsCruiser

TFS Cruiser is a great big visible information radiator to show off:

 - build status
 - test run status
 - basic code forensics (churn)

UPDATED for v2.0 of the Build REST API.

## Build Status

The build status board shows the current build status, as well as a short 
history of your builds.

The board auto-refreshes, and plays an audible alarm when a build breaks.

## Test Run Status

The test run status board shows the current test run status, as well as a short 
history of your test runs.

The board auto-refreshes, and plays an audible alarm when a test fails.

## Forensics

The forensics view gives you insight into areas of churn in your code base.

Due to the nature of churn data, this view does not auto-refresh.

The forensics view is split into left and right panels. The left-hand panel 
shows the folders where changes have been made. These folder tiles can be 
used to navigate your code base.

The right-hand panel shows files that are descendants of your selected area.

Higher levels of churn often suggest areas of code where there will be a 
higher defect density - but all of the numbers and colours in this area 
are relative to your codebase.

Forensic data is cached to reduce calls back to Visual Studio Team Services.

## Viewing

You can view the boards individually, or place them in your preferred tool 
for displaying boards in rotation. There is also a simple HTML viewer that 
can cycle a number of HTML pages with configurable durations (viewer.html).

## General

TFS Cruiser has been designed to work on a big screen, or projected onto the side of your building if you really want to show off.

Hook it up to your build server and TFS Cruiser will display the current status and a history of your builds.

TFS Cruiser was inspired by CruiseControl.NET monitor, [Cruiser](https://github.com/Steve-Fenton/Cruiser)

Config... you just need to tell TFS Cruiser the address of your build server...

    <add key="TeamAccount" value="Fabrikam" />
    <add key="TeamProject" value="CoolProject" />
    <add key="TeamUsername" value="fenton" />
    <add key="TeamPassword" value="your-generated-special-key-password" />

