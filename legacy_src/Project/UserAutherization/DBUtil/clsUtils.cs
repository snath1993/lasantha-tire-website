using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using DataAccess;

namespace DBUtil
{
    public class clsUtils
    {
        public static string conStringSqlServer()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            string constring = objclsDataAccess.StrConectionStringLocal;
            
            //string constring = System.IO.File.ReadAllText(Application.StartupPath + "\\Connection.txt");
            return constring;
        }

        public static DataTable loadItems(string itemsCmdText)
        {
            SqlConnection Conn = new SqlConnection(conStringSqlServer());
            Conn.Open();
            SqlDataAdapter adpt = new SqlDataAdapter(itemsCmdText, Conn);
            DataTable dt = new DataTable();
            adpt.Fill(dt);
            Conn.Close();
            return dt;
        }
    }
}
