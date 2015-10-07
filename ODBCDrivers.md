## Introduction ##

This page contains information on where to download ODBC drivers and what drivers to download. It will also make an attempt at explaining how to install and use the drivers.


## Drivers ##

  1. My SQL: http://dev.mysql.com/downloads/connector/net/
  1. My SQL: http://dev.mysql.com/downloads/connector/odbc/
  1. MS SQL and Oracle: http://www.microsoft.com/downloads/details.aspx?displaylang=en&FamilyID=78cac895-efc2-4f8e-a9e0-3a1afbd5922e

You will also need the ODBC .NET Managed Provider which can be obtained here:
http://www.microsoft.com/downloads/details.aspx?familyid=6ccd8427-1017-4f33-a062-d165078e32b1&displaylang=en

## .NET Integration ##

  1. When you download all of the above files, run them and follow the provided directions.
  1. After installing/running all of the drivers and the manager, open Visual Studios.
  1. Within Visual Studios, open the project.
  1. Right click the reference folder and choose add reference.
  1. Under the .NET tab look for Microsoft.Data.Odbc.
  1. Now click it and click ok.
  1. Right click the reference folder and choose add reference.
  1. Under the .NET tab look for MySQL.Data.
  1. Now click it and click ok.
  1. Right click the reference folder and choose add reference.
  1. Under the .NET tab look for System.Data.OracleClient.
  1. Now click it and click ok.

Now both drivers should be incorporated into the project.

## Oracle InstantClient ##
Regardless of whether you have 32-bit or 64-bit install the 32-bit versions linked below.
  1. Download: http://download.oracle.com/otn/nt/instantclient/111070/instantclient-basic-win32-11.1.0.7.0.zip
  1. Extract to c:\instantclient
  1. Download: http://download.oracle.com/otn/nt/instantclient/111070/instantclient-odbc-win32-11.1.0.7.0.zip
  1. Extract to c:\instantclient
  1. Add c:\instantclient to your PATH variable
  1. Go into c:\instantclient and find the file called odbc\_install and install it.