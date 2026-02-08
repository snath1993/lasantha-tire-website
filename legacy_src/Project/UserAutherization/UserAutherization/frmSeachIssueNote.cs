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

    public partial class frmSeachIssueNote : Form
    {
        public static string ConnectionString;
        public frmSeachIssueNote()
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

        private void frmSeachTrans_Load(object sender, EventArgs e)
        {
            try
            {
                Load_IssueList();
            }
            catch { }
        }

        private void Load_IssueList()
        {
            try
            {

                string s = "select  IssueNoteNo,FrmWhseId,AccountID,IssueDate,DocumentType from tblIssueNoteIC order by  IssueNoteNo DESC";
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
                        dgvSearchIssue.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                    }
                }
            }
            catch { }
        }

        private void dgvSearchIssue_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                if ((dgvSearchIssue["InvoiceNo", e.RowIndex].Value.ToString().Trim()) != "")
                {
                    Search.searchIssueNoteNo = dgvSearchIssue["InvoiceNo", e.RowIndex].Value.ToString().Trim();
                }
            }
            catch { }
            this.Close();
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SeachOption()
        {
            try
            {
                string HeaderName = "";
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
                            StrCode = "Issue Note No";
                            HeaderName = "IssueNoteNo";//IssueDate
                            break;
                        }
                    case 1:
                        {
                            StrCode = "Warehouse";
                            HeaderName = "FrmWhseId";//IssueDate
                            break;
                        }
                    case 3:
                        {
                            StrCode = "DocumentType";
                            HeaderName = "IssueNoteNo";//IssueDate
                            break;
                        }
                    case 2:
                        {
                            StrCode = "Date";
                            HeaderName = "IssueDate";
                            break;
                        }

                }

                if (cmbSearchby.Text.Trim() != "")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    String s;

                    string ConnString = ConnectionString;

                    if (StrCode != "Date")
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

                    if (StrCode == "Date")
                    {
                        s = "select  IssueNoteNo,FrmWhseId,AccountID,IssueDate,DocumentType from tblIssueNoteIC where IssueDate= '" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "' order by  IssueNoteNo DESC";
                    }
                    else if(StrCode == "Issue Note No")
                    {
                        s = "select  IssueNoteNo,FrmWhseId,AccountID,IssueDate,DocumentType from tblIssueNoteIC where " + HeaderName + " like '%" + txtSearch.Text.Trim() + "'  order by IssueNoteNo DESC";
                    }


                    else
                    {
                        s = "select  IssueNoteNo,FrmWhseId,AccountID,IssueDate,DocumentType from tblIssueNoteIC where " + HeaderName + " like '" + txtSearch.Text.Trim() + "%'  order by IssueNoteNo DESC";
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
                            dgvSearchIssue.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        }
                    }

                }

            }
            catch { }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SeachOption();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dgvSearchIssue.Rows.Clear();
                Load_IssueList();
                txtSearch.Text = "";
            }
            catch { }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtSearch.Focus();
            if (cmbSearchby.Text == "Date")
                SeachOption();
        }

        private void dtpSearchDate_ValueChanged(object sender, EventArgs e)
        {
            SeachOption();
        }

        private void dgvSearchIssue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}