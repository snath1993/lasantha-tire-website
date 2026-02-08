using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using DBUtil;

namespace UserAutherization
{
    public partial class frmCusReturnList : Form
    {
        private string CusInvoiceNo;
        public static string ConnectionString;
        int Find = 0;


        public frmCusReturnList()
        {
            InitializeComponent();
            setConnectionString();
        }

        public frmCusReturnList(int _Find)
        {
            InitializeComponent();
            setConnectionString();
            Find = _Find;
        }

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }

        private void frmSupReturnList_Load(object sender, EventArgs e)
        {
            //DataTable dt = DBUtil.clsUtils.loadItems("Select distinct SupReturnNo, ReturnDate, Location from tblSupplierReturn group by SupReturnNo, ReturnDate, Location");
            //if ((dt != null) && (dt.Rows.Count > 0))
            //{
            //    dgvInvoiceList.DataSource = dt;
            //}
            try
            {
                Load_CusRetList();
            }
            catch { }
        }

        private void Load_CusRetList()
        {
            try
            {

                dgvInvoiceList.Rows.Clear();
                string ConnString = ConnectionString;
                String S1 = "Select distinct CreditNo,CustomerID,ReturnDate,GrandTotal,InvoiceNO,LocationID from tblCutomerReturn where type<>'DirectReurn' group by CreditNo,CustomerID,ReturnDate,GrandTotal,InvoiceNO,LocationID order by  CreditNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvInvoiceList.Rows.Add();
                        dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                        dgvInvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                        dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                    }
                }
            }
            catch { }
        }

        public string RtnSupReturnNo()
        {
            return CusInvoiceNo;
        }

        private void dgvInvoiceList_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    int getDGV_index = this.dgvInvoiceList.CurrentRow.Index;
            //    if (dgvInvoiceList[0, getDGV_index].ToString() != "")
            //    {
            //        SupInvoiceNo = dgvInvoiceList[0, getDGV_index].Value.ToString();
            //        this.Close();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());

            //}
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dgvInvoiceList.Rows.Clear();
                if (cmbSearchby.Text.Trim() == "CreditNo")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_CusRetList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct CreditNo,CustomerID,ReturnDate,GrandTotal,InvoiceNO,LocationID from tblCutomerReturn where CreditNo like '%" + txtSearch.Text.Trim() + "%' and Type !='DirectReurn'  group by CreditNo,CustomerID,ReturnDate,GrandTotal,InvoiceNO,LocationID order by  CreditNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt = new DataTable();
                        da1.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvInvoiceList.Rows.Add();
                                dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                                dgvInvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                                dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                            }
                        }
                    }
                }
                if (cmbSearchby.Text.Trim() == "CustomerID")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_CusRetList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct CreditNo,CustomerID,ReturnDate,GrandTotal,InvoiceNO,LocationID from tblCutomerReturn where CustomerID like '%" + txtSearch.Text.Trim() + "%' and Type !='DirectReurn' group by CreditNo,CustomerID,ReturnDate,GrandTotal,InvoiceNO,LocationID order by  CreditNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt = new DataTable();
                        da1.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvInvoiceList.Rows.Add();
                                dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                                dgvInvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                                dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                            }
                        }
                    }
                }
                if (cmbSearchby.Text.Trim() == "LocationID")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_CusRetList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct CreditNo,CustomerID,ReturnDate,GrandTotal,InvoiceNO,LocationID from tblCutomerReturn where LocationID like '%" + txtSearch.Text.Trim() + "%' and Type !='DirectReurn' group by CreditNo,CustomerID,ReturnDate,GrandTotal,InvoiceNO,LocationID order by  CreditNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt = new DataTable();
                        da1.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvInvoiceList.Rows.Add();
                                dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                                dgvInvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                                dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                            }
                        }
                    }
                }
                if (cmbSearchby.Text.Trim() == "InvoiceNO")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_CusRetList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct CreditNo,CustomerID,ReturnDate,GrandTotal,InvoiceNO,LocationID from tblCutomerReturn where InvoiceNO like '%" + txtSearch.Text.Trim() + "%' and Type !='DirectReurn'  group by CreditNo,CustomerID,ReturnDate,GrandTotal,InvoiceNO,LocationID order by  CreditNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt = new DataTable();
                        da1.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvInvoiceList.Rows.Add();
                                dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                                dgvInvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                                dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                            }
                        }
                    }
                }
                if (cmbSearchby.Text.Trim() == "GrandTotal")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_CusRetList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct CreditNo,CustomerID,ReturnDate,GrandTotal,InvoiceNO,LocationID from tblCutomerReturn where GrandTotal like '%" + txtSearch.Text.Trim() + "%' and Type !='DirectReurn'  group by CreditNo,CustomerID,ReturnDate,GrandTotal,InvoiceNO,LocationID order by  CreditNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt = new DataTable();
                        da1.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvInvoiceList.Rows.Add();
                                dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                                dgvInvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                                dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvInvoiceList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int getDGV_index = this.dgvInvoiceList.CurrentRow.Index;
                if (dgvInvoiceList[0, getDGV_index].Value!=null && dgvInvoiceList[0, getDGV_index].Value.ToString() != "")
                {
                    Search.searchIssueNoteNo = dgvInvoiceList[0, getDGV_index].Value.ToString();
                    //this.Close();
                }
                //frmMain.ObjCusRetern.TopMost = false;
                //frmMain.objfrmCusReturnList.ShowDialog();

                if (Find == 1)
                {
                    this.Close();
                    //frmMain.ObjCusRetern.Close();
                    //frmMain.ObjCusRetern = new frmCustomerReturns(CusInvoiceNo);
                    //frmMain.ObjCusRetern.Show();
                    //frmMain.ObjCusRetern.TopMost = true;
                }
                else
                {
                    this.Close();
                    if (frmMain.ObjCusRetern == null || frmMain.ObjCusRetern.IsDisposed)
                    {
                        frmMain.ObjCusRetern = new frmCustomerReturns(Search.searchIssueNoteNo);
                    }
                    //else
                    //    frmMain.

                    frmMain.objfrmCusReturnList.TopMost = false;
                    frmMain.ObjCusRetern.Show();
                    frmMain.ObjCusRetern.TopMost = true;
                    frmMain.ObjCusRetern.WindowState = FormWindowState.Normal;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void dtpSearchDate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbSearchby.Text == "ReturnDate")
                {
                    dgvInvoiceList.Rows.Clear();
                    string SerchText = dtpSearchDate.Text.ToString().Trim();                   
                    string ConnString = ConnectionString;
                    String S1 = "Select distinct CreditNo,CustomerID,ReturnDate,GrandTotal,InvoiceNO,LocationID from tblCutomerReturn where ReturnDate = '" + SerchText.Trim() + "' and Type !='DirectReurn' group by CreditNo,CustomerID,ReturnDate,GrandTotal,InvoiceNO,LocationID order by  CreditNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            dgvInvoiceList.Rows.Add();
                            dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                            dgvInvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                            dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                            dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                        }
                    }
                    //else
                    //{
                    //    dgvInvoiceList.Rows.Clear();
                    //}
                }
            }
            catch { }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dgvInvoiceList.Rows.Clear();
                Load_CusRetList();
                txtSearch.Text = "";
            }
            catch { }
        }
    }
}