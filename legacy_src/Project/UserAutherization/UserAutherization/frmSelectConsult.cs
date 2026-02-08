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
    public partial class frmSelectConsult : Form
    {
        public static string ConnectionString;
        Class1 a = new Class1();
        int flgTech = 0;
        public string StrFormType = string.Empty;

       public frmSelectConsult(frmScan frmParent)
        {
            InitializeComponent();
            flgTech = frmParent.flgTech;
            StrFormType = frmParent.StrFormType;
            setConnectionString();
        }
        public frmSelectConsult(frmPriceUpdate frmParent)
        {
            InitializeComponent();
            flgTech = frmParent.flgTech;
           // StrFormType = frmParent.StrFormType;
            setConnectionString();
        }
       
        public frmSelectConsult(frmChanellopd frmParent)
        {
            InitializeComponent();
            flgTech = frmParent.flgTech;
            setConnectionString();
        }

        //frmChanellopd

        public frmSelectConsult(frmETU  frmParent)
        {
            InitializeComponent();
            flgTech = frmParent.flgTech;
            setConnectionString();
        }

        //.....................
        public frmSelectConsult(frmXray  frmParent)
        {
            InitializeComponent();
            flgTech = frmParent.flgTech;
            setConnectionString();
        }
        //.....................

        //..................
        public frmSelectConsult(frmLabTest  frmParent)
        {
            InitializeComponent();
            flgTech = frmParent.flgTech;
            setConnectionString();
        }

        public frmSelectConsult(frmLab frmParent)
        {
            InitializeComponent();
            flgTech = frmParent.flgTech;
            StrFormType = frmParent.StrFormType;
            setConnectionString();
        }
        //.....................

        public frmSelectConsult()
        {
            InitializeComponent();
            setConnectionString();
        }


        //Method to establish the connection
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


        private void frmSelectConsult_Load(object sender, EventArgs e)
        {
            if ((flgTech == 1) || (flgTech == 3))
            {
                string ConnString = ConnectionString;
                String S1 = "Select ConsultantCode, ConsultantName, ConsultantType from tblConsultantMaster where (Block = 'False') AND (ConsultantType = 'Technician')";
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
            else if (flgTech==2)
            {
                string type = DataAccess.Access.type;
                if (!(type == "WARD" || type == "OPD" || type == "ETU"))
                {
                    string ConnString = ConnectionString;
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
                else if (DataAccess.Access.type == "WARD" || DataAccess.Access.type == "OPD" || DataAccess.Access.type == "ETU")
                {
                    string ConnString = ConnectionString;
                    String S1 = "Select ConsultantCode, ConsultantName, ConsultantType from tblConsultantMaster where Block = 'False' ";
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
                }
            else
            {
                string ConnString = ConnectionString;
                String S1 = "Select ConsultantCode, ConsultantName, ConsultantType from tblConsultantMaster where Block = 'False'";
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

        private void dgvConsultantRoom_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (flgTech == 0)
            {
                string val = dgvConsultants[1, dgvConsultants.CurrentRow.Index].Value.ToString().Trim();
                Class1.myvalue = a.GetNext(val);
                this.Close();
            }
            if (flgTech == 1)
            { 
                string val = dgvConsultants[1, dgvConsultants.CurrentRow.Index].Value.ToString().Trim();
                Class1.myvalue = a.GetNext3(val);
                this.Close();
            }
            if (flgTech == 2)
            {
                string val = dgvConsultants[1, dgvConsultants.CurrentRow.Index].Value.ToString().Trim();
                Class1.myvalue = a.GetNext4(val);
                this.Close();
            }

            if (flgTech == 3)
            {
                string val = dgvConsultants[1, dgvConsultants.CurrentRow.Index].Value.ToString().Trim();
                Class1.myvalue = a.GetNext5(val);
                this.Close();
            }
            if (flgTech == 10)
            {
                string val = dgvConsultants[1, dgvConsultants.CurrentRow.Index].Value.ToString().Trim();
                string val1 = dgvConsultants[0, dgvConsultants.CurrentRow.Index].Value.ToString().Trim();
                Search.PriceUpdate = val;
                Class1.myvalue = a.GetNext11(val1);
                this.Close();
            }
        }

        private void dgvConsultantRoom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyValue == 13)
                {
                    if (flgTech == 0)
                    {
                        string val = dgvConsultants[1, dgvConsultants.CurrentRow.Index].Value.ToString().Trim();
                        Class1.myvalue = a.GetNext(val);
                        this.Close();
                    }
                     if (flgTech == 1)
                    {
                        string val = dgvConsultants[1, dgvConsultants.CurrentRow.Index].Value.ToString().Trim();
                        Class1.myvalue = a.GetNext3(val);
                        this.Close();
                    }
                    if (flgTech == 2)
                    {
                        string val = dgvConsultants[1, dgvConsultants.CurrentRow.Index].Value.ToString().Trim();
                        Class1.myvalue = a.GetNext4(val);
                        this.Close();
                    }

                    if (flgTech == 3)
                    {
                        string val = dgvConsultants[1, dgvConsultants.CurrentRow.Index].Value.ToString().Trim();
                        Class1.myvalue = a.GetNext5(val);
                        this.Close();
                    }
                }
            }
            catch { }
        }

        private void dudSelect_SelectedItemChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TabIndexChanged(object sender, EventArgs e)
        {
            int a = 0;
        }

        private void lblDate_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dgvConsultants_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}