using Infragistics.Win;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace UserAutherization
{
    public partial class frmDiscountUpdate : Form
    {

        public static string ConnectionString;
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
        public frmDiscountUpdate()
        {
            InitializeComponent();
            setConnectionString();
        }
        private void SetItemBrand()
        {
            string StrSql = "select  Description from tbl_ItemCustom3";
            SqlCommand cmd = new SqlCommand(StrSql);
            SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                cmbBrand.DataSource = dt;
                cmbBrand.ValueMember = "Description";
                cmbBrand.DisplayMember = "Description";

                cmbBrand.DisplayLayout.Bands[0].Columns[0].Width = 200;
            }

        }
        private void frmDiscountUpdate_Load(object sender, EventArgs e)
        {
            SetItemBrand();
            txtDiscount2.Text = "0";
            txtDiscount3.Text = "0";
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            DialogResult reply = MessageBox.Show("Are you sure, you want to Update this record ? ", "Information", MessageBoxButtons.OKCancel);

            if (reply == DialogResult.Cancel)
            {
                return;

            }
            else
            {

                if (cmbBrand.Text.ToString().Trim() != string.Empty)
                {

                    String S = "UPDATE [tblItemMaster] SET [Discount2] = '" + txtDiscount2.Text.ToString().Trim() + "' ,[Discount3] ='" + txtDiscount3.Text.ToString().Trim() + "'  WHERE Custom3 = '" + cmbBrand.Text.ToString().Trim() + "'";
                    // String S = "insert into Login (UserID,PassWord,UserLevel) values ('" + txtUserName.Text.ToString().Trim() + "','" + txtPassWord.Text.ToString().Trim() + "','" + cmbUserLevel.Text.ToString().Trim() + "')";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    MessageBox.Show("Discount Update is Successfull");
                }
                else
                {

                    MessageBox.Show("Please Select a Brand Name");
                    return;
                }

            }




            //String S = "insert into Login (UserID,PassWord,UserLevel) values ('" + txtUserName.Text.ToString().Trim() + "','" + txtPassWord.Text.ToString().Trim() + "','" + cmbUserLevel.Text.ToString().Trim() + "')";
            //SqlCommand cmd = new SqlCommand(S);
            //SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            //DataTable dt = new DataTable();
            //da.Fill(dt);
            //MessageBox.Show("User Details Successfully Added", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
