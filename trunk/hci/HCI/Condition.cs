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
        private string fieldName;
        private string tableName;
        private string lowerBound;
        private string upperBound;
        private int lowerOperator;
        private int upperOperator;
        private int id;

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

        public static string upperOperatorIntToString(int op)
        {
            if (op == 1)
                return ">";
            else if (op == 2)
                return ">=";
            else if (op == 3)
                return "<";
            else if (op == 4)
                return "<=";
            else if (op == 5)
                return "==";
            else if (op == 6)
                return "!=";
            else  // NONE or invalid
                return "";
        }

        public static int upperOperatorStringToInt(string op)
        {
            if (op == ">")
                return 1;
            else if (op == ">=")
                return 2;
            else if (op == "<")
                return 3;
            else if (op == "<=")
                return 4;
            else if (op == "==")
                return 5;
            else if (op == "!=")
                return 6;
            else
                return 0;
        }

        public static string lowerOperatorIntToString(int op)
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

        public static int lowerOperatorStringToInt(string op)
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
            return lowerOperatorIntToString(lowerOperator);
        }

        //Retrieve upperOperator
        public string getUpperOperator()
        {
            return upperOperatorIntToString(upperOperator);
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
            this.lowerOperator = lowerOperatorStringToInt(lowerOperator);
        }

        //Set upperOperator
        public void setUpperOperator(int upperOperator)
        {
            this.upperOperator = upperOperator;
        }

        public void setUpperOperator(string upperOperator)
        {
            this.upperOperator = upperOperatorStringToInt(upperOperator);
        }

        public void setId(int ID)
        {
            this.id = ID;
        }
        /// <summary>
        /// below class will be used when editing IconConditions already in the database
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
            else if ((lowerBound != "") && (!double.TryParse(lowerBound, out tempDouble)))
                errorString = "Lower bound is not numeric.";
            else if ((upperBound != "") && (!double.TryParse(upperBound, out tempDouble)))
                errorString = "Upper bound is not numeric.";
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
            else if ((lowerBound != "") && (upperBound != "") && (Convert.ToDouble(lowerBound) >= Convert.ToDouble(upperBound)))
                errorString = "Your lower bound is not less than your upper bound.";

            
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
            if (tableName == condition.getTableName())
            {
                if (row[condition.getFieldName()] != null)
                {
                    if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) != Condition.NONE && Condition.lowerOperatorStringToInt(condition.getLowerOperator()) != Condition.NONE)
                    {
                        Double lower;
                        Double upper;
                        //Check for what type of comparison should be done
                        if (Double.TryParse(condition.getLowerBound(),out lower) && Double.TryParse(condition.getUpperBound(), out upper))
                        {
                            //Double lower = Double.Parse(condition.getLowerBound());
                            //Double upper = Double.Parse(condition.getUpperBound());

                            Double value = Double.Parse(row[condition.getFieldName()].ToString());

                            Boolean passLowerOperator = false;
                            Boolean passUpperOperator = false;

                            //Check your operators against valid operators
                            for (int count = 0; count < 6; count++)
                            {
                                //Check the lower operator
                                if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 1)
                                {
                                    if (lower > value)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 2)
                                {
                                    if (lower >= value)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 3)
                                {
                                    if (lower < value)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 4)
                                {
                                    if (lower <= value)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 5)
                                {
                                    if (lower == value)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 6)
                                {
                                    if (lower != value)
                                    {
                                        passLowerOperator = true;
                                    }
                                }

                                //Check the upper operator
                                if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 1)
                                {
                                    if (upper > value)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 2)
                                {
                                    if (upper >= value)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 3)
                                {
                                    if (upper < value)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 4)
                                {
                                    if (upper <= value)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 5)
                                {
                                    if (upper == value)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 6)
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
                            String lowerS = condition.getLowerBound();
                            String upperS = condition.getUpperBound();

                            String valueS = row[condition.getFieldName()].ToString();

                            Boolean passLowerOperator = false;
                            Boolean passUpperOperator = false;

                            //Check your operators against valid operators
                            for (int count = 0; count < 6; count++)
                            {
                                //Check the lower operator
                                if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 1)
                                {
                                    if (lowerS.CompareTo(valueS) < 0) 
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 2)
                                {
                                    if (lowerS.CompareTo(valueS) <= 0)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 3)
                                {
                                    if (lowerS.CompareTo(valueS) > 0)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 4)
                                {
                                    if (lowerS.CompareTo(valueS) >= 0)
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 5)
                                {
                                    if (lowerS.Equals(valueS))
                                    {
                                        passLowerOperator = true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 6)
                                {
                                    if (!lowerS.Equals(valueS))
                                    {
                                        passLowerOperator = true;
                                    }
                                }

                                //Check the upper operator
                                if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 1)
                                {
                                    if (upperS.CompareTo(valueS) > 0)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 2)
                                {
                                    if (upperS.CompareTo(valueS) >= 0)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 3)
                                {
                                    if (upperS.CompareTo(valueS) < 0)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 4)
                                {
                                    if (upperS.CompareTo(valueS) <= 0)
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 5)
                                {
                                    if (upperS.Equals(valueS))
                                    {
                                        passUpperOperator = true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 6)
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
                    }
                    else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) != Condition.NONE)
                    {
                        Double lower;
                        if (Double.TryParse(condition.getLowerBound(), out lower))
                        {
                            Double value = Double.Parse(row[condition.getFieldName()].ToString());

                            //Check your operators against valid operators
                            for (int count = 0; count < 6; count++)
                            {
                                //Check the lower operator
                                if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 1)
                                {
                                    if (lower > value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 2)
                                {
                                    if (lower >= value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 3)
                                {
                                    if (lower < value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 4)
                                {
                                    if (lower <= value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 5)
                                {
                                    if (lower == value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 6)
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
                                if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 1)
                                {
                                    if (lowerS.CompareTo(value) < 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 2)
                                {
                                    if (lowerS.CompareTo(value) <= 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 3)
                                {
                                    if (lowerS.CompareTo(value) > 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 4)
                                {
                                    if (lowerS.CompareTo(value) >= 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 5)
                                {
                                    if (lowerS.Equals(value))
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.lowerOperatorStringToInt(condition.getLowerOperator()) == 6)
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
                    }
                    else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) != Condition.NONE)
                    {
                        Double upper;
                        if (Double.TryParse(condition.getUpperBound(), out upper))
                        {
                            Double value = Double.Parse(row[condition.getFieldName()].ToString());

                            for (int count = 0; count < 6; count++)
                            {
                                //Check the upper operator
                                if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 1)
                                {
                                    if (upper > value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 2)
                                {
                                    if (upper >= value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 3)
                                {
                                    if (upper < value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 4)
                                {
                                    if (upper <= value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 5)
                                {
                                    if (upper == value)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 6)
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
                                if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 1)
                                {
                                    if (upperS.CompareTo(value) > 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 2)
                                {
                                    if (upperS.CompareTo(value) >= 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 3)
                                {
                                    if (upperS.CompareTo(value) < 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 4)
                                {
                                    if (upperS.CompareTo(value) <= 0)
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 5)
                                {
                                    if (upperS.Equals(value))
                                    {
                                        return true;
                                    }
                                }
                                else if (Condition.upperOperatorStringToInt(condition.getUpperOperator()) == 6)
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

    }
}
