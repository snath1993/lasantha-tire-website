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
    
    public partial class frmDirectSupplierReturnSearch : Form
    {
        public static string ConnectionString;
        public frmDirectSupplierReturnSearch()
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
                string s = "select  distinct(SupReturnNo) as SupReturnNo,VendorID,ReturnDate,'',WarehouseID,IsActive from tblDirectSupReturn  order by  SupReturnNo";
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

                        if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                        {
                            dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                        }
                        else
                        {
                            dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;

                        }
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private void dgvSearchIssue_CellDoubleClick(object sender,DataGridViewCellEventArgs e)
        {
            try
            {
                if ((dgvSearchIssue["CRNNo", e.RowIndex].Value.ToString().Trim()) != "")
                {
                    Search.searchIssueNoteNo = dgvSearchIssue["CRNNo", e.RowIndex].Value.ToString().Trim();
                }
               // this.Close();
            }
            catch 
            {
              
            }
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
                            StrCode = "SupReturnNo";
                            break;
                        }
                    case 1:
                        {
                            StrCode = "VendorID";
                            break;
                        }
                    case 2:
                        {
                            StrCode = "ReturnDate";
                            break;
                        }
                    case 3:                        
                        {
                            StrCode = "WarehouseID";
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
                            s = "select distinct SupReturnNo,VendorID,ReturnDate,'',WarehouseID,IsActive from tblDirectSupReturn where ReturnDate= '" + SerchText + "'  order by SupReturnNo";
                        }
                        else
                        {
                            s = "select  distinct  SupReturnNo,VendorID,ReturnDate,'',WarehouseID,IsActive from tblDirectSupReturn where " + StrCode + " like '%" + txtSearch.Text.Trim() + "%'  order by SupReturnNo";
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

                            if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                            {
                                dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            }
                            else
                            {
                                dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;

                            }
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
            if (cmbSearchby.Text != "Date")
            {
                return;
            }
            try
            {
                SeachOption();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        

   

   

    

    

    }
}