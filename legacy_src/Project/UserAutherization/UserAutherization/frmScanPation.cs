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
    public partial class frmScanPation : Form
    {
        public static string ConnectionString;
        public frmScanPation()
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

        private void frmScanPation_Load(object sender, EventArgs e)
        {
            LoadPation();
        }
            private void LoadPation()
        {
            try
            {
                if (user.Ispackage == false)
                {
                    string s = "SELECT  [CutomerID],[CustomerName],[Cus_Type],[Phone1]FROM [tblCustomerMaster] WHERE Cus_Type='" + DataAccess.Access.type + "'";
                    SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dttourlist.Rows.Clear();
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dttourlist.Rows.Add();
                            dttourlist.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dttourlist.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            dttourlist.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                            dttourlist.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                        }
                    }
                }
                else if (user.Ispackage == true)
                {
                    string s = "SELECT  [CutomerID],[CustomerName],[Cus_Type],[Phone1]FROM [tblCustomerMaster]";
                    SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dttourlist.Rows.Clear();
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dttourlist.Rows.Add();
                            dttourlist.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                            dttourlist.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                            dttourlist.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                            dttourlist.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
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
            LoadPation();
            txtSearch.Text = "";
            cmbSearchby.SelectedIndex = -1;
        }
        private void SeachOption()
        {
            try
            {
                if (user.Ispackage == false)
                {
                    dttourlist.Rows.Clear();

                    String StrCode = null;

                    if (cmbSearchby.Text.Trim() == "")
                    {
                        return;
                    }

                    switch (cmbSearchby.SelectedIndex)
                    {
                        case 0:
                            {
                                StrCode = "CutomerID";
                                break;
                            }
                        case 1:
                            {
                                StrCode = "CustomerName";
                                break;
                            }
                        case 2:
                            {
                                StrCode = "Phone1";
                                break;
                            }

                    }

                    if (cmbSearchby.Text.Trim() != "")
                    {
                        string SerchText = txtSearch.Text.ToString().Trim();
                        String s;

                        string ConnString = ConnectionString;
                        s = "SELECT  [CutomerID],[CustomerName],[Cus_Type],[Phone1]FROM [tblCustomerMaster] where  " + StrCode + " like '%" + txtSearch.Text.Trim() + "%' AND Cus_Type='" + DataAccess.Access.type + "' ";
                        dttourlist.Rows.Clear();
                        SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dttourlist.Rows.Add();

                                dttourlist.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dttourlist.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                dttourlist.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                                dttourlist.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
                            }
                        }
                    }
                }else if (user.Ispackage == true)
                {
                    dttourlist.Rows.Clear();

                    String StrCode = null;

                    if (cmbSearchby.Text.Trim() == "")
                    {
                        return;
                    }

                    switch (cmbSearchby.SelectedIndex)
                    {
                        case 0:
                            {
                                StrCode = "CutomerID";
                                break;
                            }
                        case 1:
                            {
                                StrCode = "CustomerName";
                                break;
                            }
                        case 2:
                            {
                                StrCode = "Phone1";
                                break;
                            }

                    }

                    if (cmbSearchby.Text.Trim() != "")
                    {
                        string SerchText = txtSearch.Text.ToString().Trim();
                        String s;

                        string ConnString = ConnectionString;
                        s = "SELECT  [CutomerID],[CustomerName],[Cus_Type],[Phone1]FROM [tblCustomerMaster] where  " + StrCode + " like '%" + txtSearch.Text.Trim() + "%' ";
                        dttourlist.Rows.Clear();
                        SqlDataAdapter da = new SqlDataAdapter(s, ConnectionString);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dttourlist.Rows.Add();

                                dttourlist.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                                dttourlist.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                                dttourlist.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                                dttourlist.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString().Trim();
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

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SeachOption();
        }

        private void cmbSearchby_Click(object sender, EventArgs e)
        {
            LoadPation();
            txtSearch.Text = "";
        }

        private void dttourlist_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Search.ScanPation = dttourlist[0, e.RowIndex].Value.ToString().Trim();
            this.Close();

        }
    }

}