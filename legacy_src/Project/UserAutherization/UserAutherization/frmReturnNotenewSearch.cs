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
    
    public partial class frmReturnNotenewSearch : Form
    {
        public static string ConnectionString;
        int Find = 0;

        public frmReturnNotenewSearch()
        {
            InitializeComponent();
            setConnectionString();            
        }

        public frmReturnNotenewSearch(int _Find)
        {
            InitializeComponent();
            setConnectionString();
            Find = _Find;
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

        private void frmInvoiceSearch_Load(object sender, EventArgs e)
        {
            try
            {
                Load_IssueList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Load_IssueList()
        {
            try
            {
      //          SELECT [CustomerID]
      //,[CreditNo]
      //,[ReturnDate]
      //,[LocationID]
      //,[IsApplyToInvoice]
      //,[InvoiceNO]
      //,[ARAccount]
      //,[NoofDistribution]
      //,[DistributionNo]
      //,[ItemID]
      //,[InvoiceQty]
      //,[ReturnQty]
      //,[Description]
      //,[UOM]
      //,[UnitPrice]
      //,[Discount]
      //,[Amount]
      //,[GL_Account]
      //,[NBT]
      //,[VAT]
      //,[GrossTotal]
      //,[GrandTotal]
      //,[ISExport]
      //,[CurrenUser]
      //,[IsFullInvReturn]
      //,[JobID]
      //,[Tax1ID]
      //,[Tax2ID]
      //,[Type]
  //FROM [DistributiondbDemo].[dbo].[tblCutomerReturn]
                string s = "select  distinct(CreditNo) as CreditNo,CustomerID,ReturnDate,'',LocationID from tblCutomerReturn where Type='DirectReurn' order by  CreditNo DESC";
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
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                        dgvSearchIssue.Rows[i].Cells[2].Value = abc.ToShortDateString();
                        dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        
                    }
                }
            }
            catch (Exception ex) { throw ex; }
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

                if (cmbSearchby.Text.Trim() == "")
                {
                    return;
                }
                switch (cmbSearchby.SelectedIndex)
                {
                    case 0:
                        {
                            StrCode = "CreditNo";
                            break;
                        }
                    case 1:
                        {
                            StrCode = "CustomerID";
                            break;
                        }
                    case 2:
                        {
                            StrCode = "ReturnDate";
                            break;
                        }
                    case 3:                        
                        {
                            StrCode = "LocationID";
                            break;
                        }
                }

                if (cmbSearchby.Text.Trim() != "")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    String s;

                        string ConnString = ConnectionString;

                        if (StrCode != "ReturnDate")
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

                        if (StrCode == "ReturnDate")
                        {
                            s = "select distinct CreditNo,CustomerID,ReturnDate,'',LocationID from tblCutomerReturn where ReturnDate= '" + SerchText + "' and Type='DirectReurn'   order by CreditNo DESC";
                        }
                        else
                        {
                            s = "select  distinct  CreditNo,CustomerID,ReturnDate,'',LocationID from tblCutomerReturn where " + StrCode + " like '%" + txtSearch.Text.Trim() + "%'  and Type='DirectReurn' order by CreditNo DESC";
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
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                                dgvSearchIssue.Rows[i].Cells[2].Value = abc.ToShortDateString();
                                dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvSearchIssue.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                

                            }
                        }
                    
                }

            }
            catch (Exception ex) { throw ex; }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SeachOption();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dgvSearchIssue.Rows.Clear();
                Load_IssueList();
                txtSearch.Text = "";
                dtpSearchDate.Value = System.DateTime.Now;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
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
                if (cmbSearchby.Text != "Date")
                {
                    return;
                }
                SeachOption();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvSearchIssue_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvSearchIssue["CRNNo", e.RowIndex].Value!=null && (dgvSearchIssue["CRNNo", e.RowIndex].Value.ToString().Trim()) != "")
                {
                    Search.searchIssueNoteNo = dgvSearchIssue["CRNNo", e.RowIndex].Value.ToString().Trim();
                }
                if (Find == 1)
                {
                    this.Close();
                    //frmMain.ObjDirectR.Close();
                    //frmMain.ObjDirectR = new frmReturnNotenew(Search.searchIssueNoteNo);
                    //frmMain.ObjDirectR.Show();
                }
                else
                {                   
                    this.Close();
                    if (frmMain.ObjDirectR == null || frmMain.ObjDirectR.IsDisposed)
                    {
                        frmMain.ObjDirectR = new frmReturnNotenew(Search.searchIssueNoteNo);
                    }
                    //frmMain.objfrmReturnNotenewSearch.TopMost = false;
                    frmMain.ObjDirectR.Show();
                    frmMain.ObjDirectR.TopMost = true;
                    //frmMain.ObjDirectR.WindowState = FormWindowState.Normal;                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

       

        

   

   

    

    

    }
}