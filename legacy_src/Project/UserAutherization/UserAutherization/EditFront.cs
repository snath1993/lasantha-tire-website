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
    public partial class EditFront : Form
    {
        public static string ConnectionString;
        public EditFront()
        {
            InitializeComponent();
            setConnectionString();
        }
        public void setConnectionString()
        {
            //TextReader tr = new StreamReader("Connection.txt");
            //ConnectionString = tr.ReadLine();
            //tr.Close();
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
        }
        private void EditFront_Load(object sender, EventArgs e)
        {
            String S1 = "select * from tblFrontEdit";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            DataSet dt1 = new DataSet();
            da1.Fill(dt1);

            dgvAuthentication.DataSource = dt1.Tables[0];
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            dgvAuthentication.EndEdit();
            dgvAuthentication.CommitEdit(DataGridViewDataErrorContexts.Commit);
            String S9 = " delete  from tblFrontEdit";
            SqlCommand cmd9 = new SqlCommand(S9);
            SqlDataAdapter da9 = new SqlDataAdapter(S9, ConnectionString);
            DataTable dt9 = new DataTable();
            da9.Fill(dt9);

            for (int x = 0; x < (dgvAuthentication.Rows.Count - 1); x++)
            {
                String S0 = "Insert into tblFrontEdit  Values ('"+dgvAuthentication[0, x].Value.ToString().Trim() + "','" + dgvAuthentication[1, x].Value.ToString().Trim() + "','" + dgvAuthentication[2, x].Value.ToString().Trim() + "','" + dgvAuthentication[3, x].Value.ToString().Trim() + "')";
                SqlCommand cmd0 = new SqlCommand(S0);
                SqlDataAdapter da0 = new SqlDataAdapter(S0, ConnectionString);
                DataTable dt0 = new DataTable();
                da0.Fill(dt0);
            }

            MessageBox.Show("Successfully Saved", "Successfull", MessageBoxButtons.OK);
        }
    }
}
