TfsCruiser
==========

TFS Cruiser is a great big visible information radiator to show off:

 - build status
 - test run status
 - basic code forensics (churn)

UPDATED for v2.0 of the Build REST API.

It has been designed to work on a big screen, or projected onto the side of your building if you really want to show off.

Hook it up to your build server and TFS Cruiser will display the current status and a history of your builds.

TFS Cruiser was inspired by CruiseControl.NET monitor, [Cruiser](https://github.com/Steve-Fenton/Cruiser)

Config... you just need to tell TFS Cruiser the address of your build server...

    <add key="TeamAccount" value="Fabrikam" />
    <add key="TeamProject" value="CoolProject" />
    <add key="TeamUsername" value="fenton" />
    <add key="TeamPassword" value="your-generated-special-key-password" />

You can also connect [TFS Cruiser to Visual Studio Online](https://www.stevefenton.co.uk/2015/05/Connect-TFS-Cruiser-To-Visual-Studio-Online/).
