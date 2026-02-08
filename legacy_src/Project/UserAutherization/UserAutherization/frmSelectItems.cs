using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmSelectItems : Form
    {
       
        public frmSelectItems()
        {
            InitializeComponent();
            setConnectionString();
        }
        Class1 a = new Class1();
        public static string ConnectionString;
        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

        private void frmSelectItems_Load(object sender, EventArgs e)
        {
            LoadItemDetails();
        }

        private void LoadItemDetails()
        {
            try
            {
                string ConnString = ConnectionString;
                String S1 = "Select ItemID,ItemDescription from tblItemMaster";// where ItemID like '%2OP'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    dgvSelectItemList.Rows.Add();
                    dgvSelectItemList.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                    dgvSelectItemList.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                }
            }

            catch { }
        }

        private void dgvSelectItemList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string val = dgvSelectItemList[0, dgvSelectItemList.CurrentRow.Index].Value.ToString().Trim();
                Class1.myvalue = a.GetNext2(val);
                this.Close();
            }
            catch { }
        }

        private void txtitemID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtitemID.Text != "")
                {
                    txtDescription.Text = "";
                    string add = txtitemID.Text;
                    dgvSelectItemList.Rows.Clear();
                    string ConnString = ConnectionString;
                    String S1 = "Select ItemID,ItemDescription from tblItemMaster where ItemID LIKE '" + add + "%'";//%2OP'"; ";// where ItemID like '%2OP'";
                    // String S1 = "Select ItemID, ItemDescription,LastUnitCost from tblScanItemMaster where ItemID LIKE  '" + add + "%2OP'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);

                    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        dgvSelectItemList.Rows.Add();
                        dgvSelectItemList.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                        dgvSelectItemList.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                    }
                }
                else 
                {
                    LoadItemDetails();
                }
            }
            catch { }
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            if (txtDescription.Text != "")
            {
                txtitemID.Text = "";
                string add = txtDescription.Text;
                dgvSelectItemList.Rows.Clear();


                string ConnString = ConnectionString;
                String S1 = "Select ItemID,ItemDescription from tblItemMaster where ItemDescription LIKE  '" + add + "%'";//%2OP'"; ";// where ItemID like '%2OP'";
                // String S1 = "Select ItemID, ItemDescription,LastUnitCost from tblScanItemMaster where ItemID LIKE  '" + add + "%2OP'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    dgvSelectItemList.Rows.Add();
                    dgvSelectItemList.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                    dgvSelectItemList.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                }
            }
            else 
            {
                LoadItemDetails();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            LoadItemDetails();
        }

        private void dgvSelectItemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string val = dgvSelectItemList[0, dgvSelectItemList.CurrentRow.Index].Value.ToString().Trim();
                Class1.myvalue = a.GetNext2(val);
                this.Close();

            }
        }

        private void txtitemID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    string val = dgvSelectItemList[0, dgvSelectItemList.CurrentRow.Index].Value.ToString().Trim();
                    Class1.myvalue = a.GetNext2(val);
                    this.Close();
                }
                catch { }
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    string val = dgvSelectItemList[0, dgvSelectItemList.CurrentRow.Index].Value.ToString().Trim();
                    Class1.myvalue = a.GetNext2(val);
                    this.Close();
                }
                catch { }
            }
        }
    }
}