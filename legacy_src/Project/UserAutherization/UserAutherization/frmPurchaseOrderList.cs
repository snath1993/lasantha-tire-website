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

    public partial class frmPurchaseOrderList : Form
    {
        public static string ConnectionString;
        public frmPurchaseOrderList()
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
                //   String S1 = "Select distinct SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location from tblSupplierReturn group by SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                //    string s = "select  distinct(PONumber) as PONumber,VendorID,Date,'',WHID from tblPurchaseOrder  order by  PONumber";

                string s = "SELECT distinct [PONumber],[VendorID],[Date],[WHID],[GrandTotal],IsActive FROM [dbo].[tblPurchaseOrder] order BY [PONumber] DESC";
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
                        dgvSearchIssue.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString();


                        if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                        {
                            dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            dgvSearchIssue.Rows[i].Cells[5].Value = "Save";

                        }
                        else
                        {
                            dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                            dgvSearchIssue.Rows[i].Cells[5].Value = "Process";

                        }

                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private void dgvSearchIssue_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((dgvSearchIssue["CRNNo", dgvSearchIssue.CurrentRow.Index].Value.ToString().Trim()) != "")
                {
                    Search.searchPONo = dgvSearchIssue["CRNNo", dgvSearchIssue.CurrentRow.Index].Value.ToString().Trim();
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
                            StrCode = "PONumber";
                            break;
                        }
                    case 1:
                        {
                            StrCode = "VendorID";
                            break;
                        }
                    case 2:
                        {
                            StrCode = "Date";
                            break;
                        }
                    case 3:
                        {
                            StrCode = "WHID";
                            break;
                        }

                }

                if (cmbSearchby.Text.Trim() != "")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    String s = null;

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
                        //SerchText = dtpSearchDate.Value.ToShortDateString();
                        s = "select  distinct(PONumber) as PONumber,VendorID,Date,WHID,GrandTotal,IsActive from tblPurchaseOrder where " + StrCode + " = '" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "' order by PONumber DESC";

                    }

                    if (s == null && StrCode!="Date" && StrCode != "WHID")
                    {
                        s = "select  distinct(PONumber) as PONumber,VendorID,Date,WHID,GrandTotal,IsActive from tblPurchaseOrder where " + StrCode + " like '%" + SerchText + "%'  order by PONumber DESC";
                    }
                    else if(StrCode == "WHID")
                    {
                        s = "select  distinct(PONumber) as PONumber,VendorID,Date,WHID,GrandTotal,IsActive from tblPurchaseOrder where " + StrCode + " like '" + SerchText + "%' order by PONumber DESC";

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
                            dgvSearchIssue.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();//ponumber
                            dgvSearchIssue.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();//vendorID
                            DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                            dgvSearchIssue.Rows[i].Cells[2].Value = abc.ToShortDateString();//Date
                            dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();//WH
                            dgvSearchIssue.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();//GrandTotal
                            if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                            {
                                dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                dgvSearchIssue.Rows[i].Cells[5].Value ="Save";
                            }
                            else
                            {
                                dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                dgvSearchIssue.Rows[i].Cells[5].Value = "Process";
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
                cmbSearchby.SelectedIndex = 0;
                dtpSearchDate.Value = System.DateTime.Now;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtSearch.Focus();

            if (cmbSearchby.Text.Trim() == "Save" || cmbSearchby.Text.Trim() == "Process")
            {
                try
                {
                    dgvSearchIssue.Rows.Clear();

                    bool status = false;
                 if(cmbSearchby.Text.Trim()=="Save")
                    {
                        status = true;
                    }

                 else
                    {
                        status = false;
                    }


                      string  s = "select  distinct(PONumber) as PONumber,VendorID,Date,WHID,GrandTotal,IsActive from tblPurchaseOrder where IsActive = '" + status + "'  order by PONumber DESC";

                        SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvSearchIssue.Rows.Clear();
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvSearchIssue.Rows.Add();
                            dgvSearchIssue.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();//ponumber
                            dgvSearchIssue.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();//vendorID
                            DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                            dgvSearchIssue.Rows[i].Cells[2].Value = abc.ToShortDateString();//Date
                            dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();//WH
                            dgvSearchIssue.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();//GrandTotal
                            if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                            {
                                dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                dgvSearchIssue.Rows[i].Cells[5].Value = "Save";
                            }
                            else
                            {
                                dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                dgvSearchIssue.Rows[i].Cells[5].Value = "Process";
                            }

                        }
                    }

                }
                catch (Exception ex) { throw ex; }

            }
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

        private void dgvSearchIssue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                dgvSearchIssue_CellDoubleClick(null,null);
            }
        }

        private void frmPurchaseOrderList_KeyDown(object sender, KeyEventArgs e)
        {
            
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Q)
                {
                this.Close();

                }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Down)
            {
                this.WindowState = FormWindowState.Minimized;
            }

        }

        private void cmbSearchby_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        private void dgvSearchIssue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}