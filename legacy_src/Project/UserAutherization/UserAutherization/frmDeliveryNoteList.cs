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
    public partial class frmDeliveryNoteList : Form
    {
        public static string ConnectionString;
        ClassDriiDown a = new ClassDriiDown();
        clsCommon objclsCommon = new clsCommon();
        int IsFind = 0;
        //frmMain o = new frmMain();

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
        public frmDeliveryNoteList()
        {
            try
            {
                InitializeComponent();
                setConnectionString();
                IsFind = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public frmDeliveryNoteList(int _IsFind)
        {
            try
            {
                InitializeComponent();
                setConnectionString();
                IsFind = _IsFind;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmDeliveryNoteList_Load(object sender, EventArgs e)
        {
            try
            {
                Load_DeliveryNoteList();
                cmbSearchby.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Warehouse Transfer", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void Load_DeliveryNoteList()
        {
            try
            {
                dgvDeliveryNoteList.Rows.Clear();
                string ConnString = ConnectionString;
                String S1 = "Select distinct(GRN_NO),VendorID,GRNDate,NetTotal,PONos,WareHouseID,IsActive from tblGRNTran order by GRN_NO DESC"; // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                        dgvDeliveryNoteList.Rows[i].Cells[3].Value = Convert.ToDouble(dt.Rows[i].ItemArray[3]).ToString("N2");
                        dgvDeliveryNoteList.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();
                        dgvDeliveryNoteList.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString().Trim();

                        if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                        {
                            dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            dgvDeliveryNoteList.Rows[i].Cells[6].Value ="Save";
                        }
                        else
                        {
                            dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                            dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Process";
                        }
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

                if (cmbSearchby.Text == "GRN NO")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_DeliveryNoteList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct(GRN_NO),VendorID,GRNDate,NetTotal,PONos,WareHouseID,IsActive from tblGRNTran where GRN_NO LIKE '%" + SerchText + "%' order by GRN_NO DESC";
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

                                if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                                {
                                    dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                    dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Save";
                                }
                                else
                                {
                                    dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                    dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Process";
                                }
                            }
                        }
                    }
                }
                if (cmbSearchby.Text == "VendorID")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_DeliveryNoteList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct(GRN_NO),VendorID,GRNDate,NetTotal,PONos,WareHouseID,IsActive from tblGRNTran where VendorID like '%" + SerchText + "%' order by GRN_NO DESC";
                        // String S1 = "Select distinct(DeliveryNoteNo),CustomerID,DispatchDate,TotalAmount,SONos from tblDispatchOrder where CustomerID='" + SerchText + "'"; 
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

                                if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                                {
                                    dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                    dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Save";
                                }
                                else
                                {
                                    dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                    dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Process";
                                }
                            }
                        }
                    }
                }
                //Serch in po numbers=============

                if (cmbSearchby.Text == "PO Number")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_DeliveryNoteList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct(GRN_NO),VendorID,GRNDate,NetTotal,PONos,WareHouseID,IsActive from tblGRNTran where PONos LIKE '%" + SerchText + "%' order by GRN_NO DESC";
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

                                if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                                {
                                    dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                    dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Save";
                                }
                                else
                                {
                                    dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                    dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Process";
                                }
                            }
                        }
                    }
                }

                //search in Location=========================

                if (cmbSearchby.Text == "Location")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_DeliveryNoteList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct(GRN_NO),VendorID,GRNDate,NetTotal,PONos,WareHouseID,IsActive from tblGRNTran where WareHouseID LIKE '%" + SerchText + "%' order by GRN_NO DESC";
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

                                if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                                {
                                    dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                    dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Save";
                                }
                                else
                                {
                                    dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                    dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Process";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dgvDeliveryNoteList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                string DeliveryNoteNo="";
                try
                {

                    DeliveryNoteNo = dgvDeliveryNoteList[0, dgvDeliveryNoteList.CurrentRow.Index].Value.ToString().Trim();
                }
                catch { }
                ClassDriiDown.Delivery_note_No = a.GetNext2(DeliveryNoteNo);
                if (IsFind == 1)
                {
                    this.Close();
                    frmMain.ObjGRN.Close();
                    frmMain.ObjGRN = new frmGRN(ClassDriiDown.Delivery_note_No);
                    frmMain.ObjGRN.Show();
                }
                //frmMain.ObjGRN.frmGRN_Activated
                else
                {
                    this.Close();
                    if (frmMain.ObjGRN == null || frmMain.ObjGRN.IsDisposed)
                    {
                        frmMain.ObjGRN = new frmGRN(ClassDriiDown.Delivery_note_No);
                    }

                    frmMain.ObjGRN.Show();
                    frmMain.ObjGRN.TopMost = true;
                    frmMain.ObjGRN.WindowState = FormWindowState.Normal;
                    frmMain.ObjGRNList.TopMost = false;
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
                ClassDriiDown.Delivery_note_No = a.GetNext2("");
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dgvDeliveryNoteList.Rows.Clear();
                Load_DeliveryNoteList();
                cmbSearchby.SelectedIndex = 0;
                txtSearch.Text = "";
                dtpSearchDate.Value = System.DateTime.Now;
            }
            catch { }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbSearchby.Text == "Date")
                {
                    dgvDeliveryNoteList.Rows.Clear();
                    string SerchText = dtpSearchDate.Text.ToString().Trim();

                    string ConnString = ConnectionString;
                    String S1 = "Select distinct(GRN_NO),VendorID,GRNDate,NetTotal,PONos,WareHouseID,IsActive from tblGRNTran where GRNDate = '" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "' order by GRN_NO DESC";
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

                            if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                            {
                                dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Save";
                            }
                            else
                            {
                                dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Process";
                            }
                        }
                    }
                }
            
          


          
                // dgvDeliveryNoteList.Rows.Clear();
                if (cmbSearchby.Text == "Save"|| cmbSearchby.Text == "Process")
                {
                    dgvDeliveryNoteList.Rows.Clear();

                    bool status = false;
                    if(cmbSearchby.Text =="Save")
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }
                    string ConnString = ConnectionString;
                    String S1 = "Select distinct(GRN_NO),VendorID,GRNDate,NetTotal,PONos,WareHouseID,IsActive from tblGRNTran where IsActive = '" + status + "' order by GRN_NO DESC";
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

                            if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                            {
                                dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Save";
                            }
                            else
                            {
                                dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Process";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void frmDeliveryNoteList_FormClosed(object sender, FormClosedEventArgs e)
        {
            //inv.Show();
         
        }

        private void dtpDispatchDate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                dgvDeliveryNoteList.Rows.Clear();

                if (cmbSearchby.Text == "Date")
                {
                    string SerchText = dtpSearchDate.Text.ToString().Trim();

                    string ConnString = ConnectionString;
                    String S1 = "Select distinct(GRN_NO),VendorID,GRNDate,NetTotal,PONos,WareHouseID,IsActive from tblGRNTran where GRNDate = '" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "'";
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


                            if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                            {
                                dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Save";
                            }
                            else
                            {
                                dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Process";
                            }
                        }
                    }
                }
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
                // dgvDeliveryNoteList.Rows.Clear();
                if (cmbSearchby.Text == "Date")
                {
                    dgvDeliveryNoteList.Rows.Clear();
                    string SerchText = dtpSearchDate.Text.ToString().Trim();

                    string ConnString = ConnectionString;
                    String S1 = "Select distinct(GRN_NO),VendorID,GRNDate,NetTotal,PONos,WareHouseID,IsActive from tblGRNTran where GRNDate = '" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "' order by GRN_NO DESC";
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

                            if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                            {
                                dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Save";
                            }
                            else
                            {
                                dgvDeliveryNoteList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                                dgvDeliveryNoteList.Rows[i].Cells[6].Value = "Process";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Delivery Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }
    }
}