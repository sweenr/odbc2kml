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
        private string serverAddress;
        private string userName;
        private string password;
        private string databaseName;
        private string portNumber;
        private string connectionName;
        private int connID;

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
