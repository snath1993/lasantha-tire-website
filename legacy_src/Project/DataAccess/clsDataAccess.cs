using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Sql;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

// Author:	<Tharaka>
// Create date: <10/08/2010>
//Modified By :
namespace DataAccess
{

    public class clsDataAccess
    {
        public string str_ConnectionString = null;
        //public string str_ConnectionString_evl = null;
        public SqlConnection obj_Conn;
        public SqlTransaction obj_SqlTransation = null;
        public SqlCommand obj_cmd = new SqlCommand();
        DataTable obj_Datatable = null;
        DataSet obj_Dataset = null;
        SqlDataAdapter obj_DataAdapter = null;
        string connStr_TextFile = "ConnectionString.txt";
        string Server = "";
        string Database = "";
        string UserID = "";
        string pwd = "";
        string DatabaseEvl = "";
        bool IntegratedSecurity = false;

        clsCommon objclsCommon = new clsCommon();

        private string strConectionStringLocal;

        public string StrConectionStringLocal
        {
            get { return strConectionStringLocal; }
            set { strConectionStringLocal = value; }
        }

        private string strConectionStringEvl;
        public string StrConectionStringEvl
        {
            get { return strConectionStringEvl; }
            set { strConectionStringEvl = value; }
        }        

        public clsDataAccess()
        {
            try
            {
                //TextReader tr = new StreamReader(Application.StartupPath + "\\Connection1.txt");
                //str_ConnectionString = tr.ReadLine();
                //tr.Close();

                GetDataFromTextFile();

                strConectionStringLocal = "Data Source=" + Server + ";Initial Catalog=" + Database + ";Persist Security Info = True;User ID=" + UserID + ";Password=" + pwd + ";Connection timeout=50000;";
                strConectionStringEvl = "Data Source=" + Server + ";Initial Catalog=" + DatabaseEvl + ";Persist Security Info = True;User ID=" + UserID + ";Password=" + pwd + ";Connection timeout=50000;";

                //TextReader tr_evl = new StreamReader(Application.StartupPath + "\\Connection.txt");
                //str_ConnectionString_evl = tr_evl.ReadLine();
                //tr_evl.Close();            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GetDataFromTextFile()
        {
            try
            {

               // TextReader tr = new StreamReader(Application.StartupPath + "\\config.txt"); 
                TextReader tr = new StreamReader(Application.StartupPath + "\\config.txt"); 

                Server = objclsCommon.decryptPassword(tr.ReadLine());
                UserID = objclsCommon.decryptPassword(tr.ReadLine());
                pwd = objclsCommon.decryptPassword(tr.ReadLine());
                Database = objclsCommon.decryptPassword(tr.ReadLine());
                DatabaseEvl = objclsCommon.decryptPassword(tr.ReadLine());
                tr.Close();

                if (string.IsNullOrEmpty(Database))
                {
                    Database = UserID;
                    UserID = "";
                    DatabaseEvl = pwd;
                    pwd = "";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //open the database connection
        private void OpenDataBase()
        {
            try
            {
                obj_Conn = new SqlConnection();
                obj_Conn.ConnectionString = strConectionStringLocal;
                obj_cmd.Connection = obj_Conn;
                obj_cmd.CommandTimeout = 6000;
                obj_Conn.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //private void OpenDataBase_Evl()
        //{
        //    try
        //    {
        //        obj_Conn = new SqlConnection();
        //        obj_Conn.ConnectionString = str_ConnectionString_evl;
        //        obj_cmd.Connection = obj_Conn;
        //        obj_Conn.Open();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //close the databse connection
        private void CloseDataBase()
        {
            try
            {
                obj_Conn.Close();
                obj_Conn.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //begin transaction
        public void BeginTransaction()
        {
            try
            {
                OpenDataBase();
                obj_SqlTransation = obj_Conn.BeginTransaction();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void BeginTransaction_Evl()
        //{
        //    try
        //    {
        //        OpenDataBase_Evl();
        //        obj_SqlTransation = obj_Conn.BeginTransaction();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //commit transaction
        public void CommitTransaction()
        {
            try
            {
                obj_cmd.Transaction = obj_SqlTransation;
                obj_SqlTransation.Commit();
                CloseDataBase();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //rolback the transaction
        public void RollBackTransaction()
        {
            try
            {
                obj_SqlTransation.Rollback();
                obj_SqlTransation.Dispose();
                CloseDataBase();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //execute an stoerd procedure and return a dataset
        public DataSet ExecuteSPReturnDataset(string Sp, SqlParameter[] Param)
        {
            try
            {
                OpenDataBase();
                obj_DataAdapter = new SqlDataAdapter();
                obj_cmd.CommandType = CommandType.StoredProcedure;
                obj_cmd.CommandText = Sp;
                obj_cmd.Parameters.Clear();
                if (Param.Length > 0)
                    obj_cmd.Parameters.AddRange(Param);
                obj_DataAdapter.SelectCommand = obj_cmd;
                obj_Dataset = new DataSet();
                obj_DataAdapter.Fill(obj_Dataset);
                obj_cmd.Dispose();
                CloseDataBase();
            }
            catch (Exception ex)
            {
                obj_cmd.Dispose();
                CloseDataBase();
                throw ex;
            }
            return obj_Dataset;
        }

        public DataSet ExecuteSPReturnDataset_WithoutOpen(string Sp, SqlParameter[] Param)
        {
            try
            {
                //OpenDataBase();
                obj_DataAdapter = new SqlDataAdapter();
                obj_cmd.CommandType = CommandType.StoredProcedure;
                obj_cmd.CommandText = Sp;
                obj_cmd.Parameters.Clear();
                if (Param.Length > 0)
                    obj_cmd.Parameters.AddRange(Param);
                obj_DataAdapter.SelectCommand = obj_cmd;
                obj_Dataset = new DataSet();
                obj_DataAdapter.Fill(obj_Dataset);
                obj_cmd.Dispose();
                //CloseDataBase();
            }
            catch (Exception ex)
            {
                obj_cmd.Dispose();
                //CloseDataBase();
                throw ex;
            }
            return obj_Dataset;
        }

        //execute an stoerd procedure and return a datatable
        public DataTable ExecuteSPReturnDataTable(string Sp, SqlParameter[] Param)
        {
            try
            {
                OpenDataBase();
                obj_DataAdapter = new SqlDataAdapter();
                obj_cmd.CommandType = CommandType.StoredProcedure;
                obj_cmd.CommandText = Sp;
                obj_cmd.Parameters.Clear();
                if (Param.Length > 0)
                    obj_cmd.Parameters.AddRange(Param);
                obj_DataAdapter.SelectCommand = obj_cmd;
                obj_Datatable = new DataTable();
                obj_DataAdapter.Fill(obj_Datatable);
                obj_cmd.Dispose();
                CloseDataBase();
            }
            catch (Exception ex)
            {
                obj_cmd.Dispose();
                //CloseDataBase();
                throw ex;
            }
            return obj_Datatable;
        }

        //execute an stoerd procedure and return a integer
        public int ExecuteSPReturnInteger(string Sp, SqlParameter[] Param)
        {
            int effectedRows = 0;
            try
            {
               // OpenDataBase();
                obj_cmd.CommandType = CommandType.StoredProcedure;
                obj_cmd.Parameters.Clear();
                obj_cmd.Parameters.AddRange(Param);
                obj_cmd.CommandText = Sp;
                obj_cmd.Transaction = obj_SqlTransation;
                effectedRows = obj_cmd.ExecuteNonQuery();
                obj_cmd.Dispose();
                //CloseDataBase();
            }
            catch (Exception ex)
            {
                obj_cmd.Dispose();
                //CloseDataBase();
                throw ex;
            }
            return effectedRows;
        }

        //execute an stoerd procedure and return a object
        public object ExecuteSPReturnObject(string Sp, SqlParameter[] Param)
        {
            object obj = null;
            try
            {
                //OpenDataBase();
                obj_cmd.CommandType = CommandType.StoredProcedure;
                obj_cmd.Parameters.Clear();
                obj_cmd.Parameters.AddRange(Param);
                obj_cmd.CommandText = Sp;
                obj_cmd.Transaction = obj_SqlTransation;
                obj = obj_cmd.ExecuteScalar();
                obj_cmd.Dispose();
                //CloseDataBase();
            }
            catch (Exception ex)
            {
                obj_cmd.Dispose();
                //CloseDataBase();
                throw ex;
            }
            return obj;
        }

        public int ExecuteSQLReturnInteger(string sSQL)
        {
            int effectedRows = 0;
            try
            {
                // OpenDataBase();
                obj_cmd.CommandType = CommandType.Text;
                //obj_cmd.Parameters.Clear();
                //obj_cmd.Parameters.AddRange(Param);
                obj_cmd.CommandText = sSQL;
                obj_cmd.Transaction = obj_SqlTransation;
                effectedRows = obj_cmd.ExecuteNonQuery();
                obj_cmd.Dispose();
                //CloseDataBase();
            }
            catch (Exception ex)
            {
                obj_cmd.Dispose();
                //CloseDataBase();
                throw ex;
            }
            return effectedRows;
        }

        public object ExecuteSQLReturnObject(string sSQL)
        {
            object obj = null;
            try
            {
                // OpenDataBase();
                obj_cmd.CommandType = CommandType.Text;
                //obj_cmd.Parameters.Clear();
                //obj_cmd.Parameters.AddRange(Param);
                obj_cmd.CommandText = sSQL;
                obj_cmd.Transaction = obj_SqlTransation;
                obj = obj_cmd.ExecuteScalar();
                obj_cmd.Dispose();
                //CloseDataBase();
            }
            catch (Exception ex)
            {
                obj_cmd.Dispose();
                //CloseDataBase();
                throw ex;
            }
            return obj;
        }


        public DataTable ExecuteSQLReturnDataTable_WithoutOpen(string sSQl)
        {
            try
            {
                OpenDataBase();
                obj_DataAdapter = new SqlDataAdapter();
                obj_cmd.CommandType = CommandType.Text;
                obj_cmd.CommandText = sSQl;
                //obj_cmd.Parameters.Clear();
                //if (Param.Length > 0)
                //    obj_cmd.Parameters.AddRange(Param);
                obj_DataAdapter.SelectCommand = obj_cmd;
                obj_Datatable = new DataTable();
                obj_DataAdapter.Fill(obj_Datatable);
                obj_cmd.Dispose();
                CloseDataBase();
            }
            catch (Exception ex)
            {
                obj_cmd.Dispose();
                //CloseDataBase();
                throw ex;
            }
            return obj_Datatable;
        }

        public DataTable ExecuteSQLReturnDataTable_Trans(string sSQl)
        {
            try
            {
                //OpenDataBase();
                obj_DataAdapter = new SqlDataAdapter();
                obj_cmd.CommandType = CommandType.Text;
                obj_cmd.CommandText = sSQl;
                //obj_cmd.Parameters.Clear();
                //if (Param.Length > 0)
                //    obj_cmd.Parameters.AddRange(Param);
                obj_DataAdapter.SelectCommand = obj_cmd;
                obj_Datatable = new DataTable();
                obj_DataAdapter.Fill(obj_Datatable);
                obj_cmd.Dispose();
                //CloseDataBase();
            }
            catch (Exception ex)
            {
                obj_cmd.Dispose();
                //CloseDataBase();
                throw ex;
            }
            return obj_Datatable;
        }

        public DataTable ExecuteSQLFillDataTable(string sSQl,string TablName,DataSet dataset)
        {
            try
            {
                OpenDataBase();
                obj_DataAdapter = new SqlDataAdapter();
                obj_cmd.CommandType = CommandType.Text;
                obj_cmd.CommandText = sSQl;
                //obj_cmd.Parameters.Clear();
                //if (Param.Length > 0)
                //    obj_cmd.Parameters.AddRange(Param);
                obj_DataAdapter.SelectCommand = obj_cmd;
                obj_Datatable = new DataTable();
                obj_DataAdapter.Fill(obj_Datatable);
                obj_DataAdapter.Fill(dataset, TablName);
                obj_cmd.Dispose();
                CloseDataBase();
            }
            catch (Exception ex)
            {
                obj_cmd.Dispose();
                //CloseDataBase();
                throw ex;
            }
            return obj_Datatable;
        }


    }
}
