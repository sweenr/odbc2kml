# Introduction #

This wiki has information on how the ODBC2KML project uniformly handles errors.


# Details #

First add the ODBC2KMLException.cs file to your project.

If you are working on a support class (one not tied to an .aspx page) do the following:
  * When you encounter an error, type:
```
throw new ODBC2KMLException("Error Text Here");
```

Refer to the proper section below to figure out how to use ErrorHandler correctly.
If you are working on a .aspx page, you need to catch ODBC2KMLExceptions.
  * Put a try block around any code that could have an error
  * Have this code as your catch block (example using scenario 1 from below):
```
catch (ODBC2KMLException ex)
{
     ErrorHandler eh = new ErrorHandler(ex.errorText, errorPanel1);
     eh.displayError();
     return;
}
```

There are 4 different scenarios for using ErrorHandler, each with their own constructor:
  1. You are in a Panel but not in a ModalPopupExtender
  1. You are in a Panel which is inside a ModalPopupExtender
  1. You are in an UpdatePanel but not in a ModalPopupExtender
  1. You are in an UpdatePanel which is inside a ModalPopupExtender

## Scenario 1 ##
The first argument is the error text to use.
The second argument should be "errorPanel1"

## Scenario 2 ##
The first argument is the error text to use.
The second argument should be "errorPanel1".
The third argument should be the ClientID of the ModalPopupExtender that your Panel is in.

## Scenario 3 ##
The first argument is the error text to use.
The second argument should be the ClientID of the UpdatePanel your code is run from.

## Scenario 4 ##
The first argument is the error text to use.
The second argument should be the ClientID of the UpdatePanel your code is run from.
The third argument should be the ClientID of the ModalPopupExtender that your UpdatePanel is in.


You can look at Database.cs and DBTest.cs as examples.