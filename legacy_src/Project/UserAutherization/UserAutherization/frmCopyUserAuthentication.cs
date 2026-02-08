using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Collections;


namespace UserAutherization
{
    public partial class frmCopyUserAuthentication : Form
    {
        public static string ConnectionString;
        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
            //TextReader tr = new StreamReader("Connection.txt");
            //ConnectionString = tr.ReadLine();
            //tr.Close();
        }
        public frmCopyUserAuthentication()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void frmCopyUserAuthentication_Load(object sender, EventArgs e)
            //load the User Id's to the Combo boxes
        {          
            String S = "Select UserID from Login";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);

            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                cmbFrom.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString());
                cmbTo.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString());
            }   

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            String S = "select * from UserAuthentication where UserName = '" + cmbFrom.Text.ToString().Trim() + "'";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);

            String S5 = "select * from UserAuthentication where UserName = '" + cmbTo.Text.ToString().Trim() + "'";
            SqlCommand cmd5 = new SqlCommand(S5);
            SqlDataAdapter da5= new SqlDataAdapter(S5, ConnectionString);
            DataSet dt5 = new DataSet();
            da5.Fill(dt5);

            if (dt5.Tables[0].Rows.Count <= 0)
            {
                if (cmbFrom.Text != "" && cmbTo.Text != "")
                {
                    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                    {
                        String S1 = "Insert into UserAuthentication (UserName,ActivityName,CanAct,CanRun,CanAdd,CanEdit,CanDelete) Values ('" + cmbTo.Text.ToString().Trim() + "','" + dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim() + "','" + dt.Tables[0].Rows[i].ItemArray[6].ToString().Trim() + "','" + dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim() + "','" + dt.Tables[0].Rows[i].ItemArray[3].ToString().Trim() + "','" + dt.Tables[0].Rows[i].ItemArray[4].ToString().Trim() + "','" + dt.Tables[0].Rows[i].ItemArray[5].ToString().Trim() + "')";
                        SqlCommand cmd1 = new SqlCommand(S1);
                        SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                        DataTable dt1 = new DataTable();
                        da1.Fill(dt1);
                    }
                    MessageBox.Show("Copied Authentications '"+cmbFrom.Text.ToString().Trim()+"' To '"+cmbTo.Text.ToString().Trim()+ "'", "Successfull", MessageBoxButtons.OK);
                }
                else
                {

                    if (cmbFrom.Text != "")
                    {
                        MessageBox.Show("Please Select the To User", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cmbTo.Focus();
                    }
                    if (cmbTo.Text != "")
                    {
                        MessageBox.Show("Please Select the From User", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cmbFrom.Focus();
                    }
                }
                if (cmbFrom.Text == "" && cmbTo.Text == "")
                {
                    MessageBox.Show("Please Select the From User and To User", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cmbFrom.Focus();
                }
            }
            else//if (dt5.Tables[0].Rows.Count > 0)
            {
                String S9 = " delete  from UserAuthentication where UserName = '" + cmbTo.Text.ToString().Trim() + "' ";
                SqlCommand cmd9 = new SqlCommand(S9);
                SqlDataAdapter da9 = new SqlDataAdapter(S9, ConnectionString);
                DataTable dt9 = new DataTable();
                da9.Fill(dt9);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {

                    String S1 = "Insert into UserAuthentication (UserName,ActivityName,CanAct,CanRun,CanAdd,CanEdit,CanDelete) Values ('" + cmbTo.Text.ToString().Trim() + "','" + dt.Tables[0].Rows[i].ItemArray[1].ToString().Trim() + "','" + dt.Tables[0].Rows[i].ItemArray[6].ToString().Trim() + "','" + dt.Tables[0].Rows[i].ItemArray[2].ToString().Trim() + "','" + dt.Tables[0].Rows[i].ItemArray[3].ToString().Trim() + "','" + dt.Tables[0].Rows[i].ItemArray[4].ToString().Trim() + "','" + dt.Tables[0].Rows[i].ItemArray[5].ToString().Trim() + "')";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);
                }
                MessageBox.Show("Copied Authentications '"+cmbFrom.Text.ToString().Trim()+"' To '"+cmbTo.Text.ToString().Trim()+"'", "Successfull", MessageBoxButtons.OK);
 
            }            
        }      

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}