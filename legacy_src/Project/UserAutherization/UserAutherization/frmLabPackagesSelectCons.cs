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
    public partial class frmLabPackagesSelectCons : Form
    {
        public static string ConnectionString;
        Class1 a = new Class1();
        
        public string StrFormType = string.Empty;
        public frmLabPackagesSelectCons(frmLabPackages frmParent)
        {
            InitializeComponent();
            
            StrFormType = frmParent.StrFormType;
            setConnectionString();
        }
        public void setConnectionString()
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
        private void frmLabPackagesSelectCons_Load(object sender, EventArgs e)
        {
            //string ConnString = ConnectionString;
            String S1 = "Select ConsultantCode, ConsultantName, ConsultantType from tblConsultantMaster where Block = 'False' and ConsultantType='" + StrFormType + "' ";
            SqlCommand cmd1 = new SqlCommand(S1);
            SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
            DataSet ds1 = new DataSet();
            da1.Fill(ds1);

            dgvConsultants.DataSource = ds1.Tables[0];
            //dgvConsultantRoom.Columns[0].Visible = false;
            dgvConsultants.Columns[0].HeaderText = "Consultant Code";
            dgvConsultants.Columns[0].Width = 123;
            dgvConsultants.Columns[1].HeaderText = "Consultant Name";
            dgvConsultants.Columns[1].Width = 200;
            dgvConsultants.Columns[2].HeaderText = "Consultant Type";
            dgvConsultants.Columns[2].Width = 200;

            for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
            {
                dgvConsultants.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                dgvConsultants.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                dgvConsultants.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DataSet ds1 = new DataSet();

            if (cmbType.Text.ToString().Trim() == "Name")
            {
                string ConnString = ConnectionString;
                String S1 = "Select ConsultantCode, ConsultantName, ConsultantType from tblConsultantMaster where ConsultantName LIKE '" + textBox1.Text.ToString().Trim() + "%' AND Block = 'False'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                da1.Fill(ds1);
            }

            if (cmbType.Text.ToString().Trim() == "Code")
            {
                string ConnString = ConnectionString;
                String S1 = "Select ConsultantCode, ConsultantName, ConsultantType from tblConsultantMaster where ConsultantCode LIKE '" + textBox1.Text.ToString().Trim() + "%' AND Block = 'False'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                da1.Fill(ds1);
            }

            if (cmbType.Text.ToString().Trim() == "Type")
            {
                string ConnString = ConnectionString;
                String S1 = "Select ConsultantCode, ConsultantName, ConsultantType from tblConsultantMaster where ConsultantType LIKE '" + textBox1.Text.ToString().Trim() + "%' AND Block = 'False'";
                SqlCommand cmd1 = new SqlCommand(S1);
                SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                da1.Fill(ds1);
            }

            dgvConsultants.DataSource = ds1.Tables[0];
            //dgvConsultantRoom.Columns[0].Visible = false;
            dgvConsultants.Columns[0].HeaderText = "Consultant Code";
            dgvConsultants.Columns[0].Width = 123;
            dgvConsultants.Columns[1].HeaderText = "Consultant Name";
            dgvConsultants.Columns[1].Width = 200;
            dgvConsultants.Columns[2].HeaderText = "Consultant Type";
            dgvConsultants.Columns[2].Width = 200;

            for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
            {
                dgvConsultants.Rows[i].Cells[0].Value = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                dgvConsultants.Rows[i].Cells[1].Value = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                dgvConsultants.Rows[i].Cells[2].Value = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
            }
        }

        private void dgvConsultants_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string val = dgvConsultants[1, dgvConsultants.CurrentRow.Index].Value.ToString().Trim();
            Class1.myvalue = a.GetNext(val);
            this.Close();
        }
    }
}
