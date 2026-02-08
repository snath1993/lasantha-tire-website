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
    public partial class frmSupInvoiceList : Form
    {
        Controlers objControlers = new Controlers();
        clsCommon objclsCommon = new clsCommon();
        private string SupInvoiceNo;
        int IsFind = 0;

        public frmSupInvoiceList()
        {
            InitializeComponent();
            setConnectionString();
        }

        public frmSupInvoiceList(int _IsFind)
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ConnectionString;

        private void frmSupInvoiceList_Load(object sender, EventArgs e)
        {
            try
            {
                //DataTable dt = DBUtil.clsUtils.loadItems("Select distinct SupInvoiceNo,CustomerID,InvoiceDate, Location,NetTotal from tblSupplierInvoices group by SupInvoiceNo,CustomerID,InvoiceDate, Location,NetTotal");
                //if ((dt != null) && (dt.Rows.Count > 0))
                //{
                //    dgvInvoiceList.DataSource = dt;
                //}
                Load_SuppInvList();
                cmbSearchby.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void Load_SuppInvList()
        {
            try
            {
                dgvInvoiceList.Rows.Clear();

                string ConnString = ConnectionString;
                String S1 = "Select distinct SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive from tblSupplierInvoices group by SupInvoiceNo,CustomerID,InvoiceDate, Location,NetTotal,GRNNos,IsActive order by SupInvoiceNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                        // dgvDeliveryNoteList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();

                        if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            dgvInvoiceList.Rows[i].Cells[6].Value = "Save";
                        }
                        else
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                            dgvInvoiceList.Rows[i].Cells[6].Value = "Process";

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string RtnSupInvoiceNo()
        {
            return SupInvoiceNo;
        }



        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //===============================================================
                dgvInvoiceList.Rows.Clear();
                if (cmbSearchby.Text == "Invoice No")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_SuppInvList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive from tblSupplierInvoices where SupInvoiceNo like '%" + txtSearch.Text.Trim() + "%' group by  SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive order by SupInvoiceNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                                // dgvDeliveryNoteList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();
                                if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                                {
                                    dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                    dgvInvoiceList.Rows[i].Cells[6].Value = "Save";
                                }
                                else
                                {
                                    dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                    dgvInvoiceList.Rows[i].Cells[6].Value = "Process";

                                }

                            }
                        }
                    }
                }
                //======================================================================
                //            Invoice No
                //VendorID
                //Date
                //GRN NO
                //Location
                if (cmbSearchby.Text == "VendorID")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_SuppInvList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive from tblSupplierInvoices where CustomerID like '%" + txtSearch.Text.Trim() + "%' group by  SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive order by SupInvoiceNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                                // dgvDeliveryNoteList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();

                                if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                                {
                                    dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                    dgvInvoiceList.Rows[i].Cells[6].Value = "Save";
                                }
                                else
                                {
                                    dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                    dgvInvoiceList.Rows[i].Cells[6].Value = "Process";

                                }
                            }
                        }
                    }
                }
                //===================================================

                if (cmbSearchby.Text == "GRN NO")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_SuppInvList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive from tblSupplierInvoices where GRNNos like '%" + txtSearch.Text.Trim() + "%' group by  SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive order by SupInvoiceNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                                // dgvDeliveryNoteList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();

                                if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                                {
                                    dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                    dgvInvoiceList.Rows[i].Cells[6].Value = "Save";
                                }
                                else
                                {
                                    dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                    dgvInvoiceList.Rows[i].Cells[6].Value = "Process";

                                }
                            }
                        }
                    }
                }

                if (cmbSearchby.Text == "Location")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_SuppInvList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive from tblSupplierInvoices where Location like '%" + txtSearch.Text.Trim() + "%' group by  SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive order by SupInvoiceNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                                // dgvDeliveryNoteList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();

                                if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                                {
                                    dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                    dgvInvoiceList.Rows[i].Cells[6].Value = "Save";
                                }
                                else
                                {
                                    dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                    dgvInvoiceList.Rows[i].Cells[6].Value = "Process";

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dtpSearchDate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbSearchby.Text == "Date")
                {
                    dgvInvoiceList.Rows.Clear();
                    string SerchText = dtpSearchDate.Text.ToString().Trim();
                    //if (SerchText == "")
                    //{
                    //    Load_SuppInvList();
                    //}
                    //else
                    //{
                    string ConnString = ConnectionString;
                    String S1 = "Select SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive from tblSupplierInvoices where InvoiceDate ='" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "' group by  SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive order by SupInvoiceNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                    // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                            // dgvDeliveryNoteList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                            dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                            dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();

                            if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                            {
                                dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                dgvInvoiceList.Rows[i].Cells[6].Value = "Save";
                            }
                            else
                            {
                                dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                dgvInvoiceList.Rows[i].Cells[6].Value = "Process";

                            }
                        }
                    }
                    //  }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dgvInvoiceList.Rows.Clear();
                Load_SuppInvList();
                txtSearch.Text = "";
                cmbSearchby.SelectedIndex = 0;
                dtpSearchDate.Value = System.DateTime.Now;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSearchby.Text == "Date")
            {
                dgvInvoiceList.Rows.Clear();
                string SerchText = dtpSearchDate.Text.ToString().Trim();
                //if (SerchText == "")
                //{
                //    Load_SuppInvList();
                //}
                //else
                //{
                string ConnString = ConnectionString;
                String S1 = "Select SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive from tblSupplierInvoices where InvoiceDate ='" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "' group by  SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive order by SupInvoiceNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                        // dgvDeliveryNoteList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();

                        if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            dgvInvoiceList.Rows[i].Cells[6].Value = "Save";
                        }
                        else
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                            dgvInvoiceList.Rows[i].Cells[6].Value = "Process";

                        }
                    }
                }
                //  }
            }


            if (cmbSearchby.Text == "Save"|| cmbSearchby.Text == "Process")
            {
                dgvInvoiceList.Rows.Clear();
                string SerchText = dtpSearchDate.Text.ToString().Trim();

                bool status = false;
                if (cmbSearchby.Text == "Save")
                {
                    status = true;
                }
                else
                {
                    status = false;
                }
                string ConnString = ConnectionString;
                String S1 = "Select SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive from tblSupplierInvoices where IsActive ='" + status + "' group by  SupInvoiceNo,CustomerID,InvoiceDate,NetTotal,GRNNos,Location,IsActive order by SupInvoiceNo DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                // String S1 = "Select distinct(tblSalesInvoices),CustomerID,InvoiceDate,NetTotal,DeliveryNoteNos from tblSalesInvoices"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                        // dgvDeliveryNoteList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();

                        if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            dgvInvoiceList.Rows[i].Cells[6].Value = "Save";
                        }
                        else
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                            dgvInvoiceList.Rows[i].Cells[6].Value = "Process";

                        }
                    }
                }
                //  }
            }
        }

        private void dgvInvoiceList_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int getDGV_index = this.dgvInvoiceList.CurrentRow.Index;
                try
                {
                    if (dgvInvoiceList[0, getDGV_index].ToString() != "")
                    {
                        SupInvoiceNo = dgvInvoiceList[0, getDGV_index].Value.ToString();
                    }
                }
                catch { }

                if (IsFind == 1)
                {
                    this.Close();
                    frmMain.ObjSupInvoice.Close();
                    frmMain.ObjSupInvoice = new frmSupInvoice(SupInvoiceNo);
                    frmMain.ObjSupInvoice.Show();
                }
                else
                {
                    this.Close();
                    if (frmMain.ObjSupInvoice == null || frmMain.ObjSupInvoice.IsDisposed)
                    {
                        frmMain.ObjSupInvoice = new frmSupInvoice(SupInvoiceNo);
                    }
                    frmMain.ObjSupInvoice.Show();
                    frmMain.ObjSupInvoice.TopMost = true;                    
                    frmMain.ObjSupInvoice.WindowState = FormWindowState.Normal;
                    frmMain.ObjSupInvoicelist.TopMost = false;                    
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Invoice List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

    }
}