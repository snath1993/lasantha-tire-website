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
    public partial class Form1 : Form
    {
        public static string ConnectionString;

        public Form1()
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
            catch (Exception ex) { throw ex; }
        }

        private void btnBackupDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                string[] split = ConnectionString.Split(';');
                split = split[1].Split('=');
                string s = split[1];
                saveFileDialog1.Filter = ".bak|Backup Files";
                string fileName = s + "_" + DateTime.Now.ToShortDateString().Replace('/', '_') +".bak";
                saveFileDialog1.FileName = fileName;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        string sqlstr = String.Format("BACKUP DATABASE [" + s + "] TO DISK='{0}'", saveFileDialog1.FileName);

                        using (SqlCommand bu2 = new SqlCommand(sqlstr, conn))
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            conn.Open();
                            bu2.ExecuteNonQuery();
                            conn.Close();
                            Cursor.Current = Cursors.Default;
                            MessageBox.Show("Database Backup Successfully Completed");
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, Ex.Source);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }            
    }
}