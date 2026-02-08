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
    public partial class frmViewActivities : Form
    {
        public static string ConnectionString;
        public void setConnectionString()
        {
            //TextReader tr = new StreamReader("Connection.txt");
            //ConnectionString = tr.ReadLine();
            //tr.Close();
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
        }
        public frmViewActivities()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void frmViewActivities_Load(object sender, EventArgs e)
        {            
            String S = "select * from Activities";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    dgvActivities.Rows.Add();
                    dgvActivities.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString().Trim();
                   //dgvActivities.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString().Trim();
                    //following code segment change to get real name for authentication
                    dgvActivities.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                    //dgvActivities.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString().Trim();
                }
                catch { }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult reply = MessageBox.Show("Do you want Delete this Record", "Are You Sure", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (reply == DialogResult.Yes)
            {               
                String S = "delete  from Activities where ActNo = '" + dgvActivities.Rows[dgvActivities.CurrentRow.Index].Cells[0].Value.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                MessageBox.Show("Record Deleted", "Successfull", MessageBoxButtons.OK);
                dgvActivities_Load();//refresh
            }
            else if (reply == DialogResult.No)
            {
                btnDelete.Focus();
            }
        }
        private void dgvActivities_Load()
        {
            //Load the data from database to grid
            setConnectionString();
            String S = "Select * from Activities";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);

            dgvActivities.DataSource = dt.Tables[0];

            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                dgvActivities.Rows[i].Cells[0].Value = dt.Tables[0].Rows[i].ItemArray[0].ToString();
                dgvActivities.Rows[i].Cells[1].Value = dt.Tables[0].Rows[i].ItemArray[1].ToString();
                //dgvActivities.Rows[i].Cells[2].Value = dt.Tables[0].Rows[i].ItemArray[2].ToString();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}