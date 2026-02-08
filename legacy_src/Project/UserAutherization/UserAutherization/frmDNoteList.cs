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
    public partial class frmDNoteList : Form
    {
        public static string ConnectionString;
        ClassDriiDown a = new ClassDriiDown();
        clsCommon objclsCommon = new clsCommon();
        int Find = 0;
        //bool IsFind = false;

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
        public frmDNoteList()
        {
            InitializeComponent();
            setConnectionString();
        }

        public frmDNoteList(int _Find)
        {
            InitializeComponent();
            setConnectionString();
            Find = _Find;
        }

        private void frmDeliveryNoteList_Load(object sender, EventArgs e)
        {
            try
            {
                Load_DeliveryNoteList();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void Load_DeliveryNoteList()
        {
            try
            {
                string ConnString = ConnectionString;
                String S1 = "Select distinct(DeliveryNoteNo),CustomerID,DispatchDate,TotalAmount,SONos,WareHouseID from tblDispatchOrder"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        dgvDeliveryNoteList.Rows.Add();

                        dgvDeliveryNoteList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvDeliveryNoteList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                        dgvDeliveryNoteList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                        // dgvDeliveryNoteList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvDeliveryNoteList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvDeliveryNoteList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        dgvDeliveryNoteList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dgvDeliveryNoteList.Rows.Clear();                               
                if (cmbSearchby.Text == "Delivey Note No")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    string ConnString = ConnectionString;
                    String S1 = "Select distinct(DeliveryNoteNo),CustomerID,DispatchDate,TotalAmount,SONos,WareHouseID from tblDispatchOrder where DeliveryNoteNo like '%" + SerchText + "%'"; 
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvDeliveryNoteList.Rows.Add();
                            dgvDeliveryNoteList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dgvDeliveryNoteList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                            dgvDeliveryNoteList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                            dgvDeliveryNoteList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            dgvDeliveryNoteList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                            dgvDeliveryNoteList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                        }
                    }
                }
                if (cmbSearchby.Text == "Date")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    string ConnString = ConnectionString;
                    String S1 = "Select distinct(DeliveryNoteNo),CustomerID,DispatchDate,TotalAmount,SONos,WareHouseID from tblDispatchOrder where DispatchDate ='" + dtpSearchDate.Value.ToShortDateString() + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvDeliveryNoteList.Rows.Add();
                            dgvDeliveryNoteList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dgvDeliveryNoteList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                            dgvDeliveryNoteList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                            dgvDeliveryNoteList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            dgvDeliveryNoteList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                            dgvDeliveryNoteList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                        }
                    }
                }
                 if (cmbSearchby.Text == "Customer ID")
                 {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    string ConnString = ConnectionString;
                    String S1 = "Select distinct(DeliveryNoteNo),CustomerID,DispatchDate,TotalAmount,SONos,WareHouseID from tblDispatchOrder where CustomerID like '%" + SerchText + "%'"; 
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da1.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvDeliveryNoteList.Rows.Add();
                            dgvDeliveryNoteList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dgvDeliveryNoteList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                            dgvDeliveryNoteList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                            dgvDeliveryNoteList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            dgvDeliveryNoteList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                            dgvDeliveryNoteList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                        }
                    }                 
                 }
                 if (cmbSearchby.Text == "Sales Order Number")
                 {
                     string SerchText = txtSearch.Text.ToString().Trim();
                     string ConnString = ConnectionString;
                     String S1 = "Select distinct(DeliveryNoteNo),CustomerID,DispatchDate,TotalAmount,SONos,WareHouseID from tblDispatchOrder where SONos LIKE '%" + SerchText + "%'";
                     SqlCommand cmd1 = new SqlCommand(S1);
                     SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                     DataTable dt = new DataTable();
                     da1.Fill(dt);

                     if (dt.Rows.Count > 0)
                     {
                         for (int i = 0; i < dt.Rows.Count; i++)
                         {
                             dgvDeliveryNoteList.Rows.Add();

                             dgvDeliveryNoteList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                             dgvDeliveryNoteList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                             DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                             dgvDeliveryNoteList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                             dgvDeliveryNoteList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                             dgvDeliveryNoteList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                             dgvDeliveryNoteList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                         }
                     }
                 }
                 if (cmbSearchby.Text == "Warehouse")
                 {
                     string SerchText = txtSearch.Text.ToString().Trim();
                     string ConnString = ConnectionString;
                     String S1 = "Select distinct(DeliveryNoteNo),CustomerID,DispatchDate,TotalAmount,SONos,WareHouseID from tblDispatchOrder where WareHouseID like '%" + SerchText + "%'";
                     SqlCommand cmd1 = new SqlCommand(S1);
                     SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                     DataTable dt = new DataTable();
                     da1.Fill(dt);

                     if (dt.Rows.Count > 0)
                     {
                         for (int i = 0; i < dt.Rows.Count; i++)
                         {
                             dgvDeliveryNoteList.Rows.Add();

                             dgvDeliveryNoteList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                             dgvDeliveryNoteList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                             DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[2]);
                             dgvDeliveryNoteList.Rows[i].Cells[2].Value = abc.ToShortDateString();
                             dgvDeliveryNoteList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                             dgvDeliveryNoteList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                             dgvDeliveryNoteList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                         }
                     }
                 }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        //frmDeliveryNote inv = new frmDeliveryNote();
        private void dgvDeliveryNoteList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string DeliveryNoteNo = "";

                try
                {
                    DeliveryNoteNo = dgvDeliveryNoteList[0, dgvDeliveryNoteList.CurrentRow.Index].Value.ToString().Trim();
                }
                catch { }
                ClassDriiDown.Delivery_note_No = a.GetNext2(DeliveryNoteNo);
                
                //this.Close();

                if (Find == 1)
                {
                    this.Close();
                    frmMain.ObjDeliveryNote.Close();
                    frmMain.ObjDeliveryNote = new frmDeliveryNote(ClassDriiDown.Delivery_note_No);
                    frmMain.ObjDeliveryNote.Show();
                    frmMain.ObjDeliveryNote.flag = true;
                }
                else
                {
                    this.Close();
                    if (frmMain.ObjDeliveryNote == null || frmMain.ObjDeliveryNote.IsDisposed)
                    {
                        frmMain.ObjDeliveryNote = new frmDeliveryNote(ClassDriiDown.Delivery_note_No);
                    }
                    frmMain.ObjDeliveryNote.Show();
                    frmMain.ObjDeliveryNote.TopMost = true;
                    frmMain.ObjDeliveryNote.WindowState = FormWindowState.Normal;
                    frmMain.ObjDNoteList.TopMost = false;
                    frmMain.ObjDeliveryNote.flag = true;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dgvDeliveryNoteList.Rows.Clear();
                Load_DeliveryNoteList();
                txtSearch.Text = "";
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dtpSearchDate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtSearch_TextChanged(sender, e);
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
    }
}