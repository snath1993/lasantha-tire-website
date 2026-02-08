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
    public partial class frmPackageList : Form
    {
        public static string ConnectionString;
        public frmPackageList()
        {
            InitializeComponent();
            setConnectionString();
        }
        private void setConnectionString()
        {
            try
            {
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }

        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            dgvSearchEmployee.Rows.Clear();
            cmbSearchby.SelectedIndex = -1;
            txtSearch.Text = string.Empty;
            txtSearch.Enabled = true;
            dtpSearchDate.Value = DateTime.Today;
            btnSearchByDate.Enabled = false;
            ScanList_Load(sender, e);
        }

        private void ScanList_Load(object sender, EventArgs e)
        {
            LoadCustomerList();
        }
        private void LoadCustomerList()
        {
            try
            {
                
                    string s = "SELECT * FROM ViewScanChannel WHERE (ItemType<>'LAB' AND ItemType<>'SCAN' AND ItemType<>'ECHO' AND ItemType<>'XRAY' AND ItemType<>'ECG' AND ItemType<>'DENTAL'AND ItemType<>'WARD')ORDER BY ReceiptNo";
                    SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvSearchEmployee.Rows.Clear();

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dgvSearchEmployee.Rows.Add();
                            dgvSearchEmployee.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dgvSearchEmployee.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            dgvSearchEmployee.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                            dgvSearchEmployee.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            //dgvSearchEmployee.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[10].ToString().Trim();
                            dgvSearchEmployee.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();

                            DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[5]);
                            dgvSearchEmployee.Rows[i].Cells[5].Value = abc.ToShortDateString();
                            if (dt.Rows[i].ItemArray[6].ToString() == "False")
                            {
                                dgvSearchEmployee.Rows[i].Cells[6].Value = "Save";
                            }
                            else
                            {
                                dgvSearchEmployee.Rows[i].Cells[6].Value = "Confirmed";
                                dgvSearchEmployee.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                            }

                            if (dt.Rows[i].ItemArray[7].ToString() == "False")
                            {
                                dgvSearchEmployee.Rows[i].Cells[7].Value = "Original";

                            }
                            else
                            {
                                dgvSearchEmployee.Rows[i].Cells[7].Value = "Void";
                                dgvSearchEmployee.Rows[i].DefaultCellStyle.BackColor = Color.Pink;
                            }


                        }
                    }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSearchby.SelectedIndex == 4)
            {
                txtSearch.Text = "";
                txtSearch.Enabled = false;
                btnSearchByDate.Enabled = true;

            }
            else
            {
                txtSearch.Text = "";
                txtSearch.Enabled = true;
                btnSearchByDate.Enabled = false;


            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SeachOption();
        }
        private void SeachOption()
        {
            try
            {
               
                    dgvSearchEmployee.Rows.Clear();

                    String StrCode = null;

                    if (cmbSearchby.Text.Trim() == "")
                    {
                        return;
                    }

                    switch (cmbSearchby.SelectedIndex)
                    {
                        case 0:
                            {
                                StrCode = "ReceiptNo";

                                break;
                            }
                        case 1:
                            {
                                StrCode = "PatientName";
                                break;
                            }
                        case 2:
                            {
                                StrCode = "Consultant";
                                break;
                            }

                        case 3:
                            {
                                StrCode = "ItemType";
                                break;
                            }


                    }

                    if (cmbSearchby.Text.Trim() != "")
                    {

                        string SerchText = txtSearch.Text.ToString().Trim();
                        String s;



                        s = "SELECT * FROM ViewScanChannel WHERE " + StrCode + " LIKE '%" + txtSearch.Text + "%' AND (ItemType<>'LAB' AND ItemType<>'SCAN' AND ItemType<>'ECHO' AND ItemType<>'XRAY' AND ItemType<>'ECG' AND ItemType<>'DENTAL'AND ItemType<>'WARD') ORDER BY ReceiptNo";

                        dgvSearchEmployee.Rows.Clear();
                        SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dgvSearchEmployee.Rows.Add();
                                dgvSearchEmployee.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dgvSearchEmployee.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                dgvSearchEmployee.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                                dgvSearchEmployee.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                                //dgvSearchEmployee.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[10].ToString().Trim();
                                dgvSearchEmployee.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();

                                DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[5]);
                                dgvSearchEmployee.Rows[i].Cells[5].Value = abc.ToShortDateString();

                                if (dt.Rows[i].ItemArray[6].ToString() == "False")
                                {
                                    dgvSearchEmployee.Rows[i].Cells[6].Value = "Save";
                                }
                                else
                                {
                                    dgvSearchEmployee.Rows[i].Cells[6].Value = "Confirmed";
                                    dgvSearchEmployee.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                                }

                                if (dt.Rows[i].ItemArray[7].ToString() == "False")
                                {
                                    dgvSearchEmployee.Rows[i].Cells[7].Value = "Original";

                                }
                                else
                                {
                                    dgvSearchEmployee.Rows[i].Cells[7].Value = "Void";
                                    dgvSearchEmployee.Rows[i].DefaultCellStyle.BackColor = Color.Pink;
                                }

                            }
                        }
                    }
               
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSearchByDate_Click(object sender, EventArgs e)
        {
           
                String s;



                s = "SELECT * FROM ViewScanChannel WHERE Date ='" + dtpSearchDate.Value.ToString("MM/dd/yyyy") + "'AND (ItemType<>'LAB' AND ItemType<>'SCAN' AND ItemType<>'ECHO' AND ItemType<>'XRAY' AND ItemType<>'ECG' AND ItemType<>'DENTAL'AND ItemType<>'WARD') ORDER BY ReceiptNo";

                dgvSearchEmployee.Rows.Clear();
                SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvSearchEmployee.Rows.Add();
                        dgvSearchEmployee.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        //dgvSearchEmployee.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[10].ToString().Trim();
                        dgvSearchEmployee.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString().Trim();

                        DateTime abc = Convert.ToDateTime(dt.Rows[i].ItemArray[5]);
                        dgvSearchEmployee.Rows[i].Cells[5].Value = abc.ToShortDateString();

                        if (dt.Rows[i].ItemArray[6].ToString() == "False")
                        {
                            dgvSearchEmployee.Rows[i].Cells[6].Value = "Save";
                        }
                        else
                        {
                            dgvSearchEmployee.Rows[i].Cells[6].Value = "Confirmed";
                            dgvSearchEmployee.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                        }

                        if (dt.Rows[i].ItemArray[7].ToString() == "False")
                        {
                            dgvSearchEmployee.Rows[i].Cells[7].Value = "Original";

                        }
                        else
                        {
                            dgvSearchEmployee.Rows[i].Cells[7].Value = "Void";
                            dgvSearchEmployee.Rows[i].DefaultCellStyle.BackColor = Color.Pink;
                        }

                    }
                
            }
            
        }

        private void cmbSearchby_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            btnClear_Click(sender, e);
        }

        private void dgvSearchEmployee_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Search.ScanList = dgvSearchEmployee[0, dgvSearchEmployee.CurrentRow.Index].Value.ToString().Trim();
            Search.ScanDate = dgvSearchEmployee[5, dgvSearchEmployee.CurrentRow.Index].Value.ToString().Trim();
            Search.tokenNo = dgvSearchEmployee[1, dgvSearchEmployee.CurrentRow.Index].Value.ToString().Trim();

            this.Close();
        }
    }
}
