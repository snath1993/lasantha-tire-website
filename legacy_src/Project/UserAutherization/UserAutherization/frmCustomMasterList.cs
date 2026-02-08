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
    public partial class frmCustomMasterList : Form
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
        public frmCustomMasterList()
        {
            InitializeComponent();
            setConnectionString();

        }

        private void frmCustomMasterList_Load(object sender, EventArgs e)
        {
            SetList();
        }

        private void SetList()
        {
            string tableName = null;
            if(frmMain.ItemMasterFormName== "jobDoneMaster")
            {
                tableName = "tbl_ItemCustom6";
            }
            else if(frmMain.ItemMasterFormName == "categoryMaster")
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
            String StrSql = "SELECT ID,Description from "+tableName;
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvSearchCustomer.Rows.Clear();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dgvSearchCustomer.Rows.Add();
                    dgvSearchCustomer.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                    dgvSearchCustomer.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                  
                }
            }
        }

        private void dgvSearchCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                string val = dgvSearchCustomer[0, dgvSearchCustomer.CurrentRow.Index].Value.ToString().Trim();
                Search.CustomSearch = val;
                this.Close();
            }
            catch { }
        }
    }
}
