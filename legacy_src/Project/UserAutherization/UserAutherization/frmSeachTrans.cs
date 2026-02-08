using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using DataAccess;

namespace UserAutherization
{
    public partial class frmSeachTrans : Form
    {
        clsCommon objclsCommon = new clsCommon();
        public static string ConnectionString;
        public frmSeachTrans()
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmSeachTrans_Load(object sender, EventArgs e)
        {
            try
            {
                Load_IssueList();
                cmbSearchby.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void Load_IssueList()
        {
            try
            {
                string s = "select  WhseTransId,FrmWhseId,ToWhseId,TDate from tblWhseTransfer order by  WhseTransId";
                SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvSearchIssue.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvSearchIssue.Rows.Add();
                        dgvSearchIssue.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[3]);
                        dgvSearchIssue.Rows[i].Cells[3].Value = abc.ToShortDateString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void dgvSearchIssue_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((dgvSearchIssue["InvoiceNo", e.RowIndex].Value.ToString().Trim()) != "")
                {
                    Search.searchIssueNoteNo = dgvSearchIssue["InvoiceNo", e.RowIndex].Value.ToString().Trim();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SeachOption()
        {
            try
            {
                dgvSearchIssue.Rows.Clear();

                String StrCode = null;
                String s;

                if (cmbSearchby.Text.Trim() == "")
                {
                    s = "select  WhseTransId,FrmWhseId,ToWhseId,TDate from tblWhseTransfer order by  WhseTransId";
                    SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvSearchIssue.Rows.Clear();
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvSearchIssue.Rows.Add();
                            dgvSearchIssue.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dgvSearchIssue.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            dgvSearchIssue.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                            DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[3]);
                            dgvSearchIssue.Rows[i].Cells[3].Value = abc.ToShortDateString();
                        }
                    }
                }

                switch (cmbSearchby.SelectedIndex)
                {
                    case 0:
                        {
                            StrCode = "WhseTransId";
                            break;
                        }
                    case 1:
                        {
                            StrCode = "FrmWhseId";
                            break;
                        }
                    case 2:
                        {
                            StrCode = "ToWhseId";
                            break;
                        }
                    case 3:
                        {
                            StrCode = "TDate";
                            break;
                        }
                }

                if (cmbSearchby.Text.Trim() != "")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();                   
                    string ConnString = ConnectionString;

                    if (StrCode != "TDate")
                    {
                        if (SerchText == "")
                        {
                            Load_IssueList();
                            return;
                        }
                        SerchText = txtSearch.Text.ToString().Trim();
                    }
                    else
                    {
                        SerchText = dtpSearchDate.Text.Trim();
                    }
                    if (StrCode == "TDate")
                    {
                        s = "select  WhseTransId,FrmWhseId,ToWhseId,TDate from tblWhseTransfer where TDate= '" + SerchText + "' order by  WhseTransId";
                    }
                    else
                    {
                        s = "select  WhseTransId,FrmWhseId,ToWhseId,TDate from tblWhseTransfer where " + StrCode + " like '%" + txtSearch.Text.Trim() + "%'  order by WhseTransId";
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
                            dgvSearchIssue.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dgvSearchIssue.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            dgvSearchIssue.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                            DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[3]);
                            dgvSearchIssue.Rows[i].Cells[3].Value = abc.ToShortDateString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SeachOption();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dgvSearchIssue.Rows.Clear();
                Load_IssueList();
                cmbSearchby.SelectedIndex = 0;
                txtSearch.Text = "";
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtSearch.Focus();
        }

        private void dtpSearchDate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                SeachOption();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvSearchIssue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvSearchIssue_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((dgvSearchIssue["InvoiceNo", e.RowIndex].Value.ToString().Trim()) != "")
                {
                    Search.searchIssueNoteNo = dgvSearchIssue["InvoiceNo", e.RowIndex].Value.ToString().Trim();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }











    }
}