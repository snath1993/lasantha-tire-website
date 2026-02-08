using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using DataAccess;


namespace UserAutherization
{
    public partial class frmWarehouseSearch : Form
    {
        public static string ConnectionString;
        public int error = 0;
        public frmWarehouseSearch()
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
        private void frmWarehouseSearch_Load(object sender, EventArgs e)
        {
           // dgvSearch.Enabled = false;
            dgvSearch.ReadOnly = true;
            String S = "Select WhseId,WhseName,Address1 from tblWhseMaster";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);
            dgvSearch.Rows.Clear();

            //filling
            if (dt.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    dgvSearch.Rows.Add();
                    dgvSearch.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
                    dgvSearch.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
                    dgvSearch.Rows[i].Cells[2].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();
                    //dgvSearch.Rows[i].Cells[3].Value = dt.Tables[0].Rows[i].ItemArray[3].ToString();
                }
            }
            cmbSearchBy.SelectedIndex = 0;
            txtSearchBy.Focus();
        }

        //private void txtSearch_TextChanged(object sender, EventArgs e)
        //{
        //    if (txtSearchBy.Text != "")
        //        {
        //            // code
        //            try
        //            {
        //                if (cmbSearchBy.Text.ToString().Trim() == "Warehouse Id")
        //                {
        //                    string add = txtSearchBy.Text;
        //                    if (add != "")
        //                    {
        //                        String S = "Select WhseId,WhseName,Address1 from tblWhseMaster where WhseId LIKE  '" + add + "%'";

        //                        SqlCommand cmd = new SqlCommand(S);
        //                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);

        //                        DataSet dt = new DataSet();
        //                        da.Fill(dt);
        //                        dgvSearch.Rows.Clear();
                                
                               
        //                        if (dt.Tables[0].Rows.Count > 0)
        //                        {
        //                            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //                            {
        //                                dgvSearch.Rows.Add();
        //                                dgvSearch.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
        //                                dgvSearch.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
        //                                dgvSearch.Rows[i].Cells[2].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();
                                      
        //                            }
        //                        }
        //                    }

        //                }
        //            }
        //            catch { }
        //           // name
        //            try
        //            {
        //                if (cmbSearchBy.Text.ToString().Trim() == "Warehouse Name")
        //                {
        //                    string add = txtSearchBy.Text;
        //                    if (add != "")
        //                    {
        //                        String S = "Select WhseName from tblWhseMaster where WhseName LIKE  '" + add + "%'";
        //                        SqlCommand cmd = new SqlCommand(S);
        //                        SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //                        DataSet dt = new DataSet();
        //                        da.Fill(dt);
        //                        dgvSearch.Rows.Clear();
                               
        //                        if (dt.Tables[0].Rows.Count > 0)
        //                        {
        //                            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //                            {
        //                                dgvSearch.Rows.Add();
        //                                dgvSearch.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
        //                                dgvSearch.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
        //                                dgvSearch.Rows[i].Cells[2].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();
        //                                //dgvSearch.Rows[i].Cells[3].Value = dt.Tables[0].Rows[i].ItemArray[3].ToString();
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            catch { }
        //    }
        //        else
        //        {
        //            String S = "Select WhseId,WhseName,Address1 from tblWhseMaster";
        //            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
        //            DataSet dt = new DataSet();
        //            da.Fill(dt);
        //            dgvSearch.Rows.Clear();

                                       
        //            if (dt.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
        //                {
        //                    dgvSearch.Rows.Add();
        //                    dgvSearch.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
        //                    dgvSearch.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
        //                    dgvSearch.Rows[i].Cells[2].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();
        //                   // dgvSearch.Rows[i].Cells[3].Value = dt.Tables[0].Rows[i].ItemArray[3].ToString();
        //                }
        //            }
        //        }

        //    }

        private void cmbSearchBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearchBy.Focus();
        }

        private void dgvSearch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            error = 1;
            try
            {
                Search.WhseId = dgvSearch["WuseId", e.RowIndex].Value.ToString().Trim();
                Search.WhseName = dgvSearch["WuseName", e.RowIndex].Value.ToString().Trim();
                Search.Address1 = dgvSearch["Address", e.RowIndex].Value.ToString().Trim();
               
                this.Close();
            }
            catch { }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSearchBy_TextChanged(object sender, EventArgs e)
        {

            if (txtSearchBy.Text != "")
            {
                // code
                try
                {
                    if (cmbSearchBy.Text.ToString().Trim() == "Warehouse Id")
                    {
                        string add = txtSearchBy.Text;
                        if (add != "")
                        {
                            String S = "Select WhseId,WhseName,Address1 from tblWhseMaster where WhseId LIKE  '" + add + "%'";

                            SqlCommand cmd = new SqlCommand(S);
                            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);

                            DataSet dt = new DataSet();
                            da.Fill(dt);
                            dgvSearch.Rows.Clear();


                            if (dt.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                                {
                                    dgvSearch.Rows.Add();
                                    dgvSearch.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
                                    dgvSearch.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
                                    dgvSearch.Rows[i].Cells[2].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();

                                }
                            }
                        }

                    }
                }
                catch { }
                // name
                try
                {
                    if (cmbSearchBy.Text.ToString().Trim() == "Warehouse Name")
                    {
                        string add = txtSearchBy.Text;
                        if (add != "")
                        {
                            String S = "Select WhseId,WhseName,Address1 from tblWhseMaster where WhseName LIKE  '" + add + "%'";
                            SqlCommand cmd = new SqlCommand(S);
                            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                            DataSet dt = new DataSet();
                            da.Fill(dt);
                            dgvSearch.Rows.Clear();

                            if (dt.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                                {
                                    dgvSearch.Rows.Add();
                                    dgvSearch.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
                                    dgvSearch.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
                                    dgvSearch.Rows[i].Cells[2].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();
                                    //dgvSearch.Rows[i].Cells[3].Value = dt.Tables[0].Rows[i].ItemArray[3].ToString();
                                }
                            }
                        }
                    }
                }
                catch { }
            }
            else
            {
                String S = "Select WhseId,WhseName,Address1 from tblWhseMaster";
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);
                dgvSearch.Rows.Clear();


                if (dt.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                    {
                        dgvSearch.Rows.Add();
                        dgvSearch.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
                        dgvSearch.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
                        dgvSearch.Rows[i].Cells[2].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();
                        // dgvSearch.Rows[i].Cells[3].Value = dt.Tables[0].Rows[i].ItemArray[3].ToString();
                    }
                }
            }
        }

    }
}