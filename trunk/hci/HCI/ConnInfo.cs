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
using System.Net.Sockets;
using System.Net;
using HCI;

namespace HCI
{
    public class ConnInfo
    {
        //Constants for different databases
        public static readonly int MYSQL = 0;
        public static readonly int MSSQL = 1;
        public static readonly int ORACLE = 2;
        //Add More as needed

        //Datatypes
        internal int databaseType;
        internal string serverAddress;
        internal string userName;
        internal string password;
        internal string databaseName;
        internal string portNumber;
        internal string connectionName;
        internal string oracleProtocol;
        internal string oracleServiceName;
        internal string oracleSID;

        //Constructors
        public ConnInfo()
        {
            oracleProtocol = "";
            oracleServiceName = "";
            oracleSID = "";

        }

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

        public string getOracleProtocol()
        {
            return this.oracleProtocol;
        }

        public string getOracleServiceName()
        {
            return this.oracleServiceName;
        }

        public string getOracleSID()
        {
            return this.oracleSID;
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

        public void setOracleProtocol(string oracleProtocol)
        {
            this.oracleProtocol = oracleProtocol;
        }

        public void setOracleServiceName(string oracleServiceName)
        {
            this.oracleServiceName = oracleServiceName;
        }

        public void setOracleSID(string oracleSID)
        {
            this.oracleSID = oracleSID;
        }

        public bool isValid()
        {
            //If oracle specific information is missing for oracle, return false
            if (this.databaseType == ConnInfo.ORACLE)
            {
                if (!ConnInfo.validSName(this.oracleServiceName) && !ConnInfo.validSID(this.oracleSID))
                {
                    return false;
                }
                if (!ConnInfo.validProtocol(this.oracleProtocol))
                {
                    return false;
                }
            }

            //If anything specific to any connection is missing, return false
            if (!ConnInfo.validConnName(this.connectionName))
            {
                return false;
            }
            else if (!ConnInfo.validDBAddress(this.serverAddress))
            {
                return false;
            }
            else if (!ConnInfo.validPort(this.portNumber))
            {
                return false;
            }
            else if (!ConnInfo.validDBName(this.databaseName))
            {
                return false;
            }
            else if (!ConnInfo.validUserName(this.userName))
            {
                return false;
            }
            else if (!ConnInfo.validPassword(this.password))
            {
                return false;
            }

            //All is good!
            return true;

        }

        public static ConnInfo getConnInfo(int connID)
        {
            ConnInfo connInfo = new ConnInfo();
            Database localDatabase = new Database();

            //Construct the connInfo query and retrieve the DataTable
            string query = "SELECT * FROM Connection WHERE ID=" + connID;
            DataTable table = localDatabase.executeQueryLocal(query);

            //Cycle through each row and column
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    //Set all connInfo
                    switch (col.ColumnName)
                    {
                        case "name":
                            connInfo.setConnectionName(row[col].ToString());
                            break;
                        case "dbName":
                            connInfo.setDatabaseName(row[col].ToString());
                            break;
                        case "userName":
                            connInfo.setUserName(row[col].ToString());
                            break;
                        case "password":
                            connInfo.setPassword(row[col].ToString());
                            break;
                        case "port":
                            connInfo.setPortNumber(row[col].ToString());
                            break;
                        case "address":
                            connInfo.setServerAddress(row[col].ToString());
                            break;
                        case "type":
                            connInfo.setDatabaseType((int)row[col]);
                            break;
                        case "protocol":
                            connInfo.setOracleProtocol(row[col].ToString());
                            break;
                        case "serviceName":
                            connInfo.setOracleServiceName(row[col].ToString());
                            break;
                        case "SID":
                            connInfo.setOracleSID(row[col].ToString());
                            break;
                        default:
                            break;
                    }
                }
            }//End outer loop

            return connInfo;
        }

        /// <summary>
        /// Validates the connection name. Only use is to ensure the name is not
        /// an empty string.
        /// </summary>
        /// <param name="name">String --> Connection string to validate</param>
        /// <returns>Boolean --> true -> Valid, False -> Invalid</returns>
        public static Boolean validConnName(String name)
        {
            //Empty connection name
            if(name.Equals(""))
            {
                return false;
            }

            //Create database and execute query
            Database DB = new Database();
            String query = "SELECT * FROM Connection WHERE name='" + name + "'";

            DataTable DT = DB.executeQueryLocal(query);

            //If the row number isn't 0, return false
            if (DT.Rows.Count > 0)
                return false;


            //Valid name
            return true;
        }

