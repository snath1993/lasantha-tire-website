using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmVendorMasterList : Form
    {
        private string ConnectionString;
        public frmVendorMasterList()
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void frmVendorMasterList_Load(object sender, EventArgs e)
        {
            LoadCusData();
        }

        private void LoadCusData()
        {
            try
            {
                dgvInvoiceList.Rows.Clear();

                string ConnString = ConnectionString;
                String S1 = "Select VendorID,VendorName,IsActive from tblVendorMaster  order by VendorID DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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

                        if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            dgvInvoiceList.Rows[i].Cells[2].Value = "Save";
                        }
                        else
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                            dgvInvoiceList.Rows[i].Cells[2].Value = "Process";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

            try
            {
                dgvInvoiceList.Rows.Clear();
                string status = "";


                if (cmbSearchby.Text == "Id")
                {
                    status = "VendorID";
                }
                else if (cmbSearchby.Text == "Name")
                {
                    status = "VendorName";
                }
                string ConnString = ConnectionString;
                String S1 = "Select VendorID,VendorName,IsActive from tblVendorMaster where " + status + " like '" + txtSearch.Text.ToString().Trim() + "%' order by VendorID DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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

                        if (Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString().Trim()) == true)
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            dgvInvoiceList.Rows[i].Cells[2].Value = "Save";
                        }
                        else
                        {
                            dgvInvoiceList.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                            dgvInvoiceList.Rows[i].Cells[2].Value = "Process";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            LoadCusData();
        }

        private void dgvInvoiceList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int getDGV_index = this.dgvInvoiceList.CurrentRow.Index;

            if (dgvInvoiceList[0, getDGV_index].ToString() != "")
            {
                string SupInvoiceNo = dgvInvoiceList[0, getDGV_index].Value.ToString();
                try
                {

                    Search.VendorSearch = SupInvoiceNo;

                    // this.Close();
                }
                catch
                {

                }
                this.Close();
            }
        }
    }
}
