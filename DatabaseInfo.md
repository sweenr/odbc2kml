# Introduction #

There is a problem with the database that I created for our data and put on the SVN. The ID fields in the database do not auto-increment and the location field in IconLibrary  (set to 50) is to small.  Also this describes how to run a query against your local database using visual studio.


# Details #

To fix the first error you may go to each ID that you wish changed to auto-increment and do the following:
1. Open Visual studio, go to tools and click Connect to Database
> ServerName=(yourComputer'sName)\SQLEXPRESS            database is odbc2kml
2. click on odbc2kml.dbo, click Tables
3. right click on the table you desire to change and click "Open table definition"
4. click on primary key, and scroll down on the "column properties" window
5. click the plus next to "Identity specification" and change "(is identity)" to yes
> (Identity increment and Identity seed should set themselves to 1, if not, you should)
> (if you have errors in visual studio, download Microsoft SQL server management studio
> > express and use it)
6. click save

To fix the small size of location do steps 1-3 above
4. click on location, change varchar(50) to (150)
5. click save

To run queries in visual studio do steps 1-2 above
3. right click on the table you desire to change and click new query

# Needed Fixes #
  1. Change format in Mapping table to an int
  1. Add protocol to Connection table as varchar(150)
  1. Add SID to Connection table as varchar(150)
  1. Add serviceName to Connection table as varchar(150)
  1. Add type to Connection table (int)

# More Fixes #
  1. Change your local database for IconCondition and OverlayCondition to have default operator values of 0, NOT NULL.
  1. Change Overlay table in local database: remove connID from primary key, set ID as primary key and autoincrement if you haven't already.
  1. Change Mapping: Add a column named placemarkFieldName, varchar(50), allow null