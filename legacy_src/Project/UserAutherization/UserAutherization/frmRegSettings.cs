using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using System.IO;
using System.Data.Sql;

namespace UserAutherization
{
    public partial class frmRegSettings : Form
    {
        public static string CurrentUser;

        static string Server = "";
        static string DatabaseEvl = "";
        static string Database = "";
        static string UserID = "";
        static string pwd = "";
        static string connStringEvl = "";
        static string connString = "";

        DataAccess.clsCommon objclsCommon = new DataAccess.clsCommon();

        public frmRegSettings()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            frmAddUser au = new frmAddUser();
            au.ShowDialog();
        }

        private void GetDataFromTextFile()
        {
            try
            {
                TextReader tr = new StreamReader(Application.StartupPath + "\\config.txt");

                Server = objclsCommon.decryptPassword(tr.ReadLine());
                UserID = objclsCommon.decryptPassword(tr.ReadLine());
                pwd = objclsCommon.decryptPassword(tr.ReadLine());
                Database = objclsCommon.decryptPassword(tr.ReadLine());
                DatabaseEvl = objclsCommon.decryptPassword(tr.ReadLine());
                tr.Close();

                if (string.IsNullOrEmpty(Database))
                {
                    Database = UserID;
                    UserID = "";
                    DatabaseEvl = pwd;
                    pwd = "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            try
            {
                //panel1.Visible = false;

                txtUserID.Enabled = true;
                txtPWD.Enabled = true;

                GetDataFromTextFile();

                string myServer = Environment.MachineName;
                DataTable servers = SqlDataSourceEnumerator.Instance.GetDataSources();

                for (int i = 0; i < servers.Rows.Count; i++)
                {
                    myServer = servers.Rows[i]["ServerName"].ToString() + "\\" + servers.Rows[i]["InstanceName"].ToString(); ///// used to get the servers in the local machine////
                    cmbServer.Items.Add(myServer);
                }
                cmbServer.Text = "";
                cmbServer.SelectedText = Server;
                cmbDB.Text = "";
                cmbDB.SelectedText = Database;
                txtUserID.Text = UserID;
            }
            catch (Exception ex)
            {
                string myServer = Environment.MachineName;
                DataTable servers = SqlDataSourceEnumerator.Instance.GetDataSources();

                for (int i = 0; i < servers.Rows.Count; i++)
                {
                    myServer = servers.Rows[i]["ServerName"].ToString(); ///// used to get the servers in the local machine////
                    cmbServer.Items.Add(myServer);
                }
                cmbServer.Text = "";
                cmbServer.SelectedText = Server;
                cmbDB.Text = "";
                cmbDB.SelectedText = Database;
                txtUserID.Text = UserID;

            }

        }

        private void btnReg_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbServer.Text == "")
                {
                    MessageBox.Show("Specify a server");
                    return;
                }

                if (!rdoWindowsAuth.Checked && !rdoSQLAuthentication.Checked)
                {
                    MessageBox.Show("Select an authentication Mode");
                    return;
                }

                if (rdoWindowsAuth.Checked)
                {
                    MessageBox.Show("You must establish the connection with SQL authentication Mode");
                    return;
                }

                if (rdoSQLAuthentication.Checked)
                {
                    if (txtUserID.Text == "")
                    {
                        MessageBox.Show("Enter User ID");
                        return;
                    }
                    else
                    {
                        if (txtPWD.Text == "")
                        {
                            MessageBox.Show("Enter Password of User '" + txtUserID.Text.ToString().Trim() + "'");
                            return;
                        }
                    }

                }
                if (txtDB.Text == "")
                {
                    MessageBox.Show("You must specify DataBase Name");
                    return;
                }

                TextWriter tr = new StreamWriter("config.txt");
                tr.WriteLine(objclsCommon.encryptPassword(cmbServer.Text.Trim()));
                tr.WriteLine(objclsCommon.encryptPassword(txtUserID.Text.Trim()));
                tr.WriteLine(objclsCommon.encryptPassword(txtPWD.Text.Trim()));

                string TempDB = "test";
                if (cmbDB.Text.Trim() != "") TempDB = cmbDB.Text.Trim();
                tr.WriteLine(objclsCommon.encryptPassword(TempDB));
                tr.Close();
                MessageBox.Show("Create a Connection with the databse Name'" + cmbDB.Text.ToString().Trim() + "' is successfull");

                //cmbDB.Text = "";
                //txtUserID.Text = "";
                txtPWD.Text = "";
                //cmbServer.Text = "";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void cmbServer_SelectedIndexChanged(object sender, EventArgs e)
        {

            cmbDB.Items.Clear();
            txtUserID.Text = "sa";
            txtPWD.Text = "";

            if (rdoWindowsAuth.Checked)
            {
                try
                {
                    cmbDB.Items.Clear();
                    GetDataFromTextFile();

                    if (txtPWD.Text.Trim().Length > 0) pwd = txtPWD.Text.Trim();
                    if (txtUserID.Text.Trim().Length > 0) UserID = txtUserID.Text.Trim();

                    String connString = "";
                    string svrName = "";

                    if (cmbServer.SelectedItem == null) svrName = cmbServer.Text;
                    else svrName = cmbServer.SelectedItem.ToString();


                    if (Database == null)
                    {
                        Database = UserID;
                        DatabaseEvl = pwd;
                        connString = "Data Source=" + svrName + "; Integrated Security=True;";
                    }

                    if (rdoWindowsAuth.Checked)
                    {
                        Database = UserID;
                        DatabaseEvl = pwd;
                        connString = "Data Source=" + svrName + "; Integrated Security=True;";
                    }
                    if (rdoSQLAuthentication.Checked)
                    {
                        connString = "Data Source=" + svrName + ";Persist Security Info = True;User ID=" + UserID + ";Password=" + pwd + ";";
                    }
                    //if (Database == null)
                    //{
                    //    Database = UserID;
                    //    DatabaseEvl = pwd;
                    //    connString = "Data Source=" + svrName + "; Integrated Security=True;";
                    //}
                    //else
                    //    connString = "Data Source=" + svrName + ";Persist Security Info = True;User ID=" + UserID + ";Password=" + pwd + ";";

                    SqlConnection sqlConx = new SqlConnection(connString);
                    sqlConx.Open();
                    DataTable tblDatabases = sqlConx.GetSchema("Databases");
                    sqlConx.Close();

                    cmbDB.Text = "";
                    cmbDB.Items.Clear();
                    cmbDB.SelectedText = Database;

                    for (int i = 0; i < tblDatabases.Rows.Count; i++)
                    {
                        cmbDB.Items.Add(tblDatabases.Rows[i]["database_name"].ToString());
                    }



                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);

                }
            }
        }

