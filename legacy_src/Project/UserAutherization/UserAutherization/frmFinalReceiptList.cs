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
    public partial class frmFinalReceiptList : Form
    {
        private string ConnectionString;

        public frmFinalReceiptList()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void frmFinalReceiptList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch (Exception ex) { throw ex; }

        }
        private void LoadList()
        {
            try
            {
                string s = "SELECT distinct(FinallReceptNO)as FinallReceptNO, [CustomerID],[InvoiceNo],[Name],[Date],[isVoid],GrandTotal FROM[ViewFinalReceipList]";
                SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvSearchIssue.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvSearchIssue.Rows.Add();
                        dgvSearchIssue.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[4]);
                        dgvSearchIssue.Rows[i].Cells[4].Value = abc.ToShortDateString();
                        dgvSearchIssue.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[6].ToString().Trim();
                        // dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        if (dt.Rows[i].ItemArray[5].ToString().Trim() == "True")
                        {
                            dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = Color.LightPink;
                        }
                        else { }

                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtSearch.Focus();
            frmFinalReceiptList_Load(sender, e);
            if (cmbSearchby.Text == "Date")
            {
                txtSearch.Enabled = false;
                dtpSearchDate.Enabled = true;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SeachOption();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void SeachOption()
        {
            try
            {
                dgvSearchIssue.Rows.Clear();

                String StrCode = null;

                if (cmbSearchby.Text.Trim() == "")
                {
                    return;
                }
                switch (cmbSearchby.SelectedIndex)
                {
                    case 0:
                        {
                            StrCode = "InvoiceNo";
                            break;
                        }
                    case 1:
                        {
                            StrCode = "CustomerID";
                            break;
                        }
                    case 2:
                        {
                            StrCode = "FinallReceptNO";
                            break;
                        }
                    case 3:
                        {
                            StrCode = "Date";
                            break;
                        }

                }

                if (cmbSearchby.Text.Trim() != "")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    String s;

                    string ConnString = ConnectionString;

                    if (StrCode != "InvoiceDate")
                    {
                        if (SerchText == "")
                        {
                            LoadList();
                            return;

                        }

                        SerchText = txtSearch.Text.ToString().Trim();

                    }
                    else
                    {
                        SerchText = dtpSearchDate.Text.Trim();
                    }

                    if (StrCode == "InvoiceDate")
                    {
                        s = "SELECT distinct(FinallReceptNO)as FinallReceptNO, [CustomerID],[InvoiceNo],[Name],[Date],[isVoid] FROM [ViewFinalReceipList] where  " + StrCode + " like '%" + SerchText + "' order by  InvoiceNo";
                    }
                    else
                    {
                        s = "SELECT distinct(FinallReceptNO)as FinallReceptNO, [CustomerID],[InvoiceNo],[Name],[Date],[isVoid] FROM [ViewFinalReceipList] where " + StrCode + " like '%" + txtSearch.Text.Trim() + "%'   order by  InvoiceNo";
                    }
                    SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvSearchIssue.Rows.Clear();
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvSearchIssue.Rows.Add();
                            dgvSearchIssue.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                            dgvSearchIssue.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dgvSearchIssue.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[4]);
                            dgvSearchIssue.Rows[i].Cells[4].Value = abc.ToShortDateString();
                            // dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            if (dt.Rows[i].ItemArray[5].ToString().Trim() == "True")
                            {
                                dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = Color.LightPink;
                            }

                        }
                    }

                }

            }
            catch (Exception ex) { throw ex; }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dgvSearchIssue.Rows.Clear();
                LoadList();
                txtSearch.Text = "";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void dgvSearchIssue_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvSearchIssue["CRNNo", e.RowIndex].Value != null && (dgvSearchIssue["CRNNo", e.RowIndex].Value.ToString().Trim()) != "")
                {
                    Search.searchFinalReceiptInvoiceNo = dgvSearchIssue["CRNNo", e.RowIndex].Value.ToString().Trim();
                    Search.searchFinalReceiptNo = dgvSearchIssue["IssueNoteID", e.RowIndex].Value.ToString().Trim();
                    Search.FinalReceiptCus= dgvSearchIssue["Column1", e.RowIndex].Value.ToString().Trim();
                    Search.FinalReceiptCusName = dgvSearchIssue["Column2", e.RowIndex].Value.ToString().Trim();
                    Search.FinalReINV= dgvSearchIssue["CRNNo", e.RowIndex].Value.ToString().Trim();
                    Search.FinalGeand= dgvSearchIssue["Column3", e.RowIndex].Value.ToString().Trim();
                    Search.searchFinalDate = dgvSearchIssue["date", e.RowIndex].Value.ToString().Trim();
                }

                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
