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
using HCI;

namespace HCI
{
    public class Condition
    {
        //Globals
        public static readonly int NONE = 0;
        public static readonly int LESSTHAN = 1;
        public static readonly int LESSTHANEQUAL = 2;
        public static readonly int GREATERTHAN = 3;
        public static readonly int GREATERTHANEQUAL = 4;
        public static readonly int EQUAL = 5;
        public static readonly int NOTEQUAL = 6;

        //Data Types
        internal string fieldName;
        internal string tableName;
        internal string lowerBound;
        internal string upperBound;
        internal int lowerOperator;
        internal int upperOperator;
        internal int id;

        //Functions
        
        //Constructor
        public Condition()
        {
            fieldName = "";
            tableName = "";
            lowerBound = "";
            upperBound = "";
            lowerOperator = NONE;
            upperOperator = NONE;
            id = 0;
        }

        public Condition(string FieldName, string TableName, string LowerBound, string UpperBound, string LowerOperator, string UpperOperator)
        {
            fieldName = FieldName;
            tableName = TableName;
            lowerBound = LowerBound;
            upperBound = UpperBound;
            if (LowerOperator != "")
                lowerOperator = Convert.ToInt32(LowerOperator);
            else
                lowerOperator = NONE;
            if (UpperOperator != "")
                upperOperator = Convert.ToInt32(UpperOperator);
            else
                upperOperator = NONE;
            id = 0;
        }

        public static string operatorIntToString(int op)
        {
            if (op == 1)
                return "<";
            else if (op == 2)
                return "<=";
            else if (op == 3)
                return ">";
            else if (op == 4)
                return ">=";
            else if (op == 5)
                return "==";
            else if (op == 6)
                return "!=";
            else  // NONE or invalid
                return "";
        }

        public static int operatorStringToInt(string op)
        {
            if (op == "<")
                return 1;
            else if (op == "<=")
                return 2;
            else if (op == ">")
                return 3;
            else if (op == ">=")
                return 4;
            else if (op == "==")
                return 5;
            else if (op == "!=")
                return 6;
            else
                return 0;
        }

        //Getters

        //Retrieve fieldName
        public string getFieldName()
        {
            return this.fieldName;
        }

        //Retrieve tableName
        public string getTableName()
        {
            return this.tableName;
        }

        //Retrieve lowerBound
        public string getLowerBound()
        {
            return this.lowerBound;
        }

        //Retrieve upperBound
        public string getUpperBound()
        {
            return this.upperBound;
        }

        //Retrieve lowerOperator
        public string getLowerOperator()
        {
            return operatorIntToString(lowerOperator);
        }

        //Retrieve upperOperator
        public string getUpperOperator()
        {
            return operatorIntToString(upperOperator);
        }

        public string getId()
        {
            return this.id.ToString();
        }

        //Setters

        //Set fieldName
        public void setFieldName(string fieldName)
        {
            this.fieldName = fieldName;
        }

        //Set tableName
        public void setTableName(string tableName)
        {
            this.tableName = tableName;
        }

        //Set lowerBound
        public void setLowerBound(string lowerBound)
        {
            this.lowerBound = lowerBound;
        }

        //Set upperBound
        public void setUpperBound(string upperBound)
        {
            this.upperBound = upperBound;
        }

        //Set lowerOperator
        public void setLowerOperator(int lowerOperator)
        {
            this.lowerOperator = lowerOperator;
        }

        public void setLowerOperator(string lowerOperator)
        {
            this.lowerOperator = operatorStringToInt(lowerOperator);
        }

        //Set upperOperator
        public void setUpperOperator(int upperOperator)
        {
            this.upperOperator = upperOperator;
        }

        public void setUpperOperator(string upperOperator)
        {
            this.upperOperator = operatorStringToInt(upperOperator);
        }

        public void setId(int ID)
        {
            this.id = ID;
        }
        /// <summary>
        /// below function will be used when editing IconConditions already in the database
        /// </summary>
        /// <param name="connID"></param>
        /// <param name="iconID"></param>
        public void setIDfromDB(int connID, int iconID)
        {
            Database DB = new Database();
            DataTable id = new DataTable();
            try
            {
                 id = DB.executeQueryLocal("SELECT ID FROM IconCondition WHERE connID=" 
                     + connID + " and iconID=" + iconID + " "
                     + "and lowerBound=\'" + this.lowerBound + "\' "
                     + "and upperBound=\'" + this.upperBound + "\' " 
                     + "and lowerOperator=\'" + this.lowerOperator + "\' " 
                     + "and upperOperator=\'" + this.upperOperator + "\' " 
                     + "and fieldName=\'" + this.fieldName + "\' " 
                     + "and tableName=\'" + this.tableName + "\'");
            }
            catch (ODBC2KMLException ex)
            {
                throw new ODBC2KMLException(ex.errorText);
            }
            foreach (DataRow row in id.Rows)
            {
                this.id = (int)row[0];
            }
        }

