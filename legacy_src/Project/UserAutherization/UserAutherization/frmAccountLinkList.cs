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
    public partial class frmAccountLinkList : Form
    {
        public static string ConnectionString;
        public frmAccountLinkList()
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
        private void frmAccountLinkList_Load(object sender, EventArgs e)
        {
            Search.Referenceno = "";
            Search.AccountLinkNo = "";
            LoadData();
        }

        private void LoadData()
        {
           try
            {
                string s = "SELECT AccLinDeNo,ReferanceNo,IncomeType,Amount,Date,SaveDate FROM tblAccountLinkDetail  ORDER BY SaveDate DESC";
                SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvSearchAccountList.Rows.Clear();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvSearchAccountList.Rows.Add();
                        dgvSearchAccountList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvSearchAccountList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        dgvSearchAccountList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvSearchAccountList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                       // dgvSearchAccountList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                         DateTime Date1 = Convert.ToDateTime(dt.Rows[i].ItemArray[4]);
                        dgvSearchAccountList.Rows[i].Cells[4].Value = Date1.ToShortDateString();
                        DateTime SaveDate = Convert.ToDateTime(dt.Rows[i].ItemArray[5]);
                        dgvSearchAccountList.Rows[i].Cells[5].Value = SaveDate.ToShortDateString();
                    }
                }

                    }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSearchby.SelectedIndex == 3|| cmbSearchby.SelectedIndex == 4)
            {
                txtSearch.Text = "";
                txtSearch.Enabled = false;
                btnSearchByDate.Enabled = true;

            }
            else
            {
                txtSearch.Text = "";
                txtSearch.Enabled = true;
                btnSearchByDate.Enabled = false;


            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dgvSearchAccountList.Rows.Clear();
            cmbSearchby.SelectedIndex = -1;
            txtSearch.Text = string.Empty;
            txtSearch.Enabled = true;
            dtpSearchDate.Value = DateTime.Today;
            btnSearchByDate.Enabled = false;
            frmAccountLinkList_Load(sender, e);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SeachOption();
        }
        private void SeachOption()
        {
            try
            {


                dgvSearchAccountList.Rows.Clear();

                String StrCode = null;

                if (cmbSearchby.Text.Trim() == "")
                {
                    return;
                }

                switch (cmbSearchby.SelectedIndex)
                {
                    case 0:
                        {
                            StrCode = "AccLinDeNo";

                            break;
                        }
                    case 1:
                        {
                            StrCode = "ReferanceNo";
                            break;
                        }
                    case 2:
                        {
                            StrCode = "IncomeType";
                            break;
                        }

                   


                }

                if (cmbSearchby.Text.Trim() != "")
                {

                    string SerchText = txtSearch.Text.ToString().Trim();
                    String s;


                     s = "SELECT AccLinDeNo,ReferanceNo,IncomeType,Amount,Date,SaveDate FROM tblAccountLinkDetail WHERE " + StrCode + " LIKE '%" + txtSearch.Text + "%'  ORDER BY SaveDate DESC";
                    SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvSearchAccountList.Rows.Clear();

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvSearchAccountList.Rows.Add();
                            dgvSearchAccountList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dgvSearchAccountList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            dgvSearchAccountList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                            dgvSearchAccountList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            // dgvSearchAccountList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                            DateTime Date1 = Convert.ToDateTime(dt.Rows[i].ItemArray[4]);
                            dgvSearchAccountList.Rows[i].Cells[4].Value = Date1.ToShortDateString();
                            DateTime SaveDate = Convert.ToDateTime(dt.Rows[i].ItemArray[5]);
                            dgvSearchAccountList.Rows[i].Cells[5].Value = SaveDate.ToShortDateString();
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
            SerachDate();
        }

        private void SerachDate()
        {
            try
            {


                dgvSearchAccountList.Rows.Clear();

                String StrCode = null;

                if (cmbSearchby.Text.Trim() == "")
                {
                    return;
                }

                switch (cmbSearchby.SelectedIndex)
                {
                    case 3:
                        {
                            StrCode = "Date";

                            break;
                        }
                    case 4:
                        {
                            StrCode = "SaveDate";
                            break;
                        }
                   


                }

                if (cmbSearchby.Text.Trim() != "")
                {

                    string SerchText = txtSearch.Text.ToString().Trim();
                    String s;


                    s = "SELECT AccLinDeNo,ReferanceNo,IncomeType,Amount,Date,SaveDate FROM tblAccountLinkDetail WHERE " + StrCode + "='"+ dtpSearchDate.Value.ToString("MM/dd/yyyy") + "'   ORDER BY SaveDate DESC";
                    SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvSearchAccountList.Rows.Clear();

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvSearchAccountList.Rows.Add();
                            dgvSearchAccountList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dgvSearchAccountList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            dgvSearchAccountList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                            dgvSearchAccountList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            // dgvSearchAccountList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                            DateTime Date1 = Convert.ToDateTime(dt.Rows[i].ItemArray[4]);
                            dgvSearchAccountList.Rows[i].Cells[4].Value = Date1.ToShortDateString();
                            DateTime SaveDate = Convert.ToDateTime(dt.Rows[i].ItemArray[5]);
                            dgvSearchAccountList.Rows[i].Cells[5].Value = SaveDate.ToShortDateString();
                        }
                    }


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void dgvSearchAccountList_DoubleClick(object sender, EventArgs e)
        {
            Search.AccountLinkNo = dgvSearchAccountList[0, dgvSearchAccountList.CurrentRow.Index].Value.ToString().Trim();
            Search.Referenceno = dgvSearchAccountList[1, dgvSearchAccountList.CurrentRow.Index].Value.ToString().Trim();
            this.Close();
        }
    }
}
