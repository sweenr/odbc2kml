using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace HCI
{
    public class ConnInfo
    {
        //Constants for different databases
        public const int MYSQL = 0;
        public const int MSSQL = 1;
        public const int ORACLE = 2;
        //Add More as needed

        //Datatypes
        private int databaseType;
        private string serverAddress;
        private string userName;
        private string password;
        private string databaseName;
        private string portNumber;
        private string connectionName;

        //Functions
        public int getDatabaseType()
        {
            return this.databaseType;
        }

        public string getServerAddress()
        {
            return this.serverAddress;
        }

        public string getUserName()
        {
            return this.userName;
        }

        public string getPassword()
        {
            return this.password;
        }

        public string getPortNumber()
        {
            return this.portNumber;
        }

        public string getConnectionName()
        {
            return this.connectionName;
        }

        public string getDatabaseName()
        {
            return this.databaseName;
        }

        public void setDatabaseType(int databaseType)
        {
            this.databaseType = databaseType;
        }

        public void setServerAddress(string serverAddr)
        {
            this.serverAddress = serverAddr;
        }

        public void setUserName(string userName)
        {
            this.userName = userName;
        }

        public void setPassword(string password)
        {
            this.password = password;
        }

        public void setPortNumber(string portNum)
        {
            this.portNumber = portNum;
        }

        public void setConnectionName(string connName)
        {
            this.connectionName = connName;
        }

        public void setDatabaseName(string dbName)
        {
            this.databaseName = dbName;
        }

        public bool isValid()
        {
            bool valid = false;


            return valid;

        }
    }
}
