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
    public partial class frmItemMasterDefault : Form
    {
        private string ConnectionString;
        public frmItemMasterDefault()
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
        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            SqlTransaction Trans = con.BeginTransaction();
            try
            {
                string S21 = "UPDATE  [dbo].[tblItemMaster] SET CostMethod = '" + cmbCostMethod.Text.Trim() + "'";

                SqlCommand cmd1 = new SqlCommand(S21, con, Trans);
                cmd1.ExecuteNonQuery();

                Trans.Commit();
                MessageBox.Show("Save SucsessFully");
            }
            catch
            {
                Trans.Rollback();
                
            }

            finally
            {
                con.Close();
            }
        }

        private void frmItemMasterDefault_Load(object sender, EventArgs e)
        {
         
           
            //try
            //{
               
            //    var StrSql = "SELECT [ID],[Description] FROM tbl_ItemClass";
            //    DataTable dt = new DataTable();
            //    SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
            //    dAdapt.Fill(dt);

            //    cmbItemClass.DataSource = dt;
            //    cmbItemClass.DisplayMember = "Description";
            //    cmbItemClass.ValueMember = "ID";

               
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

        }
    }
}
