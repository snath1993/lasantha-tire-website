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
    public partial class frmActivities : Form
    {
        int ActNo;//for get new activity no
        public static string ConnectionString;
        public void setConnectionString()
        {
            //TextReader tr = new StreamReader("Connection.txt");
            //ConnectionString = tr.ReadLine();
            //tr.Close();
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
        }

        public frmActivities()
        {
            InitializeComponent();
            setConnectionString();
        }
        public void getActNo()//get new activity no
        {
            setConnectionString();
            string S = "Select ActNo from Activities  ORDER BY ActNo ";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count == 0)
            {
                String S7 = "insert into Activities (ActNo) values ('" + 1 + "')";
                SqlCommand cmd7 = new SqlCommand(S7);
                SqlDataAdapter da7 = new SqlDataAdapter(S7, ConnectionString);
                DataTable dt7 = new DataTable();
                da7.Fill(dt7);
                txtActivityNo.Text = "1";
            }
            else 
            {
                int p = ds.Tables[0].Rows.Count - 1;
                ActNo = Convert.ToInt32(ds.Tables[0].Rows[p].ItemArray[0].ToString().Trim());
                ActNo = ActNo + 1;
                txtActivityNo.Text = ActNo.ToString();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(txtActivityNo.Text == "1")
            {               
                //String S = "update Activities set ActName = '" + txtActivityName.Text.ToString().Trim()+ "' where ActNo = '"+ txtActivityNo.Text.ToString().Trim() +"' ";
                String S = "update Activities set ActName = '" + txtActivityName.Text.ToString().Trim() + "',UserActName='" + txtSystemName.Text.ToString().Trim() + "' where ActNo = '" + txtActivityNo.Text.ToString().Trim() + "' ";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                MessageBox.Show("The Activity Succesfully Added ", "Successfull", MessageBoxButtons.OK);
                btnNew.Enabled = true;     
            }
            else
            {
            try
            {
                if ((txtActivityNo.Text.ToString().Trim() != null) && (txtActivityName.Text.ToString().Trim() != null)  && (txtActivityNo.Text.ToString().Trim() != "") && (txtActivityName.Text.ToString().Trim() != "") )
                {
                    String S = "insert into Activities (ActNo,ActName,UserActName) values ('" + txtActivityNo.Text.ToString().Trim() + "','" + txtActivityName.Text.ToString().Trim() + "','" + txtSystemName.Text.ToString().Trim() + "')";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    MessageBox.Show("The Activity Succesfully Added", "Successfull", MessageBoxButtons.OK);
                    btnNew.Enabled = true;                   
                }
                else
                {
                    MessageBox.Show("Please Enter the Activity Name", "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtActivityName.Focus();
                }
            }
            catch
            { }
            }
            btnSave.Enabled = false;
            btnNew_Click(sender, e);
            btnNew.Enabled = true;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            getActNo();//get new activity no
            
            txtActivityNo.Enabled = true;
            txtActivityName.Enabled = true;
            btnSave.Enabled = true;
            btnNew.Enabled = false;           
            txtActivityName.Text = "";
            txtSystemName.Text = "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmActivities_Load(object sender, EventArgs e)
        {            
            txtActivityNo.Enabled = false;
            txtActivityName.Enabled = false;
            btnSave.Enabled = false;
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            frmViewActivities view = new frmViewActivities();
            view.ShowDialog();
        }
    }
}