        private void txtPWD_Leave(object sender, EventArgs e)
        {
            try
            {
                if (cmbServer.Text == "")
                {
                    MessageBox.Show("Specify a server");
                    return;
                }

                if (!rdoWindowsAuth.Checked && !rdoSQLAuthentication.Checked)
                {
                    MessageBox.Show("Select an authentication Mode");
                    return;
                }

                if (rdoSQLAuthentication.Checked)
                {
                    if (txtUserID.Text == "")
                    {
                        MessageBox.Show("Select User ID");
                        return;
                    }
                    else
                    {
                        if (txtPWD.Text == "")
                        {
                            MessageBox.Show("Select PWD");
                            return;
                        }
                    }

                }





                cmbDB.Items.Clear();
                GetDataFromTextFile();




                if (txtPWD.Text.Trim().Length > 0) pwd = txtPWD.Text.Trim();
                if (txtUserID.Text.Trim().Length > 0) UserID = txtUserID.Text.Trim();

                String connString = "";
                string svrName = "";

                if (cmbServer.SelectedItem == null) svrName = cmbServer.Text;
                else svrName = cmbServer.SelectedItem.ToString();


                if (Database == null)
                {
                    Database = UserID;
                    DatabaseEvl = pwd;
                    connString = "Data Source=" + svrName + "; Integrated Security=True;";
                }

                if (rdoWindowsAuth.Checked)
                {
                    Database = UserID;
                    DatabaseEvl = pwd;
                    connString = "Data Source=" + svrName + "; Integrated Security=True;";
                }
                if (rdoSQLAuthentication.Checked)
                {
                    connString = "Data Source=" + svrName + ";Persist Security Info = True;User ID=" + UserID + ";Password=" + pwd + ";";
                }
                //if (Database == null)
                //{
                //    Database = UserID;
                //    DatabaseEvl = pwd;
                //    connString = "Data Source=" + svrName + "; Integrated Security=True;";
                //}
                //else
                //    connString = "Data Source=" + svrName + ";Persist Security Info = True;User ID=" + UserID + ";Password=" + pwd + ";";

                SqlConnection sqlConx = new SqlConnection(connString);
                sqlConx.Open();
                DataTable tblDatabases = sqlConx.GetSchema("Databases");
                sqlConx.Close();

                cmbDB.Text = "";
                cmbDB.Items.Clear();
                cmbDB.SelectedText = Database;

                for (int i = 0; i < tblDatabases.Rows.Count; i++)
                {
                    cmbDB.Items.Add(tblDatabases.Rows[i]["database_name"].ToString());
                }



            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);

            }
        }

        private void lblUserName_Click(object sender, EventArgs e)
        {

        }

        private void lnkNewDB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                if (cmbServer.Text == "")
                {
                    MessageBox.Show("Specify a server");
                    return;
                }

                if (!rdoWindowsAuth.Checked && !rdoSQLAuthentication.Checked)
                {
                    MessageBox.Show("Select an authentication Mode");
                    return;
                }

                if (rdoSQLAuthentication.Checked)
                {
                    if (txtUserID.Text == "")
                    {
                        MessageBox.Show("Select User ID");
                        return;
                    }
                    else
                    {
                        if (txtPWD.Text == "")
                        {
                            MessageBox.Show("Select PWD");
                            return;
                        }
                    }

                }
                if (txtldfPath.Text == "")
                {
                    MessageBox.Show("Select .mdf and .ldf file paths");
                    return;
                                    }
                if (txtDB.Text == "")
                {
                    MessageBox.Show("You must specify DataBase Name");
                    return;
                }


                cmbDB.Items.Clear();
                GetDataFromTextFile();

                if (txtPWD.Text.Trim().Length > 0) pwd = txtPWD.Text.Trim();
                if (txtUserID.Text.Trim().Length > 0) UserID = txtUserID.Text.Trim();

                String connString = "";
                string svrName = "";

                if (cmbServer.SelectedItem == null) svrName = cmbServer.Text;
                else svrName = cmbServer.SelectedItem.ToString();


                if (Database == null)
                {
                    Database = UserID;
                    DatabaseEvl = pwd;
                    connString = "Data Source=" + svrName + "; Integrated Security=True;";
                }

                if (rdoWindowsAuth.Checked)
                {
                    Database = UserID;
                    DatabaseEvl = pwd;
                    connString = "Data Source=" + svrName + "; Integrated Security=True;";
                }
                if (rdoSQLAuthentication.Checked)
                {
                    connString = "Data Source=" + svrName + ";Persist Security Info = True;User ID=" + UserID + ";Password=" + pwd + ";";
                }







                //if (txtDB.Text == "")
                //{
                //    MessageBox.Show("You must specify DataBase Name");
                //    return;
                //}

                //GetDataFromTextFile();
                //connString = "Data Source=" + Server + ";Persist Security Info = True;User ID=" + UserID + ";Password=" + pwd + ";Connection timeout=50000;";
                //if (Server == "")
                //{
                //    MessageBox.Show("You must specify valid Servername Username and Password");
                //    return;
                //}
                SqlConnection conn = new SqlConnection(connString);
                //conn.Open();
                //SqlCommand cmd = conn.CreateCommand();
                //cmd.CommandType = CommandType.Text;
                //cmd.CommandText = "if not exists(select * from sys.databases where name = '" + txtDB.Text.Trim() + "') " +
                //    " create database " + txtDB.Text.Trim();
                //cmd.ExecuteNonQuery();
                //conn.Close();

                conn.Open();
                SqlCommand cmd1 = conn.CreateCommand();
                cmd1.CommandType = CommandType.Text;

                cmd1.CommandText = "if not exists(select * from sys.databases where name = '" + txtDB.Text.Trim() + "') " +
                                   " begin " +
                                   " RESTORE DATABASE " + txtDB.Text.Trim() +
                                   " FROM  DISK = '" + Application.StartupPath + "\\" + txtBU.Text.Trim() +
                                   ".bak' WITH  FILE = 1,  MOVE N'PerfectDistribution' TO N" +
                                   "'" + txtmdfPath.Text.ToString().Trim() + ".mdf',  " +
                                   " MOVE N'PerfectDistribution_log' TO N'" + txtldfPath.Text.ToString().Trim() + ".ldf',NOUNLOAD,  STATS = 10 end";

                // ;+
                // Environment.NewLine +
                //"GO";


                //cmd1.CommandText = "if exists(select * from sys.databases where name = '" + txtDB.Text.Trim() + "') " +
                //    " begin " +
                //    " RESTORE DATABASE " + txtDB.Text.Trim() +
                //    " FROM DISK = '" + Application.StartupPath + "\\" + txtBU.Text.Trim() + ".bak' " +
                //    " WITH REPLACE end";



                cmd1.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Create a Sample DB Successfully");
                txtPWD_Leave(sender, e);

                //txtBU.Text = "";
                //txtDB.Text = "";
            }
            catch(Exception ex)
            {
               // MessageBox.Show("The Database alrwady exsists in the System");
                 MessageBox.Show(ex.Message);
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtBU_TextChanged(object sender, EventArgs e)
        {

        }

        private void rdoWindowsAuth_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoWindowsAuth.Checked)
            {
                txtUserID.Enabled = false;
                txtPWD.Enabled = false;

                try
                {
                    cmbDB.Items.Clear();
                    GetDataFromTextFile();

                    if (txtPWD.Text.Trim().Length > 0) pwd = txtPWD.Text.Trim();
                    if (txtUserID.Text.Trim().Length > 0) UserID = txtUserID.Text.Trim();

                    String connString = "";
                    string svrName = "";

                    if (cmbServer.SelectedItem == null) svrName = cmbServer.Text;
                    else svrName = cmbServer.SelectedItem.ToString();


                    if (Database == null)
                    {
                        Database = UserID;
                        DatabaseEvl = pwd;
                        connString = "Data Source=" + svrName + "; Integrated Security=True;";
                    }

                    if (rdoWindowsAuth.Checked)
                    {
                        Database = UserID;
                        DatabaseEvl = pwd;
                        connString = "Data Source=" + svrName + "; Integrated Security=True;";
                    }
                    if (rdoSQLAuthentication.Checked)
                    {
                        connString = "Data Source=" + svrName + ";Persist Security Info = True;User ID=" + UserID + ";Password=" + pwd + ";";
                    }
                    //if (Database == null)
                    //{
                    //    Database = UserID;
                    //    DatabaseEvl = pwd;
                    //    connString = "Data Source=" + svrName + "; Integrated Security=True;";
                    //}
                    //else
                    //    connString = "Data Source=" + svrName + ";Persist Security Info = True;User ID=" + UserID + ";Password=" + pwd + ";";

                    SqlConnection sqlConx = new SqlConnection(connString);
                    sqlConx.Open();
                    DataTable tblDatabases = sqlConx.GetSchema("Databases");
                    sqlConx.Close();

                    cmbDB.Text = "";
                    cmbDB.Items.Clear();
                    cmbDB.SelectedText = Database;

                    for (int i = 0; i < tblDatabases.Rows.Count; i++)
                    {
                        cmbDB.Items.Add(tblDatabases.Rows[i]["database_name"].ToString());
                    }



                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);

                }
            }
        }

        private void rdoSQLAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoSQLAuthentication.Checked)
            {
                txtUserID.Enabled = true;
                txtUserID.Text = "sa";
                txtPWD.Text = "";
                txtPWD.Enabled = true;
            }
        }

        private void cmbServer_Leave(object sender, EventArgs e)
        {
            try
            {
                cmbDB.Items.Clear();
                GetDataFromTextFile();

                if (txtPWD.Text.Trim().Length > 0) pwd = txtPWD.Text.Trim();
                if (txtUserID.Text.Trim().Length > 0) UserID = txtUserID.Text.Trim();

                String connString = "";
                string svrName = "";

                if (cmbServer.SelectedItem == null) svrName = cmbServer.Text;
                else svrName = cmbServer.SelectedItem.ToString();


                if (Database == null)
                {
                    Database = UserID;
                    DatabaseEvl = pwd;
                    connString = "Data Source=" + svrName + "; Integrated Security=True;";
                }

                if (rdoWindowsAuth.Checked)
                {
                    Database = UserID;
                    DatabaseEvl = pwd;
                    connString = "Data Source=" + svrName + "; Integrated Security=True;";
                }
                if (rdoSQLAuthentication.Checked)
                {
                    connString = "Data Source=" + svrName + ";Persist Security Info = True;User ID=" + UserID + ";Password=" + pwd + ";";
                }
                //if (Database == null)
                //{
                //    Database = UserID;
                //    DatabaseEvl = pwd;
                //    connString = "Data Source=" + svrName + "; Integrated Security=True;";
                //}
                //else
                //    connString = "Data Source=" + svrName + ";Persist Security Info = True;User ID=" + UserID + ";Password=" + pwd + ";";

                SqlConnection sqlConx = new SqlConnection(connString);
                sqlConx.Open();
                DataTable tblDatabases = sqlConx.GetSchema("Databases");
                sqlConx.Close();

                cmbDB.Text = "";
                cmbDB.Items.Clear();
                cmbDB.SelectedText = Database;

                for (int i = 0; i < tblDatabases.Rows.Count; i++)
                {
                    cmbDB.Items.Add(tblDatabases.Rows[i]["database_name"].ToString());
                }



            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);

            }
        }

        private void btnmdfbrows_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                //txtmdfPath.Text = OFD.
                txtmdfPath.Text = folderBrowserDialog1.SelectedPath.ToString() + "\\" + txtDB.Text.ToString().Trim();
                txtldfPath.Text = folderBrowserDialog1.SelectedPath.ToString() + "\\" + txtDB.Text.ToString().Trim();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtldfPath.Text = folderBrowserDialog1.SelectedPath.ToString().Trim();
            }
        }

        private void btnfinished_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}