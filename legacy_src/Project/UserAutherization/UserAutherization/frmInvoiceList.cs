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
    public partial class frmInvoiceList : Form
    {
        public static string ConnectionString;
        ClassDriiDown a = new ClassDriiDown();

        public frmInvoiceList()
        {
            InitializeComponent();
            setConnectionString();
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
            catch { }
        }
        private void frmInvoiceList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadInvoiceList();
            }
            catch { }
        }
        private void LoadInvoiceList()
        {
            try
            {

                string ConnString = ConnectionString;
               // String S1 = "Select distinct(DeliveryNoteNo),CustomerID,DispatchDate,TotalAmount,SONos from tblDispatchOrder"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
               // String S1 = "Select distinct(InvoiceNo),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                String S1 = "Select distinct(GRN_NO),VendorID,GRNDate,NetTotal,PONos from tblInvoiceTransaction"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                        // dgvinvoiceList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvinvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvinvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                    }
                }
            }
            catch { }
        }

        //frmInvoicing invoicn = new frmInvoicing();
        frmNewInvoice NEWInv = new frmNewInvoice();

        private void dgvinvoiceList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string InvoiceNO = dgvinvoiceList[0, dgvinvoiceList.CurrentRow.Index].Value.ToString().Trim();
                ClassDriiDown.Invoice_No = a.GetNext1(InvoiceNO);
                NEWInv.flag1 = true;
                NEWInv.Show();
                this.Close();
            }
            catch { }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dgvinvoiceList.Rows.Clear();

                if (cmbSearchby.Text == "INV NO")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();

                    string ConnString = ConnectionString;
                    String S1 = "Select distinct(GRN_NO),VendorID,GRNDate,NetTotal,PONos from tblInvoiceTransaction where GRN_NO='" + SerchText + "'";
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
                        }
                    }
                }
                if (cmbSearchby.Text == "Customer ID")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();

                    string ConnString = ConnectionString;
                    String S1 = "Select distinct(GRN_NO),VendorID,GRNDate,NetTotal,PONos from tblInvoiceTransaction where VendorID='" + SerchText + "'";
                    // String S1 = "Select distinct(DeliveryNoteNo),CustomerID,DispatchDate,TotalAmount,SONos from tblDispatchOrder where CustomerID='" + SerchText + "'"; 
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dgvinvoiceList.Rows.Clear();
                LoadInvoiceList();
            }
            catch { }
        }
    }
}