        /// <summary>
        /// Validates Database address. Can currently only check to actual text.
        /// </summary>
        /// <param name="dBAddress">String --> Database address, ip or hostname </param>
        /// <returns>Boolean --> true -> valid, false -> invalid</returns>
        public static Boolean validDBAddress(String dBAddress)
        {
            if (dBAddress.Equals(""))
            {
                return false;
            }

            return true;

           /* IPAddress[] ip;

            try
            {
                ip = Dns.GetHostAddresses(dBAddress);

                //Could not be resolved into an IP address
                if (ip.GetLength(1) > 0)
                {
                    return false;
                }

                //Valid dBAddress
                return true;
            }
            catch (Exception e) //Invalid, or no connection can be made to verify
            {
                return false;
            }*/
        }

        /// <summary>
        /// Verifies if the port is a proper port number. Returns false if the port is 
        /// a string or less than 1/greater than 65535.
        /// </summary>
        /// <param name="port">String --> Port number</param>
        /// <returns>Boolean --> false -> invalid, true -> valid</returns>
        public static Boolean validPort(String port)
        {
            Double portNum;

            //Port number is too short or too long
            if (port.Length > 5 || port.Length < 1)
            {
                return false;
            }

            if (Double.TryParse(port, out portNum))
            {
                //Port number is within an invalid range
                if (portNum < 1 || portNum > 65535)
                {
                    return false;
                }
                else //Valid port number
                {
                    return true;
                }
            }

            //Cannot be parsed into a double
            return false;
        }

        /// <summary>
        /// Validates the user name. Only use is to ensure the name is not
        /// an empty string.
        /// </summary>
        /// <param name="name">String --> Username string to validate</param>
        /// <returns>Boolean --> true -> Valid, False -> Invalid</returns>
        public static Boolean validUserName(String name)
        {
            //Empty connection name
            if (name.Equals(""))
            {
                return false;
            }

            //Valid name
            return true;
        }

        /// <summary>
        /// Validates the database name. Only use is to ensure the name is not
        /// an empty string.
        /// </summary>
        /// <param name="name">String --> DBName string to validate</param>
        /// <returns>Boolean --> true -> Valid, False -> Invalid</returns>
        public static Boolean validDBName(String name)
        {
            //Empty connection name
            if (name.Equals(""))
            {
                return false;
            }

            //Valid name
            return true;
        }

        /// <summary>
        /// Validates the password. Only use is to ensure the password is not
        /// an empty string.
        /// </summary>
        /// <param name="name">String --> Password string to validate</param>
        /// <returns>Boolean --> true -> Valid, False -> Invalid</returns>
        public static Boolean validPassword(String pass)
        {
            //Empty connection name
            if (pass.Equals(""))
            {
                return false;
            }

            //Valid name
            return true;
        }

        /// <summary>
        /// Validates oracle protocols. Currently only supports SDP, TCP, and TCPS protoocols.
        /// IPC and NMP are not supported.
        /// </summary>
        /// <param name="protocol">String --> Protocol to be validated</param>
        /// <returns>Boolean --> true -> valid, false -> invalid</returns>
        public static Boolean validProtocol(String protocol)
        {
            protocol = protocol.ToLower();

            if (protocol.Equals("sdp"))
            {
                return true;
            }
            else if (protocol.Equals("tcp"))
            {
                return true;
            }
            else if (protocol.Equals("tcps"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validates a SID. Currently only checks to make sure the SID is not empty.
        /// </summary>
        /// <param name="SID">String --> Service ID</param>
        /// <returns>Boolean --> true -> valid, false -> invalid</returns>
        public static Boolean validSID(string SID)
        {
            //empty SID
            if(SID.Equals(""))
            {
                return false;
            }

            //Non-empty
            return true;
        }

        /// <summary>
        /// Validates a Service name. Currently only checks to make sure the Service name is not empty.
        /// </summary>
        /// <param name="SID">String --> Service name</param>
        /// <returns>Boolean --> true -> valid, false -> invalid</returns>
        public static Boolean validSName(string SName)
        {
            //empty SID
            if(SName.Equals(""))
            {
                return false;
            }

            //Non-empty
            return true;
        }



    }
}
