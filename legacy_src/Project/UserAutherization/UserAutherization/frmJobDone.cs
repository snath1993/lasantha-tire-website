using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmJobDone : Form
    {

        public static string ConnectionString;
 

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public frmJobDone()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void frmJobDone_Load(object sender, EventArgs e)
        {

            SetAutoID();
            this.Text = frmMain.ItemMasterFormName;

        }

        private void SetAutoID()
        {
            string tableName = null;
            if (frmMain.ItemMasterFormName == "jobDoneMaster")
            {
                tableName = "tbl_ItemCustom6";
            }
            else if (frmMain.ItemMasterFormName == "categoryMaster")
            {
                tableName = "tbl_ItemCustom5";
            }
            else if (frmMain.ItemMasterFormName == "CountryMaster")
            {
                tableName = "tbl_ItemCustom2";
            }
            else if (frmMain.ItemMasterFormName == "typeMaster")
            {
                tableName = "tbl_ItemCustom4";
            }
            else if (frmMain.ItemMasterFormName == "brandMaster")
            {
                tableName = "tbl_ItemCustom3";
            }
            else if (frmMain.ItemMasterFormName == "sizeMaster")
            {
                tableName = "tbl_ItemCustom1";
            }

            else if (frmMain.ItemMasterFormName == "WhiteWallMaster")
            {
                tableName = "tbl_ItemCustom7";
            }
            String StrSql = "SELECT max(ID) from " + tableName;
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0 && dt.Rows[0].ItemArray[0].ToString().Trim() != "")
            {
                textBox1.Text = (Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString().Trim()) + 1).ToString();

            }
            else
            {
                textBox1.Text = "1";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            SqlTransaction Trans = con.BeginTransaction();
            try
            {
                SaveJobMaster(con, Trans);
                Trans.Commit();
                con.Close();
                MessageBox.Show("Save Successfuly");
                SetAutoID();
                btnSave.Enabled = false;
                textBox2.Enabled = true;
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Trans.Rollback();
                con.Close();
            }
        }

        private void SaveJobMaster(SqlConnection con, SqlTransaction trans)
        {
            if (frmMain.ItemMasterFormName == "jobDoneMaster")
            {
                string S1 = "DELETE FROM [dbo].[tbl_ItemCustom6] where Id ='" + textBox1.Text.ToString().Trim() + "'";

                SqlCommand cmd1 = new SqlCommand(S1, con, trans);
                cmd1.ExecuteNonQuery();


                string S3 = "INSERT INTO [dbo].[tbl_ItemCustom6] VALUES('" + textBox2.Text.ToString() + "')";

                SqlCommand cmd3 = new SqlCommand(S3, con, trans);
                cmd3.ExecuteNonQuery();
            }

            else if (frmMain.ItemMasterFormName == "categoryMaster")
            {
                string S1 = "DELETE FROM [dbo].[tbl_ItemCustom5] where Id ='" + textBox1.Text.ToString().Trim() + "'";

                SqlCommand cmd1 = new SqlCommand(S1, con, trans);
                cmd1.ExecuteNonQuery();


                string S3 = "INSERT INTO [dbo].[tbl_ItemCustom5] VALUES('" + textBox2.Text.ToString() + "')";

                SqlCommand cmd3 = new SqlCommand(S3, con, trans);
                cmd3.ExecuteNonQuery();
            }

            else if (frmMain.ItemMasterFormName == "CountryMaster")
            {
                string S1 = "DELETE FROM [dbo].[tbl_ItemCustom2] where Id ='" + textBox1.Text.ToString().Trim() + "'";

                SqlCommand cmd1 = new SqlCommand(S1, con, trans);
                cmd1.ExecuteNonQuery();


                string S3 = "INSERT INTO [dbo].[tbl_ItemCustom2] VALUES('" + textBox2.Text.ToString() + "')";

                SqlCommand cmd3 = new SqlCommand(S3, con, trans);
                cmd3.ExecuteNonQuery();
            }

            else if (frmMain.ItemMasterFormName == "typeMaster")
            {
                string S1 = "DELETE FROM [dbo].[tbl_ItemCustom4] where Id ='" + textBox1.Text.ToString().Trim() + "'";

                SqlCommand cmd1 = new SqlCommand(S1, con, trans);
                cmd1.ExecuteNonQuery();


                string S3 = "INSERT INTO [dbo].[tbl_ItemCustom4] VALUES('" + textBox2.Text.ToString() + "')";

                SqlCommand cmd3 = new SqlCommand(S3, con, trans);
                cmd3.ExecuteNonQuery();
            }

            else if (frmMain.ItemMasterFormName == "brandMaster")
            {
                string S1 = "DELETE FROM [dbo].[tbl_ItemCustom3] where Id ='" + textBox1.Text.ToString().Trim() + "'";

                SqlCommand cmd1 = new SqlCommand(S1, con, trans);
                cmd1.ExecuteNonQuery();


                string S3 = "INSERT INTO [dbo].[tbl_ItemCustom3] VALUES('" + textBox2.Text.ToString() + "')";

                SqlCommand cmd3 = new SqlCommand(S3, con, trans);
                cmd3.ExecuteNonQuery();
            }

            else if (frmMain.ItemMasterFormName == "sizeMaster")
            {
                string S1 = "DELETE FROM [dbo].[tbl_ItemCustom1] where Id ='" + textBox1.Text.ToString().Trim() + "'";

                SqlCommand cmd1 = new SqlCommand(S1, con, trans);
                cmd1.ExecuteNonQuery();


                string S3 = "INSERT INTO [dbo].[tbl_ItemCustom1] VALUES('" + textBox2.Text.ToString() + "')";

                SqlCommand cmd3 = new SqlCommand(S3, con, trans);
                cmd3.ExecuteNonQuery();
            }

            else if (frmMain.ItemMasterFormName == "WhiteWallMaster")
            {
                string S1 = "DELETE FROM [dbo].[tbl_ItemCustom7] where Id ='" + textBox1.Text.ToString().Trim() + "'";

                SqlCommand cmd1 = new SqlCommand(S1, con, trans);
                cmd1.ExecuteNonQuery();


                string S3 = "INSERT INTO [dbo].[tbl_ItemCustom7] VALUES('" + textBox2.Text.ToString() + "')";

                SqlCommand cmd3 = new SqlCommand(S3, con, trans);
                cmd3.ExecuteNonQuery();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            frmCustomMasterList cl = new frmCustomMasterList();
            cl.ShowDialog();
            if (Search.CustomSearch == "" || Search.CustomSearch == null)
            {
                return;
            }
            LoadData();
            btnEditer.Enabled = true;
            btnSave.Enabled = false;
            Search.CustomSearch = "";
            DisableFields();
        }

        private void DisableFields()
        {
            textBox2.Enabled = false;
        }

        private void LoadData()
        {
            string tbl = "";
            if (frmMain.ItemMasterFormName == "jobDoneMaster")
            {
                tbl = "[tbl_ItemCustom6]";

               
            }

            else if (frmMain.ItemMasterFormName == "categoryMaster")
            {
                tbl = "[tbl_ItemCustom5]";

             
            }

            else if (frmMain.ItemMasterFormName == "CountryMaster")
            {
                tbl = "[tbl_ItemCustom2]";

              
            }

            else if (frmMain.ItemMasterFormName == "typeMaster")
            {
                tbl = "[tbl_ItemCustom4]";




              
            }

            else if (frmMain.ItemMasterFormName == "brandMaster")
            {
                tbl = "[tbl_ItemCustom3]";

            
            }

            else if (frmMain.ItemMasterFormName == "sizeMaster")
            {
                tbl = "[tbl_ItemCustom1]";


            }

            else if (frmMain.ItemMasterFormName == "WhiteWallMaster")
            {
                tbl = "[tbl_ItemCustom7]";

               
            }
            string ConnString = ConnectionString;
            String S1 = "Select * from "+tbl+ "  where ID='" + Search.CustomSearch + "'";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                                                                                                                                                                                                    // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            DataTable dt = new DataTable();
            da1.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                textBox1.Text = dt.Rows[0].ItemArray[0].ToString();
                textBox2.Text = dt.Rows[0].ItemArray[1].ToString();
               

                

            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            SetAutoID();
            btnSave.Enabled = true;
            textBox2.Enabled = true;
        }

        private void btnEditer_Click(object sender, EventArgs e)
        {
            textBox2.Enabled = true;
            btnSave.Enabled = true;
        }
    }
}
