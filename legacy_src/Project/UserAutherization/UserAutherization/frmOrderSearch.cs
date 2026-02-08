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
    
    public partial class frmOrderSearch : Form
    {
        public static string ConnectionString;
        public frmOrderSearch()
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
                LoadSalesOrderData();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void LoadSalesOrderData()
        {
            try
            {
                string s = "SELECT DISTINCT SalesOrderNo, CustomerName, Date,TotalAmount,WarehouseID FROM View_salesOrder_customer order by SalesOrderNo";
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

        private void dgvSearchIssue_CellDoubleClick(object sender,DataGridViewCellEventArgs e)
        {
            try
            {
                if ((dgvSearchIssue["CRNNo", e.RowIndex].Value.ToString().Trim()) != "")
                {
                    Search.searchIssueNoteNo = dgvSearchIssue["CRNNo", e.RowIndex].Value.ToString().Trim();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

                if (cmbSearchby.Text.Trim() == "")
                {
                    return;
                }
                switch (cmbSearchby.SelectedIndex)
                {
                    case 0:
                        {
                            StrCode = "SalesOrderNo";
                            break;
                        }
                    case 1:
                        {
                            StrCode = "CustomerName";
                            break;
                        }
                    case 2:
                        {
                            StrCode = "Date";
                            break;
                        }
                    case 3:                        
                        {

                            StrCode = "WarehouseID";
                            break;
                        }
                    case 4:                        
                        {

                            StrCode = "TotalAmount";
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
                                LoadSalesOrderData();
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
                            s = "SELECT DISTINCT SalesOrderNo, CustomerName, Date,TotalAmount,WarehouseID FROM View_salesOrder_customer  where Date= '" + SerchText + "'   order by SalesOrderNo";
//                            s = "select distinct CreditNo,CustomerID,ReturnDate,'',LocationID from tblCutomerReturn where ReturnDate= '" + SerchText + "' and Type='DirectReurn'   order by CreditNo";
                        }
                        else
                        {
                            s = "SELECT DISTINCT SalesOrderNo, CustomerName, Date,TotalAmount,WarehouseID FROM View_salesOrder_customer  where " + StrCode + " like '%" + txtSearch.Text.Trim() + "%'   order by SalesOrderNo";
                            //s = "select  distinct  CreditNo,CustomerID,ReturnDate,'',LocationID from tblCutomerReturn where " + StrCode + " like '%" + txtSearch.Text.Trim() + "%'  and Type='DirectReurn' order by CreditNo";
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
                LoadSalesOrderData();
                txtSearch.Text = "";
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
                SeachOption();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvSearchIssue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        

   

   

    

    

    }
}