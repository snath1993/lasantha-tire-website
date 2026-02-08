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
    public partial class frmWhseTranList : Form
    {
        private string TransferNo;
        public static string ConnectionString;
        public frmWhseTranList()
        {
            InitializeComponent();
            setConnectionString();
        }

        public void setConnectionString()
        {
            try
            {
                TextReader tr = new StreamReader("Connection.txt");
                ConnectionString = tr.ReadLine();
                tr.Close();
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
                Load_TransferList();
                cmbSearchby.SelectedIndex = 0;
                //btnn
            }
            catch { }
        }

        private void Load_TransferList()
        {
            try
            {

                string ConnString = ConnectionString;
                String S1 = "Select distinct WhseTransId,FrmWhseId,ToWhseId,TDate from tblWhseTransfer group by WhseTransId,FrmWhseId,ToWhseId,TDate order by WhseTransId DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da1.Fill(dt);
                dgvInvoiceList.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvInvoiceList.Rows.Add();
                        dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        dgvInvoiceList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[3]);
                        dgvInvoiceList.Rows[i].Cells[3].Value = abc.ToShortDateString();
                    }
                }
            }
            catch { }
        }

        public string RtnSupReturnNo()
        {
            return TransferNo;
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
                if (cmbSearchby.Text.Trim() == "Transfer ID")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_TransferList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct WhseTransId,FrmWhseId,ToWhseId,TDate from tblWhseTransfer where WhseTransId like '" + txtSearch.Text.Trim() + "%' group by WhseTransId,FrmWhseId,ToWhseId,TDate order by WhseTransId DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt = new DataTable();
                        da1.Fill(dt);
                        dgvInvoiceList.Rows.Clear();
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvInvoiceList.Rows.Add();
                                dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[3]);
                                dgvInvoiceList.Rows[i].Cells[3].Value = abc.ToShortDateString();
                            }
                        }
                    }
                }
                if (cmbSearchby.Text.Trim() == "From Warehouse")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_TransferList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct WhseTransId,FrmWhseId,ToWhseId,TDate from tblWhseTransfer where FrmWhseId like '" + txtSearch.Text.Trim() + "%' group by WhseTransId,FrmWhseId,ToWhseId,TDate order by WhseTransId DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt = new DataTable();
                        da1.Fill(dt);
                        dgvInvoiceList.Rows.Clear();
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvInvoiceList.Rows.Add();
                                dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[3]);
                                dgvInvoiceList.Rows[i].Cells[3].Value = abc.ToShortDateString();
                            }
                        }
                    }
                }
                if (cmbSearchby.Text.Trim() == "To Warehouse")
                {
                    string SerchText = txtSearch.Text.ToString().Trim();
                    if (SerchText == "")
                    {
                        Load_TransferList();
                    }
                    else
                    {
                        string ConnString = ConnectionString;
                        String S1 = "Select distinct WhseTransId,FrmWhseId,ToWhseId,TDate from tblWhseTransfer where ToWhseId like '" + txtSearch.Text.Trim() + "%' group by WhseTransId,FrmWhseId,ToWhseId,TDate order by WhseTransId DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt = new DataTable();
                        da1.Fill(dt);
                        dgvInvoiceList.Rows.Clear();
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvInvoiceList.Rows.Add();
                                dgvInvoiceList.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                dgvInvoiceList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[3]);
                                dgvInvoiceList.Rows[i].Cells[3].Value = abc.ToShortDateString();
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
                if (dgvInvoiceList[0, getDGV_index].ToString() != "")
                {
                    TransferNo = dgvInvoiceList[0, getDGV_index].Value.ToString();
                    this.Close();
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
                if (cmbSearchby.Text == "Date")
                {
                    dgvInvoiceList.Rows.Clear();
                    string SerchText = dtpSearchDate.Text.ToString().Trim();                   
                    string ConnString = ConnectionString;
                    String S1 = "Select distinct WhseTransId,FrmWhseId,ToWhseId,TDate from tblWhseTransfer where TDate = '" + SerchText + "' group by WhseTransId,FrmWhseId,ToWhseId,TDate order by WhseTransId DESC";// // where CustomerID='" + cmbCustomer.Text.ToString().Trim() + "' and IsInvoce='" + Isinvoce + "'";
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
                            dgvInvoiceList.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                            DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[3]);
                            dgvInvoiceList.Rows[i].Cells[3].Value = abc.ToShortDateString();
                        }
                    }
                }
            }
            catch { }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                dgvInvoiceList.Rows.Clear();
                Load_TransferList();
                cmbSearchby.SelectedIndex = 0;
                txtSearch.Text = "";
            }
            catch { }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtSearch.Focus();
        }
    }
}