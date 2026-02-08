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

namespace WarehouseTransfer
{
    public partial class frmFGTransferList : Form
    {
        public static string ConnectionString;
        public frmFGTransferList()
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

        private void frmIssueNoteSearch_Load(object sender, EventArgs e)
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
                string s = "select distinct FGTransNo,TransferDate,WarehouseID,Job from tblFGTransfer order by FGTransNo";
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
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[1]);
                        dgvSearchIssue.Rows[i].Cells[1].Value = abc.ToShortDateString();
                        dgvSearchIssue.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                    }
                }
            }
            catch { }
        }

        private void dgvSearchIssue_CellDoubleClick(object sender,DataGridViewCellEventArgs e)
        {
            Search.searchIssueNoteNo = dgvSearchIssue["IssueNoteID", e.RowIndex].Value.ToString().Trim();
            this.Close();
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {

//                Return Note No
//Date
//Warehouse
//Job
//                dgvSearchIssue.Rows.Clear();
                if (cmbSearchby.Text.Trim() == "FG TRans No")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_IssueList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                       // string s = "select distinct FGTransNo,TransferDate,WarehouseID,Job from tblFGTransfer order by FGTransNo";
                        String s = "select distinct FGTransNo,TransferDate,WarehouseID,Job from tblFGTransfer where FGTransNo like '" + txtSearch.Text.Trim() + "%' group by FGTransNo,TransferDate,WarehouseID,Job";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[1]);
                                dgvSearchIssue.Rows[i].Cells[1].Value = abc.ToShortDateString();
                                dgvSearchIssue.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                                dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            }
                        }
                    }
                }
                if (cmbSearchby.Text.Trim() == "Job")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_IssueList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String s = "select distinct FGTransNo,TransferDate,WarehouseID,Job from tblFGTransfer where Job like '" + txtSearch.Text.Trim() + "%' group by FGTransNo,TransferDate,WarehouseID,Job";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                       // String s = "select distinct ReturnNoteNo,ReturnDate,FrmWarehouseID,Job from tblReturnNote where Job like '" + txtSearch.Text.Trim() + "%' group by ReturnNoteNo,ReturnDate,FrmWarehouseID,Job";
                       // String s = "select distinct IssueNoteId,IDate,FrmWhseId from tblIssueNote where FrmWhseId like '" + txtSearch.Text.Trim() + "%' group by IssueNoteId,IDate,FrmWhseId";
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
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[1]);
                                dgvSearchIssue.Rows[i].Cells[1].Value = abc.ToShortDateString();
                                dgvSearchIssue.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                                dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            }
                        }
                    }
                }
                //=============================
                if (cmbSearchby.Text.Trim() == "Warehouse")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_IssueList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String s = "select distinct FGTransNo,TransferDate,WarehouseID,Job from tblFGTransfer where WarehouseID like '" + txtSearch.Text.Trim() + "%' group by FGTransNo,TransferDate,WarehouseID,Job";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                       // String s = "select distinct ReturnNoteNo,ReturnDate,FrmWarehouseID,Job from tblReturnNote where FrmWarehouseID like '" + txtSearch.Text.Trim() + "%' group by ReturnNoteNo,ReturnDate,FrmWarehouseID,Job";
                        // String s = "select distinct IssueNoteId,IDate,FrmWhseId from tblIssueNote where FrmWhseId like '" + txtSearch.Text.Trim() + "%' group by IssueNoteId,IDate,FrmWhseId";
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
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[1]);
                                dgvSearchIssue.Rows[i].Cells[1].Value = abc.ToShortDateString();
                                dgvSearchIssue.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                                dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            }
                        }
                    }
                }
                //==========================
            }
            catch { }
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
        }

        private void dtpSearchDate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbSearchby.Text == "Date")
                {
                    dgvSearchIssue.Rows.Clear();
                    string SerchText = dtpSearchDate.Text.ToString().Trim();
                    string ConnString = ConnectionString;
                    //String s = "select distinct ReturnNoteNo,ReturnDate,FrmWarehouseID,Job from tblReturnNote where FrmWarehouseID like '" + txtSearch.Text.Trim() + "%' group by ReturnNoteNo,ReturnDate,FrmWarehouseID,Job";
                    String S1 = "select distinct FGTransNo,TransferDate,WarehouseID,Job from tblFGTransfer where TransferDate = '" + SerchText + "%' group by FGTransNo,TransferDate,WarehouseID,Job";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                   // String S1 = "select distinct ReturnNoteNo,ReturnDate,FrmWarehouseID,Job from tblReturnNote where ReturnDate ='" + SerchText + "' group by ReturnNoteNo,ReturnDate,FrmWarehouseID,Job";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvSearchIssue.Rows.Add();
                            dgvSearchIssue.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[1]);
                            dgvSearchIssue.Rows[i].Cells[1].Value = abc.ToShortDateString();
                            dgvSearchIssue.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                            dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        }
                    }
                }
            }
            catch { }
        }

        private void frmReturnNoteList_Load(object sender, EventArgs e)
        {
            try
            {
                Load_IssueList();
            }
            catch { }
        }
    }
}