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
    public partial class frmViewUsers : Form
    {
        public static string ConnectionString;
        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
        }
        public frmViewUsers()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void frmViewUsers_Load(object sender, EventArgs e)
        {          
            String S = "select * from Login";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);       

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    dgvUsers.Rows.Add();
                    dgvUsers.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();                    
                    dgvUsers.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[2].ToString().Trim();                  
                    
                }
                catch { }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult reply = MessageBox.Show("Do you want Delete this Record", "Are You Sure", MessageBoxButtons.YesNo, MessageBoxIcon.Question);                        
            if (reply == DialogResult.Yes)
            {               
                String S = "delete  from Login where UserID = '"+ dgvUsers.Rows[dgvUsers.CurrentRow.Index].Cells[0].Value.ToString().Trim() +"'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);

                String S1 = "delete  from UserAuthentication where UserName = '" + dgvUsers.Rows[dgvUsers.CurrentRow.Index].Cells[0].Value.ToString().Trim() + "'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                MessageBox.Show(" Record Deleted ", "Successfull", MessageBoxButtons.OK);
                dgvUsers_Load();//refresh               
            }
            else if (reply == DialogResult.No)
            {
                btnDelete.Focus();
            }
        }
        private void dgvUsers_Load()
        {
            //Load the data from database to grid
            setConnectionString();
            String S = "Select * from Login";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);

            dgvUsers.DataSource = dt.Tables[0];
            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                dgvUsers.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();               
                dgvUsers.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();
            }
        }
    }
}