        public void setIDfromDBoverlay(int connID, int overlayID)
        {
            Database DB = new Database();
            DataTable id = new DataTable();
            try
            {
                id = DB.executeQueryLocal("SELECT ID FROM OverlayCondition WHERE connID="
                    + connID + " and overlayID=" + overlayID + " "
                    + "and lowerBound=\'" + this.lowerBound + "\' "
                    + "and upperBound=\'" + this.upperBound + "\' "
                    + "and lowerOperator=\'" + this.lowerOperator + "\' "
                    + "and upperOperator=\'" + this.upperOperator + "\' "
                    + "and fieldName=\'" + this.fieldName + "\' "
                    + "and tableName=\'" + this.tableName + "\'");
            }
            catch (ODBC2KMLException ex)
            {
                throw new ODBC2KMLException(ex.errorText);
            }
            foreach (DataRow row in id.Rows)
            {
                this.id = (int)row[0];
            }
        }
        //Additional

        //Add Comments
        public string getErrorText()
        {
            string errorString = "";
            double tempDouble;
            if (tableName == "")
                errorString = "Must enter table name.";
            else if (fieldName == "")
                errorString = "Must enter field name.";
            else if ((lowerBound == "") && (upperBound == ""))
                errorString = "Must enter a lower bound or an upper bound.";
            else if (((lowerBound != "") && (double.TryParse(lowerBound, out tempDouble))) && ((upperBound != "") && (!double.TryParse(upperBound, out tempDouble))))
                errorString = "Cannot mix and match strings and numeric values in conditions.";
            else if (((lowerBound != "") && (!double.TryParse(lowerBound, out tempDouble))) && ((upperBound != "") && (double.TryParse(upperBound, out tempDouble))))
                errorString = "Cannot mix and match strings and numeric values in conditions.";
            else if ((lowerOperator != 0) && (lowerBound == ""))
                errorString = "You entered a lower operator, so you must also enter a lower bound.";
            else if ((lowerBound != "") && (lowerOperator == 0))
                errorString = "You entered a lower bound, so you must also enter a lower operator.";
            else if ((upperOperator != 0) && (upperBound == ""))
                errorString = "You entered a upper operator, so you must also enter an upper bound.";
            else if ((upperBound != "") && (upperOperator == 0))
                errorString = "You entered a upper bound, so you must also enter an upper operator.";
            else if (((upperOperator == 5) && (lowerOperator != 0)) || ((lowerOperator == 5) && (upperOperator != 0)))
                errorString = "You cannot enter one \"==\" operator and also use another operator.";
            else if (((upperOperator == 6) && (lowerOperator != 0)) || ((lowerOperator == 6) && (upperOperator != 0)))
                errorString = "You cannot enter one \"!=\" operator and also use another operator.";
            else if ((lowerBound != "") && (upperBound != "") && (double.TryParse(lowerBound, out tempDouble)) && (Convert.ToDouble(lowerBound) >= Convert.ToDouble(upperBound)))  // numeric case
                errorString = "Your lower bound is not less than your upper bound.";
            else if ((lowerBound != "") && (upperBound != "") && (!double.TryParse(lowerBound, out tempDouble)) && (String.Compare(lowerBound, upperBound)) >= 0)  // string case
                errorString = "Your lower bound is not less than your upper bound.";
            else if (!double.TryParse(lowerBound, out tempDouble) && (lowerBound != "") && (lowerOperator != 5) && (lowerOperator != 6))
                errorString = "You cannot use non-doubles with a comparison operator.";
            else if (!double.TryParse(upperBound, out tempDouble) && (upperBound != "") && (upperOperator != 5) && (upperOperator != 6))
                errorString = "You cannot use non-doubles with a comparison operator.";
            
            return errorString;
        }

