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
    public partial class frmItemMasterCustomFields : Form
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
        public frmItemMasterCustomFields()
        {
            InitializeComponent();
            setConnectionString();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }



        private void frmItemMasterCustomFields_Load(object sender, EventArgs e)
        {
            LoadCustomFields();
        }

        private void LoadCustomFields()
        {

            string tbl = "";
            if (frmMain.CostomizeFormName == "ItemMaster")
            {
                tbl = "tbl_ItemMasterCostomizeFields";

            }
            else if (frmMain.CostomizeFormName == "CustomerMaster")
            {
                tbl = "tbl_CustomerMasterCostomizeFields";
            }
            else if (frmMain.CostomizeFormName == "VendorMaster")
            {
                tbl = "tbl_VendorMasterCostomizeFields";
            }

            String S = "Select * from "+tbl;
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S,ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                txtC1.Text = dt.Rows[i].ItemArray[0].ToString().Trim();
                txtC2.Text = dt.Rows[i].ItemArray[1].ToString().Trim();
                txtC3.Text = dt.Rows[i].ItemArray[2].ToString().Trim();
                txtC4.Text = dt.Rows[i].ItemArray[3].ToString().Trim();
                txtC5.Text = dt.Rows[i].ItemArray[4].ToString().Trim();
                txtC6.Text = dt.Rows[i].ItemArray[5].ToString().Trim();
                txtC7.Text = dt.Rows[i].ItemArray[6].ToString().Trim();
                txtC8.Text = dt.Rows[i].ItemArray[7].ToString().Trim();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            SqlTransaction Trans = con.BeginTransaction();
            try
            {
                SaveCustomSetting(con, Trans);
                Trans.Commit();
                con.Close();
                MessageBox.Show("Save Successfuly");
                txtC1.Enabled = false;
                txtC2.Enabled = false;
                txtC3.Enabled = false;
                txtC4.Enabled = false;
                txtC5.Enabled = false;
                txtC6.Enabled = false;
                txtC7.Enabled = false;
                txtC8.Enabled = false;
                LoadCustomFields();
                btnEdit.Enabled = true;
                btnSave.Enabled = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Trans.Rollback();
                con.Close();
            }
         
        }

        private void SaveCustomSetting(SqlConnection con, SqlTransaction trans)
        {
            string tbl = "";
            if (frmMain.CostomizeFormName == "ItemMaster")
            {
                tbl = "tbl_ItemMasterCostomizeFields";

            }
            else if (frmMain.CostomizeFormName == "CustomerMaster")
            {
                tbl = "tbl_CustomerMasterCostomizeFields";
            }
            else if (frmMain.CostomizeFormName == "VendorMaster")
            {
                tbl = "tbl_VendorMasterCostomizeFields";
            }
            string S1 = "DELETE FROM "+ tbl;

            SqlCommand cmd1 = new SqlCommand(S1, con, trans);
            cmd1.ExecuteNonQuery();


            string S3 = "INSERT INTO "+tbl+" VALUES('" + txtC1.Text.ToString() + "','" + txtC2.Text.ToString() + "','" + txtC3.Text.ToString() + "','" + txtC4.Text.ToString() + "','" + txtC5.Text.ToString() + "','" + txtC6.Text.ToString() + "','" + txtC7.Text.ToString() + "','" + txtC8.Text.ToString() + "')";

            SqlCommand cmd3 = new SqlCommand(S3, con, trans);
            cmd3.ExecuteNonQuery();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            txtC1.Enabled = true;
            txtC2.Enabled = true;
            txtC3.Enabled = true;
            txtC4.Enabled = true;
            txtC5.Enabled = true;
            txtC6.Enabled = true;
            txtC7.Enabled = true;
            txtC8.Enabled = true;
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
