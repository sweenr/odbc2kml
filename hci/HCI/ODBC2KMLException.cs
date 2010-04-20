using System;

namespace ODBC2KML
{
    public class ODBC2KMLException : Exception
    {
        public string errorText;
        public string conErrorText = "Database Error - Invalid Connection ID";
        public string tblErrorText = "Database Error - Invalid Table Name";
        public ODBC2KMLException(string text)
        {
            errorText = text;
        }
    }
}