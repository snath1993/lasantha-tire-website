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
    public partial class InpatientMasterList : Form
    {
        public static string ConnectionString;
        public InpatientMasterList()
        {
            InitializeComponent();
            setConnectionString();
        }
        private void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }

        }

        private void InpatientMasterList_Load(object sender, EventArgs e)
        {
            LoadCustomerList();
        }
        private void LoadCustomerList()
        {
            try
            {
                string s = "SELECT * FROM tblCustomerMaster ORDER BY CutomerID";
                SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                
                dgvSearchEmployee.Rows.Clear();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvSearchEmployee.Rows.Add();
                        dgvSearchEmployee.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[26].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[6].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[7].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[7].Value = dt.Rows[i].ItemArray[8].ToString().Trim();
                        string s1 = "SELECT AdmitDate FROM tblPatientAddmission WHERE CustomerID='" + dt.Rows[i].ItemArray[0].ToString().Trim() + "'";
                        SqlDataAdapter da1 = new SqlDataAdapter(s1, ConnectionString);
                        DataTable dt1 = new DataTable();
                        da1.Fill(dt1);
                        for (int j = 0; j < dt1.Rows.Count; j++)
                        {
                            DateTime abc = Convert.ToDateTime(dt1.Rows[j].ItemArray[0]);
                            dgvSearchEmployee.Rows[i].Cells[8].Value = abc.ToShortDateString();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            dgvSearchEmployee.Rows.Clear();
            cmbSearchby.SelectedIndex = -1;
            txtSearch.Text = string.Empty;
            txtSearch.Enabled = true;
            dtpSearchDate.Value = DateTime.Today;
            btnSearchByDate.Enabled = false;
            InpatientMasterList_Load(sender, e);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SeachOption();
        }
        private void SeachOption()
        {
            try
            {
                dgvSearchEmployee.Rows.Clear();

                String StrCode = null;

                if (cmbSearchby.Text.Trim() == "")
                {
                    return;
                }

                switch (cmbSearchby.SelectedIndex)
                {
                    case 0:
                        {
                            StrCode = "CutomerID";

                            break;
                        }
                    case 1:
                        {
                            StrCode = "Cus_Type";
                            break;
                        }
                    case 2:
                        {
                            StrCode = "CustomerName";
                            break;
                        }
                    case 3:
                        {
                            StrCode = "Custom2";
                            break;
                        }
                    case 4:
                        {
                            StrCode = "Custom1";
                            break;
                        }
                    case 5:
                        {
                            StrCode = "Custom3";
                            break;
                        }
                    case 6:
                        {
                            StrCode = "Phone1";
                            break;
                        }
                   
                }

                if (cmbSearchby.Text.Trim() != "")
                {

                    string SerchText = txtSearch.Text.ToString().Trim();
                    String s;

                   

                    s = "SELECT * FROM tblCustomerMaster WHERE " + StrCode + " LIKE '%" + txtSearch.Text + "%' ORDER BY CutomerID";

                    dgvSearchEmployee.Rows.Clear();
                    SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvSearchEmployee.Rows.Add();
                            dgvSearchEmployee.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dgvSearchEmployee.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[26].ToString().Trim();
                            dgvSearchEmployee.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            dgvSearchEmployee.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[6].ToString().Trim();
                            dgvSearchEmployee.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                            dgvSearchEmployee.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                            dgvSearchEmployee.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[7].ToString().Trim();
                            dgvSearchEmployee.Rows[i].Cells[7].Value = dt.Rows[i].ItemArray[8].ToString().Trim();
                            string s1 = "SELECT AdmitDate FROM tblPatientAddmission WHERE CustomerID LIKE'%" + dt.Rows[i].ItemArray[0].ToString().Trim() + "%'";
                            SqlDataAdapter da1 = new SqlDataAdapter(s1, ConnectionString);
                            DataTable dt1 = new DataTable();
                            da1.Fill(dt1);
                            for (int j = 0; j < dt1.Rows.Count; j++)
                            {
                                DateTime abc = Convert.ToDateTime(dt1.Rows[j].ItemArray[0]);
                                dgvSearchEmployee.Rows[i].Cells[8].Value = abc.ToShortDateString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSearchByDate_Click(object sender, EventArgs e)
        {
            try
            {
                String s1;



                s1 = "SELECT CustomerID,AdmitDate FROM tblPatientAddmission WHERE AdmitDate='" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "' ORDER BY CustomerID";

                dgvSearchEmployee.Rows.Clear();
                SqlDataAdapter da1 = new SqlDataAdapter(s1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    for (int j = 0; j < dt1.Rows.Count; j++)
                    {
                        string s = "SELECT * FROM tblCustomerMaster WHERE CutomerID='" + dt1.Rows[j].ItemArray[0].ToString().Trim() + "' ORDER BY CutomerID";
                        SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvSearchEmployee.Rows.Add();
                        dgvSearchEmployee.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[26].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[6].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[7].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[7].Value = dt.Rows[i].ItemArray[8].ToString().Trim();
                       DateTime abc = Convert.ToDateTime(dt1.Rows[j].ItemArray[1]);
                            dgvSearchEmployee.Rows[i].Cells[8].Value = abc.ToShortDateString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSearchby.SelectedIndex == 7)
            {
                txtSearch.Enabled = false;
                btnSearchByDate.Enabled = true;
                txtSearch.Text = string.Empty;
            }
            else
            {
                txtSearch.Enabled = true;
                btnSearchByDate.Enabled = false;
                txtSearch.Text = string.Empty;

            }
        }

        private void dgvSearchEmployee_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dtpSearchDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label31_Click(object sender, EventArgs e)
        {

        }

        private void cmbSearchby_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            btnClear_Click(sender, e);
        }

        private void dgvSearchEmployee_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Search.InpationNo= dgvSearchEmployee[0, dgvSearchEmployee.CurrentRow.Index].Value.ToString().Trim();                
                this.Close();
            }
            catch { }
        }
    }
}