        /// <summary>
        /// Evaluate Condition determines if the current row is affected 
        /// by the given condition. If it is, then it returns true; else, 
        /// return false.
        /// </summary>
        /// <param name="row">DataRow --> The DataRow from the current DataTable
        /// that is being analyzed</param>
        /// <param name="condition">Condition --> The condition you want to check
        /// the row against</param>
        /// <returns>Boolean --> Does the condition apply or not?</returns>
        public Boolean evaluateCondition(DataRow row, Condition condition, string tableName)
        {
            if (tableName == condition.getTableName()) //Valid table name?
            {
                if (row[condition.getFieldName()] != null && row[condition.getFieldName()].ToString() != "") //Valid field name?
                {
                    //Both operators set
                    if (Condition.operatorStringToInt(condition.getUpperOperator()) != Condition.NONE && Condition.operatorStringToInt(condition.getLowerOperator()) != Condition.NONE)
                    {
                        //Basic doubles created to be used with Try Parse
                        Double lower;
                        Double upper;

                        //Check for double parse
                        if (Double.TryParse(condition.getLowerBound(),out lower) && Double.TryParse(condition.getUpperBound(), out upper))
                        {
                            //Parsed value
                            Double value = Double.Parse(row[condition.getFieldName()].ToString());

                            //Decision parameters
                            Boolean passLowerOperator = false;
                            Boolean passUpperOperator = false;

                            //Check your operators against valid operators
                            for (int count = 0; count < 6; count++)
                            {
                                //Check the lower operator
                                if (Condition.operatorStringToInt(condition.getLowerOperator()) == 1)
                                {
                                    if (lower < value)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 2)
                                {
                                    if (lower <= value)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 3)
                                {
                                    if (lower > value)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 4)
                                {
                                    if (lower >= value)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 5)
                                {
                                    if (lower == value)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 6)
                                {
                                    if (lower != value)
                                    {
                                        passLowerOperator = true;
                                    }
                                }

                                //Check the upper operator
                                if (Condition.operatorStringToInt(condition.getUpperOperator()) == 1)
                                {
                                    if (upper > value)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 2)
                                {
                                    if (upper >= value)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 3)
                                {
                                    if (upper < value)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 4)
                                {
                                    if (upper <= value)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 5)
                                {
                                    if (upper == value)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 6)
                                {
                                    if (upper != value)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                            }//End for loop

                            //If it meets the condition, return true
                            if (passLowerOperator == true && passUpperOperator == true)
                                return true;

                            //Does not meet the condition
                            return false;
                        }
                        else //Do string comparisons
                        {
                            //String parameters
                            String lowerS = condition.getLowerBound();
                            String upperS = condition.getUpperBound();
                            String valueS = row[condition.getFieldName()].ToString();

                            //Pass parameters
                            Boolean passLowerOperator = false;
                            Boolean passUpperOperator = false;

                            //Check your operators against valid operators
                            for (int count = 0; count < 6; count++)
                            {
                                //Check the lower operator
                                if (Condition.operatorStringToInt(condition.getLowerOperator()) == 1)
                                {
                                    if (lowerS.CompareTo(valueS) < 0) 
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 2)
                                {
                                    if (lowerS.CompareTo(valueS) <= 0)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 3)
                                {
                                    if (lowerS.CompareTo(valueS) > 0)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 4)
                                {
                                    if (lowerS.CompareTo(valueS) >= 0)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 5)
                                {
                                    if (lowerS.Equals(valueS))
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 6)
                                {
                                    if (!lowerS.Equals(valueS))
                                    {
                                        passLowerOperator = true;
                                    }
                                }

                                //Check the upper operator
                                if (Condition.operatorStringToInt(condition.getUpperOperator()) == 1)
                                {
                                    if (upperS.CompareTo(valueS) > 0)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 2)
                                {
                                    if (upperS.CompareTo(valueS) >= 0)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 3)
                                {
                                    if (upperS.CompareTo(valueS) < 0)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 4)
                                {
                                    if (upperS.CompareTo(valueS) <= 0)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 5)
                                {
                                    if (upperS.Equals(valueS))
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 6)
                                {
                                    if (!upperS.Equals(valueS))
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                            }//End for loop

                            //If it meets the condition, return true
                            if (passLowerOperator == true && passUpperOperator == true)
                                return true;

                            //Does not meet the condition
                            return false;
                        }
                    }//Lower operator only
                    else if (Condition.operatorStringToInt(condition.getLowerOperator()) != Condition.NONE)
                    {
                        Double lower;
                        if (Double.TryParse(condition.getLowerBound(), out lower))
                        {
                            Double value = Double.Parse(row[condition.getFieldName()].ToString());

                            //Check your operators against valid operators
                            for (int count = 0; count < 6; count++)
                            {
                                //Check the lower operator
                                if (Condition.operatorStringToInt(condition.getLowerOperator()) == 1)
                                {
                                    if (lower < value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 2)
                                {
                                    if (lower <= value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 3)
                                {
                                    if (lower > value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 4)
                                {
                                    if (lower >= value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 5)
                                {
                                    if (lower == value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 6)
                                {
                                    if (lower != value)
                                    {
                                        return true;
                                    }
                                }
                            }//End for loop

                            //Does not meet condition
                            return false;
                        }
                        else //String comparison
                        {
                            String lowerS = condition.getLowerBound();
                            String value = row[condition.getFieldName()].ToString();

                            //Check your operators against valid operators
                            for (int count = 0; count < 6; count++)
                            {
                                //Check the lower operator
                                if (Condition.operatorStringToInt(condition.getLowerOperator()) == 1)
                                {
                                    if (lowerS.CompareTo(value) < 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 2)
                                {
                                    if (lowerS.CompareTo(value) <= 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 3)
                                {
                                    if (lowerS.CompareTo(value) > 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 4)
                                {
                                    if (lowerS.CompareTo(value) >= 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 5)
                                {
                                    if (lowerS.Equals(value))
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getLowerOperator()) == 6)
                                {
                                    if (!lowerS.Equals(value))
                                    {
                                        return true;
                                    }
                                }
                            }//End for loop

                            //Does not meet condition
                            return false;
                        }
                    }//Upper operator only
                    else if (Condition.operatorStringToInt(condition.getUpperOperator()) != Condition.NONE)
                    {
                        Double upper;
                        if (Double.TryParse(condition.getUpperBound(), out upper))
                        {
                            Double value = Double.Parse(row[condition.getFieldName()].ToString());

                            for (int count = 0; count < 6; count++)
                            {
                                //Check the upper operator
                                if (Condition.operatorStringToInt(condition.getUpperOperator()) == 1)
                                {
                                    if (upper > value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 2)
                                {
                                    if (upper >= value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 3)
                                {
                                    if (upper < value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 4)
                                {
                                    if (upper <= value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 5)
                                {
                                    if (upper == value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 6)
                                {
                                    if (upper != value)
                                    {
                                        return true;
                                    }
                                }
                            }//End for loop
                            
                            //Does not meet condition
                            return false;
                        }
                        else //String comparison
                        {
                            String upperS = condition.getUpperBound();
                            String value = row[condition.getFieldName()].ToString();

                            for (int count = 0; count < 6; count++)
                            {
                                //Check the upper operator
                                if (Condition.operatorStringToInt(condition.getUpperOperator()) == 1)
                                {
                                    if (upperS.CompareTo(value) > 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 2)
                                {
                                    if (upperS.CompareTo(value) >= 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 3)
                                {
                                    if (upperS.CompareTo(value) < 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 4)
                                {
                                    if (upperS.CompareTo(value) <= 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 5)
                                {
                                    if (upperS.Equals(value))
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.operatorStringToInt(condition.getUpperOperator()) == 6)
                                {
                                    if (!upperS.Equals(value))
                                    {
                                        return true;
                                    }
                                }
                            }//End for loop

                            //Does not meet condition
                            return false;
                        }
                    }

                    //If there are no operators
                    return false;
                }

                //If field names are not equal
                return false;
            }

            //If table names are not equal
            return false;
        }//End evaluate Condition

        /// <summary>
        /// isValid checks to see if the table/column name combination still exists.
        /// </summary>
        /// <param name="purgeDT">DataTable --> All of the table names</param>
        /// <param name="columnToTableRelation">DataSet --> All of the columns associated with each table</param>
        /// <returns>if valid, return true, else return false</returns>
        public Boolean isValid(DataTable purgeDT, DataSet columnToTableRelation)
        {
            //For each table name, see if the conditions table name matches
            foreach (DataRow row in purgeDT.Rows)
            {
                String rowName = row["TABLE_NAME"].ToString().ToLower();

                //Do table names match?
                if (rowName.Equals(this.getTableName().ToLower()))
                {
                    //Ok, check the column names
                    foreach (DataRow row1 in columnToTableRelation.Tables[row["TABLE_NAME"].ToString()].Rows)
                    {
                        String columnName = row1["COLUMN_NAME"].ToString().ToLower();

                        if (columnName.Equals((this.getFieldName().ToLower())))
                        {
                            //Table and columns matched
                            return true;
                        }
                    } //End for each

                    //Tables matched but columns did not
                    return false;
                }

            } //End for each

            //No table matches
            return false;
        }
    }
}
