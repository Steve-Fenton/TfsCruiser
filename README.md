TfsCruiser
==========

TFS Cruiser is a great big visible information radiator to show off the heath of your TFS builds.

It has been designed to work on a big screen, or projected onto the side of your building if you really want to show off.

Hook it up to your build server and TFS Cruiser will display the current status and a history of your builds.

TFS Cruiser was inspired by CruiseControl.NET monitor, Cruiser - http://www.stevefenton.co.uk/Content/Cruiser/

Config... you just need to tell TFS Cruiser the address of your build server...

    <add key="TfsServerAddress" value="http://BUILD-SERVER:8080/tfs/"/>
    <add key="MaxDaysToDisplay" value="4"/>
    <add key="MaxRunsToDisplay" value="11"/>

