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
    
    public partial class frmFinalInvoiceSearch : Form
    {
        public static string ConnectionString;
        int Find = 0;

        

        public frmFinalInvoiceSearch(int _Find)
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
                Load_BHTList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Load_BHTList()
        {
            try
            {
                string s = "SELECT DISTINCT[InvoiceNo],[CustomerID],[Name],[Room],[InvoiceDate],[IsConfirm],[ContactNo],[GrandTotal],[isFullpay],[Type],[InvType]" +
                         " ,[IsDischarge],[RefDoctor],[NIC],[Age],[AdmitDate],[AdmitTime]FROM[ViewFinalBillList]";
                SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvSearchIssue.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvSearchIssue.Rows.Add();
                        dgvSearchIssue.Rows[i].Cells[0].Value = dt.Rows[i]["InvoiceNo"].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[1].Value = dt.Rows[i]["CustomerID"].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[2].Value = dt.Rows[i]["Name"].ToString().Trim();
                        DateTime abc = Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString());
                        dgvSearchIssue.Rows[i].Cells[3].Value = abc.ToShortDateString();

                        dgvSearchIssue.Rows[i].Cells[4].Value = dt.Rows[i]["ContactNo"].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[5].Value = dt.Rows[i]["NIC"].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[6].Value = dt.Rows[i]["Age"].ToString().Trim();
                        dgvSearchIssue.Rows[i].Cells[7].Value = dt.Rows[i]["GrandTotal"].ToString().Trim();
                        if (dt.Rows[i]["IsConfirm"].ToString().Trim() == "False")
                        {
                            dgvSearchIssue.Rows[i].Cells[8].Value = "Available";
                            dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                        }
                        else
                        {
                            dgvSearchIssue.Rows[i].Cells[8].Value = "Discharge";
                            dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = Color.Pink;
                        }

                        //DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[4]);
                        //dgvSearchIssue.Rows[i].Cells[4].Value = abc.ToShortDateString();
                        //// dgvSearchIssue.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        //if (dt.Rows[i].ItemArray[5].ToString().Trim() == "True") {
                        //    dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                        //}
                        //else { }

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
                    MessageBox.Show("Please Select a Search Option");
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
                            StrCode = "Name";
                            break;
                        }
                    case 3:
                        {
                            StrCode = "ContactNo";
                            break;
                        }
                    case 4:
                        {
                            StrCode = "NIC";
                            break;
                        }
                    case 5:
                        {
                            StrCode = "Age";
                            break;
                        }
                    case 6:
                        {
                            StrCode = "GrandTotal";
                            break;
                        }
                    case 7:
                        {
                            StrCode = "IsDischarge";
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
                            Load_BHTList();
                            return;

                        }

                        SerchText = txtSearch.Text.ToString().Trim();

                    }
                    else
                    {
                        SerchText = dtpSearchDate.Value.ToString("yyyy/MM/dd");
                    }

                    if (StrCode == "InvoiceDate")
                    {
                        s = "select  distinct([InvoiceNo]) as [InvoiceNo],[CustomerIDd],[Name],[Room],[InvoiceDate],[IsConfirm]from ViewFinalBillList where  " + StrCode + " = '" + SerchText + "' order by  InvoiceNo";
                    }
                    else
                    {

                         s = "SELECT DISTINCT[InvoiceNo],[CustomerID],[Name],[Room],[InvoiceDate],[IsConfirm],[ContactNo],[GrandTotal],[isFullpay],[Type],[InvType]" +
                         " ,[IsDischarge],[RefDoctor],[NIC],[Age],[AdmitDate],[AdmitTime]FROM[ViewFinalBillList] where " + StrCode + " like '%" + txtSearch.Text.Trim() + "%'   order by  InvoiceNo";

                       // s = "select distinct([InvoiceNo]) as [InvoiceNo],[CustomerID],[Name],[Room],[InvoiceDate],[IsConfirm]from ViewFinalBillList where " + StrCode + " like '%" + txtSearch.Text.Trim() + "%'   order by  InvoiceNo";
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
                            dgvSearchIssue.Rows[i].Cells[0].Value = dt.Rows[i]["InvoiceNo"].ToString().Trim();
                            dgvSearchIssue.Rows[i].Cells[1].Value = dt.Rows[i]["CustomerID"].ToString().Trim();
                            dgvSearchIssue.Rows[i].Cells[2].Value = dt.Rows[i]["Name"].ToString().Trim();
                            DateTime abc = Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString());
                            dgvSearchIssue.Rows[i].Cells[3].Value = abc.ToShortDateString();

                            dgvSearchIssue.Rows[i].Cells[4].Value = dt.Rows[i]["ContactNo"].ToString().Trim();
                            dgvSearchIssue.Rows[i].Cells[5].Value = dt.Rows[i]["NIC"].ToString().Trim();
                            dgvSearchIssue.Rows[i].Cells[6].Value = dt.Rows[i]["Age"].ToString().Trim();
                            dgvSearchIssue.Rows[i].Cells[7].Value = dt.Rows[i]["GrandTotal"].ToString().Trim();
                            if (dt.Rows[i]["IsConfirm"].ToString().Trim() == "False")
                            {
                                dgvSearchIssue.Rows[i].Cells[8].Value = "Available";
                                dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                            }
                            else
                            {
                                dgvSearchIssue.Rows[i].Cells[8].Value = "Discharge";
                                dgvSearchIssue.Rows[i].DefaultCellStyle.BackColor = Color.Pink;
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
                Load_BHTList();
                txtSearch.Text = "";
                cmbSearchby.SelectedIndex = -1;
                txtSearch.Enabled = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Text = "";
           // Load_IssueList();
            if (cmbSearchby.SelectedIndex == 2)
            {
                dtpSearchDate.Focus();
                txtSearch.Enabled = false;
            }
            else
            {
                txtSearch.Focus();
                txtSearch.Enabled = true;
            }
            
        }

        private void dtpSearchDate_ValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //   SeachOption();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void dgvSearchIssue_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvSearchIssue["InvoiceNo", e.RowIndex].Value!=null && (dgvSearchIssue["InvoiceNo", e.RowIndex].Value.ToString().Trim()) != "")
                {
                    Search.searchFinalInvoiceNo = dgvSearchIssue["InvoiceNo", e.RowIndex].Value.ToString().Trim();
                    //Search.FinalInvoiceIsConfirm=
                }
                if (Find == 1)
                {
                    this.Close();
                    //frmMain.ObjDirectR.Close();
                    //frmMain.ObjDirectR = new frmReturnNotenew(Search.searchIssueNoteNo);
                    //frmMain.ObjDirectR.Show();
                }
                //else
                //{                   
                //    this.Close();
                //    if (frmMain.ObjDirectR == null || frmMain.ObjDirectR.IsDisposed)
                //    {
                //        frmMain.ObjDirectR = new frmReturnNotenew(Search.searchFinalInvoiceNo);
                //    }
                //    //frmMain.objfrmFinalInvoiceSearch.TopMost = false;
                //    frmMain.ObjDirectR.Show();
                //    frmMain.ObjDirectR.TopMost = true;
                //    //frmMain.ObjDirectR.WindowState = FormWindowState.Normal;                    
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

       

        

   

   

    

    

    }
}