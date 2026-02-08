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
    public partial class frmSupReturnList : Form
    {
        clsCommon objclsCommon = new clsCommon();
        private string SupInvoiceNo;
        public static string ConnectionString;
        int IsFind = 0;

        public frmSupReturnList()
        {
            InitializeComponent();
            setConnectionString();
        }

        public frmSupReturnList(int _IsFind)
        {
            InitializeComponent();
            setConnectionString();
            IsFind = _IsFind;
        }

        public void setConnectionString()
        {
            try
            {
                //TextReader tr = new StreamReader("Connection.txt");
                //ConnectionString = tr.ReadLine();
                //tr.Close();
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                Load_SuppRetList();
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void Load_SuppRetList()
        {
            try
            {
                dgvInvoiceList.Rows.Clear();
                string ConnString = ConnectionString;
                String S1 = "Select distinct SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive from tblSupplierReturn group by SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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

        public string RtnSupReturnNo()
        {
            return SupInvoiceNo;
        }        

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dgvInvoiceList.Rows.Clear();
                if (cmbSearchby.Text.Trim() == "Return No")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_SuppRetList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive from tblSupplierReturn where SupReturnNo like '%" + txtSearch.Text.Trim() + "%' group by SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                if (cmbSearchby.Text.Trim() == "Vendor ID")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_SuppRetList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive from tblSupplierReturn where VendorID like '%" + txtSearch.Text.Trim() + "%' group by SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                if (cmbSearchby.Text.Trim() == "Return Date")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_SuppRetList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive from tblSupplierReturn where ReturnDate ='" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "' group by SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                if (cmbSearchby.Text.Trim() == "Sup Invoice No")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_SuppRetList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive from tblSupplierReturn where SupInvoiceNos like '%" + txtSearch.Text.Trim() + "%' group by SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                if (cmbSearchby.Text.Trim() == "Location")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_SuppRetList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive from tblSupplierReturn where Location like '%" + txtSearch.Text.Trim() + "%' group by SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                objclsCommon.ErrorLog("Supplier Return Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
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
                    frmMain.ObjSupplierRetern.Close();
                    frmMain.ObjSupplierRetern = new frmSupplierReturn(SupInvoiceNo);
                    frmMain.ObjSupplierRetern.Show();
                }
                else
                {
                    this.Close();
                    if (frmMain.ObjSupplierRetern == null || frmMain.ObjSupplierRetern.IsDisposed)
                    {
                        frmMain.ObjSupplierRetern = new frmSupplierReturn(SupInvoiceNo);
                    }
                    frmMain.ObjSupplierRetern.Show();
                    frmMain.ObjSupplierRetern.TopMost = true;
                    frmMain.ObjSupplierRetern.WindowState = FormWindowState.Normal;
                    frmMain.ObjSupReturnlist.TopMost = false; 
                }
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void dtpSearchDate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbSearchby.Text == "Return Date")
                {
                    dgvInvoiceList.Rows.Clear();
                    string SerchText = dtpSearchDate.Text.ToString().Trim();                   
                    string ConnString = ConnectionString;
                    String S1 = "Select SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive from tblSupplierReturn where ReturnDate ='" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "' group by  SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dgvInvoiceList.Rows.Clear();
                Load_SuppRetList();
                txtSearch.Text = "";
            }
            catch (Exception ex)
            {
                objclsCommon.ErrorLog("Supplier Return Note List", ex.Message, sender.ToString(), ex.StackTrace);
            }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch_TextChanged(sender, e);

            try
            {
                dgvInvoiceList.Rows.Clear();
                if (cmbSearchby.Text.Trim() == "Save"|| cmbSearchby.Text.Trim() == "Process")
                {
                    bool SerchText = false;
                    if (cmbSearchby.Text.Trim() == "Save")
                    {
                        SerchText = true;
                    }
                    else
                    {
                        SerchText = false;
                    }
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive from tblSupplierReturn where IsActive = '" + SerchText + "' group by SupReturnNo,VendorID,ReturnDate,NetTotal,SupInvoiceNos,Location,IsActive";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
            catch(Exception ex)
            {

            }





            }
    }
}