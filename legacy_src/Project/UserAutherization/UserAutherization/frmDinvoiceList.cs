using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace UserAutherization
{
    public partial class frmDInvoiceList : Form
    {
        public static string ConnectionString;
        ClassDriiDown a = new ClassDriiDown();
        clsCommon objclsCommon = new clsCommon();
        int IsFind = 0;

        public frmDInvoiceList()
        {
            InitializeComponent();
            setConnectionString();
        }

        public frmDInvoiceList(int _IsFind)
        {
            InitializeComponent();
            setConnectionString();
            IsFind = _IsFind;
        }

        public void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
                //TextReader tr = new StreamReader("Connection.txt");
                //ConnectionString = tr.ReadLine();
                //tr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmInvoiceList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadInvoiceList();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void LoadInvoiceList()
        {
            try
            {
                dgvinvoiceList.Rows.Clear();
                string ConnString = ConnectionString;
                String S1 = "Select distinct(InvoiceNo),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos,Location from tblSalesInvoices  where IsDirect='False'"; //
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvinvoiceList.Rows.Add();
                        dgvinvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvinvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                        dgvinvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                        dgvinvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvinvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        dgvinvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       // frmInvoicing invoicn = new frmInvoicing();

        private void dgvinvoiceList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string InvoiceNO = "";
                try
                {
                    InvoiceNO = dgvinvoiceList[0, dgvinvoiceList.CurrentRow.Index].Value.ToString().Trim();
                }
                catch { }

                ClassDriiDown.Invoice_No = a.GetNext1(InvoiceNO);
                ClassDriiDown.IsInvSerch = true;
               // invoicn.flag1 = true;
                //invoicn.Show();
                if (IsFind == 1)
                {
                    this.Close();
                    frmMain.ObInvoicing.Close();
                    frmMain.ObInvoicing = new frmInvoicing(InvoiceNO);
                    frmMain.ObInvoicing.Show();
                }
                else
                {
                    this.Close();
                    if (frmMain.ObInvoicing == null || frmMain.ObInvoicing.IsDisposed)
                    {
                        frmMain.ObInvoicing = new frmInvoicing(InvoiceNO);
                    }
                    frmMain.ObInvoicing.Show();
                    frmMain.ObInvoicing.TopMost = true;
                    frmMain.ObInvoicing.WindowState = FormWindowState.Normal;
                    frmMain.ObjInvoiceList.TopMost = false;                  
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dgvinvoiceList.Rows.Clear();
                if (cmbSearchby.Text.Trim() == "Invoice No")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        LoadInvoiceList();
                    }
                    else
                    {
                        dgvinvoiceList.Rows.Clear();
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct(InvoiceNo),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos,Location from tblSalesInvoices where IsDirect='False' and InvoiceNo like '%" + txtSearch.Text.Trim() + "%' group by InvoiceNo,CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos,Location";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt = new DataTable();
                        da1.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvinvoiceList.Rows.Add();
                                dgvinvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvinvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                                dgvinvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                                dgvinvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvinvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                dgvinvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                            }
                        }
                    }
                }
                if (cmbSearchby.Text.Trim() == "Delivey Note No")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        LoadInvoiceList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct(InvoiceNo),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos,Location from tblSalesInvoices where IsDirect='False' and DeliveryNoteNos like '%" + txtSearch.Text.Trim() + "%' group by InvoiceNo,CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos,Location";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt = new DataTable();
                        da1.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvinvoiceList.Rows.Add();
                                dgvinvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvinvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                                dgvinvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                                dgvinvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvinvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                dgvinvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                            }
                        }
                    }
                }
                if (cmbSearchby.Text.Trim() == "Customer ID")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        LoadInvoiceList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct(InvoiceNo),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos,Location from tblSalesInvoices where IsDirect='False' and CustomerID like '%" + txtSearch.Text.Trim() + "%' group by InvoiceNo,CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos,Location";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt = new DataTable();
                        da1.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvinvoiceList.Rows.Add();
                                dgvinvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvinvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                                dgvinvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                                dgvinvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvinvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                dgvinvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                            }
                        }
                    }
                }
                if (cmbSearchby.Text.Trim() == "Location")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        LoadInvoiceList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct(InvoiceNo),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos,Location from tblSalesInvoices where IsDirect='False' and  Location like '%" + txtSearch.Text.Trim() + "%' group by InvoiceNo,CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos,Location";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt = new DataTable();
                        da1.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvinvoiceList.Rows.Add();
                                dgvinvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvinvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                                dgvinvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                                dgvinvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvinvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                dgvinvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                            }
                        }
                    }
                }
                if (cmbSearchby.Text == "Date")
                {
                    dgvinvoiceList.Rows.Clear();
                    string SerchText = dtpSearchDate.Text.ToString().Trim();
                    string ConnString = ConnectionString;
                    String S1 = "Select distinct(InvoiceNo),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos,Location from tblSalesInvoices where  IsDirect='False' and  InvoiceDate ='" + dtpSearchDate.Value.ToShortDateString() + "' group by InvoiceNo,CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos,Location";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvinvoiceList.Rows.Add();
                            dgvinvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dgvinvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                            dgvinvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                            dgvinvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            dgvinvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                            dgvinvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dtpSearchDate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbSearchby.Text == "Date")
                {
                    dgvinvoiceList.Rows.Clear();
                    string SerchText = dtpSearchDate.Text.ToString().Trim();
                    string ConnString = ConnectionString;
                    String S1 = "Select distinct(InvoiceNo),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos,Location from tblSalesInvoices where IsDirect='False' and  InvoiceDate ='" + dtpSearchDate.Value.ToShortDateString() + "' group by InvoiceNo,CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos,Location";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvinvoiceList.Rows.Add();
                            dgvinvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dgvinvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                            dgvinvoiceList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                            dgvinvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            dgvinvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                            dgvinvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dgvinvoiceList.Rows.Clear();
                LoadInvoiceList();
                txtSearch.Text = "";
                dtpSearchDate.Value = user.LoginDate;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Customer Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvinvoiceList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}