using System;

namespace HCI
{
    public class ODBC2KMLException : Exception
    {
        public string errorText;

        public ODBC2KMLException(string text)
        {
            errorText = text;
        }
    }
}