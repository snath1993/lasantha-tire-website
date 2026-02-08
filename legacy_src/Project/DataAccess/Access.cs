using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DataAccess
{
    public sealed class Access
    {
        //public static bool IsRun;

        public static string ConnectionString;
        public static string type = "";
        public static string Consultant = "";

        public static void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                //TextReader tr = new StreamReader("Connection.txt");
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
                //tr.Close();
            }
            catch { }
        }

        public static string EmployeeID = "";

        //-------------------------------
        public static DataTable setUserAuthentication(string userName, string Activity)
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            //TextReader tr = new StreamReader("Connection.txt");
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
            //tr.Close();

            String S1 = "select * from UserAuthentication where ActivityName = '" + Activity + "' and UserName = '" + userName + "'";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlConnection con1 = new SqlConnection(ConnectionString);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, con1);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            return dt1;       
         
            //IsRun = Convert.ToBoolean(dt1.Rows[0].ItemArray[2].ToString().Trim());            
        }

        //-------------------------------
    }
}
