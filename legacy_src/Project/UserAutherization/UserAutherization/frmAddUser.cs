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
    public partial class frmAddUser : Form
    {
        int update = 0;
        public static string ConnectionString;
        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
            //TextReader tr = new StreamReader("Connection.txt");
            //ConnectionString = tr.ReadLine();
            //tr.Close();
        }

        public frmAddUser()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (update == 0)
            {
                //check the username & password already exist or not               
                String S1 = "select UserID from Login";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

                if (dt1.Rows.Count == 0)
                {                   
                    String S = "insert into Login (UserID,PassWord,UserLevel) values ('" + txtUserName.Text.ToString().Trim() + "','" + txtPassWord.Text.ToString().Trim() + "','" + cmbUserLevel.Text.ToString().Trim() + "')";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    MessageBox.Show("User Details Successfully Added", "Successfull", MessageBoxButtons.OK,MessageBoxIcon.Information);                   
                }                    
                else
                {
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    if (txtUserName.Text.ToString().Trim() == dt1.Rows[i].ItemArray[0].ToString().Trim())
                    {
                        MessageBox.Show("User ID Already Exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtUserName.Clear();
                        txtUserName.Focus();
                    }
                    else
                    {
                        //save
                        if ((txtConfirmPassWord.Text.ToString().Trim() != "") && (txtPassWord.Text.ToString().Trim() != "") && (txtUserName.Text.ToString().Trim() != "") && (cmbUserLevel.Text.ToString().Trim() != ""))
                        {
                            if (txtConfirmPassWord.Text.ToString().Trim() == txtPassWord.Text.ToString().Trim())
                            {
                                try
                                {                                  
                                    String S = "insert into Login (UserID,PassWord,UserLevel) values ('" + txtUserName.Text.ToString().Trim() + "','" + txtPassWord.Text.ToString().Trim() + "','" + cmbUserLevel.Text.ToString().Trim() + "')";
                                    SqlCommand cmd = new SqlCommand(S);
                                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                                    DataTable dt = new DataTable();
                                    da.Fill(dt);
                                    MessageBox.Show("User Details Successfully Added", "Successfull", MessageBoxButtons.OK,MessageBoxIcon.Information);                                 

                                }
                                catch
                                { }
                                btnNew.Enabled = true;
                            }
                            else//if password Not valid
                            {
                                MessageBox.Show("Please Enter the Valid Password", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtConfirmPassWord.Text = "";
                                txtPassWord.Text = "";
                                txtPassWord.Focus();   
                            }
                        }
                    }
                }
            }
                if ((txtConfirmPassWord.Text.ToString().Trim() == "") || (txtPassWord.Text.ToString().Trim() == "") || (txtUserName.Text.ToString().Trim() == "") || (cmbUserLevel.Text.ToString().Trim() == ""))
                {
                    MessageBox.Show("Please Enter the Values For All Fields", "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtUserName.Focus();
                }
            }
            else
            {              
                String S2 = "update Login set PassWord = '" + txtPassWord.Text.ToString().Trim() + "',UserLevel = '" + cmbUserLevel.Text.ToString().Trim() + "' where UserID = '" + txtUserName.Text.ToString().Trim() + "'";
                SqlCommand cmd2 = new SqlCommand(S2);
                SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnectionString);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                MessageBox.Show(" User Details Successfully Updated ", "Successfull", MessageBoxButtons.OK,MessageBoxIcon.Information);            
 
            }
            btnNew.Enabled = true;
        }       

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmAddUser_Load(object sender, EventArgs e)
        {
            btnEdit.Enabled = false;
            txtUserName.Enabled = false;
            txtPassWord.Enabled = false;
            txtConfirmPassWord.Enabled = false;
            cmbUserLevel.Enabled = false;
            btnEdit.Enabled = false;
            btnSave.Enabled = false;

            //Load the user levels to Combobox 
            String S = "select UserLevel from UserLevel";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count ; i++)
            {
                cmbUserLevel.Items.Add(dt.Rows[i].ItemArray[0].ToString());
            }
        }       

        private void btnAll_Click(object sender, EventArgs e)
        {        
            frmViewUsers viewUser = new frmViewUsers();
            viewUser.ShowDialog();
            //viewUser.TopMost = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            String S = "select * from Login where UserID = '" + txtSearch.Text.ToString().Trim() + "'";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count==0)            
                {
                    MessageBox.Show("Please Enter the Valid User Name", "Invalid UserName", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtSearch.Text = "";
                    txtSearch.Focus();
                }
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (txtSearch.Text.ToString().Trim() == dt.Rows[i].ItemArray[0].ToString().Trim())
                {
                    if (txtSearch.Text.ToString().Trim() != "")
                    {
                        txtUserName.Enabled = false;
                        txtPassWord.Enabled = false;
                        txtConfirmPassWord.Enabled = false;
                        cmbUserLevel.Enabled = false;
                        btnNew.Enabled = false;

                        txtUserName.Text = dt.Rows[0].ItemArray[0].ToString().Trim();
                        txtPassWord.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                        txtConfirmPassWord.Text = dt.Rows[0].ItemArray[1].ToString().Trim();
                        cmbUserLevel.Text = dt.Rows[0].ItemArray[2].ToString().Trim();
                    }
                    btnEdit.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Please Enter the Valid User Name", "Invalid UserName", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtSearch.Focus();
                }
            }      
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            update = 1;
            txtUserName.Enabled = true;
            txtPassWord.Enabled = true;
            txtConfirmPassWord.Enabled = true;
            cmbUserLevel.Enabled = true;
            btnSave.Enabled = true;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {            
            txtUserName.Enabled = true;
            txtPassWord.Enabled = true;
            txtConfirmPassWord.Enabled = true;
            cmbUserLevel.Enabled = true;
            btnNew.Enabled = false;
            btnSave.Enabled = true;
            btnEdit.Enabled = false;

            txtUserName.Text = "";
            txtPassWord.Text = "";
            txtConfirmPassWord.Text = "";
            cmbUserLevel.Text = "1";
            txtSearch.Text = "";
            txtUserName.Focus();
        }

        private void btnAuthentication_Click(object sender, EventArgs e)
        {
            frmUserAuthentication authentication = new frmUserAuthentication();
            authentication.ShowDialog();
          //  authentication.TopMost = true;
            
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            frmCopyUserAuthentication copy = new frmCopyUserAuthentication();
            copy.ShowDialog();
            //copy.TopMost = true;
        }
       
    }
}