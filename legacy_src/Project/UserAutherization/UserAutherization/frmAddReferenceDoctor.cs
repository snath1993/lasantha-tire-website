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
    public partial class frmAddReferenceDoctor : Form
    {
        public static string ConnectionString;
        public frmAddReferenceDoctor()
        {
            InitializeComponent();
            setConnectionString();
        }

        private void setConnectionString()
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ClearData();
            cmbDoctType.Enabled = true;
            txtDoctName.Enabled = true;
        }
        private void ClearData()
        {
            cmbDoctType.Text = "";
            txtDoctName.Text = "";
        }
        private string StrSql;
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (cmbDoctType.Text == "")
            {
                MessageBox.Show("Please Select Type", "Add Reference Doctor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtDoctName.Text == "")
            {
                MessageBox.Show("Please Enter Doctor's Name", "Add Reference Doctor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SaveEvent();
            cmbDoctType.Enabled = false;
            txtDoctName.Enabled = false;
        }

        private void SaveEvent()
        {
            try
            {
                StrSql = "insert into tblDoctorMaster(Type,Name) values ('" + cmbDoctType.Value.ToString().Trim() + "','" + txtDoctName.Text.ToString().Trim() + "')";
               // StrSql = "insert into tblReferenceDoctor(Type,Name) values ('" + cmbDoctType.Value.ToString().Trim() + "','" + txtDoctName.Text.ToString().Trim() + "')";
                SqlConnection con = new SqlConnection(ConnectionString);
                SqlCommand command = new SqlCommand(StrSql, con);
                con.Open();
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                MessageBox.Show("You Add new Reference Doctor Successfuly.", "Information", MessageBoxButtons.OK);
                ClearData();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmAddReferenceDoctor_Load(object sender, EventArgs e)
        {
            DataSet dsCustomer = new DataSet();
            try
            {
                dsCustomer.Clear();
                //StrSql = "SELECT distinct([ItemType]) as[ItemType] FROM[HospitalSystem].[dbo].[tblConsultantItemFee]";
                StrSql = "SELECT [TypeName]FROM [tblPatientMasterType]";
                SqlDataAdapter dAdapt = new SqlDataAdapter(StrSql, ConnectionString);
                dAdapt.Fill(dsCustomer, "DtClient");

                cmbDoctType.DataSource = dsCustomer.Tables["DtClient"];
                cmbDoctType.DisplayMember = "TypeName";
                cmbDoctType.ValueMember = "TypeName";

                cmbDoctType.DisplayLayout.Bands["DtClient"].Columns["TypeName"].Width = 100;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
