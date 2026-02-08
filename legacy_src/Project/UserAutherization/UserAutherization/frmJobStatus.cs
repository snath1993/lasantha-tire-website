using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DataAccess;
using Interop.PeachwServer;

namespace UserAutherization
{
    public partial class frmJobStatus : Form
    {
        public DataSet dsJob;
        string StrSql;
        public static string ConnectionString;

        public frmJobStatus()
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
            catch { }
        }

        private void frmJobStatus_Load(object sender, EventArgs e)
        {
            setConnectionString();
            FillClient();


        }

        public void FillClient()
        {

            dsJob = new DataSet();
            try
            {
                cmbJob.Text = "";
                dsJob.Clear();
                StrSql = "SELECT JobID,JobDescription FROM tblJobMaster where Job_Status=2 order by JobID";
              
                SqlCommand cmd = new SqlCommand(StrSql);
                SqlDataAdapter da = new SqlDataAdapter(StrSql, ConnectionString);

                da.Fill(dsJob, "dsJob");

                cmbJob.DataSource = dsJob.Tables["dsJob"];
                cmbJob.DisplayMember = "JobID";
                cmbJob.ValueMember = "JobID";

                cmbJob.DisplayLayout.Bands[0].Columns["JobID"].Width = 100;
                cmbJob.DisplayLayout.Bands[0].Columns["JobDescription"].Width = 200;



            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            setConnectionString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            SqlTransaction myTrans = null;

             try

                {

                if (cmbJob.Text == "")
                {
                    return;
                }

                DialogResult reply = MessageBox.Show("Are you sure, you want to Finished this Project?", "Information", MessageBoxButtons.YesNo );

                if (reply == DialogResult.No)
                {
                    return;
                }

                myConnection.Open();
                myTrans = myConnection.BeginTransaction();

                StrSql = "SELECT JobID FROM tblJobMaster WHERE Job_Status=2 and JobID='" + cmbJob.Value + "'";

                SqlCommand command = new SqlCommand(StrSql, myConnection, myTrans);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    StrSql = "UPDATE tblJobMaster SET Job_Status=3 WHERE JobID='" + cmbJob.Value + "'";
                    command = new SqlCommand(StrSql, myConnection, myTrans);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                    MessageBox.Show("Successfully Updated.", "Information", MessageBoxButtons.OK);
                }
                else
                {

                    MessageBox.Show("No Data To Update.", "Information", MessageBoxButtons.OK);
                }

                myTrans.Commit();
                FillClient(); 

            }

            catch (Exception ex)
            {
                myTrans.Rollback();
                MessageBox.Show(ex.Message);
                throw;

            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}