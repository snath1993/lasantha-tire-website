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
    public partial class frmChannellingConsultantList : Form
    {
        public static string ConnectionString;
        public string StrFormType = string.Empty;
        public frmChannellingConsultantList(frmChannellig frmParent)
        {
            InitializeComponent();
            setConnectionString();
            StrFormType = frmParent.StrFormType;
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
        private void frmChannellingConsultantList_Load(object sender, EventArgs e)
        {
            try {
                cmbType.SelectedIndex = 1;
                string ConnString = ConnectionString;
                String S1 = "Select ItemID,Consultant, ItemType from tblConsultantItemFee where (ItemType = 'CHANNELLING') ";
                SqlDataAdapter da = new SqlDataAdapter(S1, ConnectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
               

                dgvConsultants.DataSource = dt;
                //dgvConsultantRoom.Columns[0].Visible = false;
                dgvConsultants.Columns[0].HeaderText = "Consultant Code";
                dgvConsultants.Columns[0].Width = 123;
                dgvConsultants.Columns[1].HeaderText = "Consultant Name";
                dgvConsultants.Columns[1].Width = 200;
                dgvConsultants.Columns[2].HeaderText = "Consultant Type";
                dgvConsultants.Columns[2].Width = 200;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dgvConsultants.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString();
                    dgvConsultants.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString();
                    dgvConsultants.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
      
        private void textBox1_TextChanged(object sender, EventArgs e)
        {try
            {



                String StrCode = null;

                if (cmbType.Text.Trim() == "")
                {
                    return;
                }

                switch (cmbType.SelectedIndex)
                {
                    case 0:
                        {
                            StrCode = "ItemID";

                            break;
                        }
                    case 1:
                        {
                            StrCode = "Consultant";
                            break;
                        }


                }
                if (cmbType.Text.Trim() != "")
                {

                    string ConnString = ConnectionString;
                    String S1 = "Select ItemID,Consultant, ItemType from tblConsultantItemFee where (ItemType = 'CHANNELLING')  AND (" + StrCode + " Like '%" + txtsearch.Text + "%')";
                                   
                    SqlDataAdapter da = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                   

                    dgvConsultants.DataSource = dt;
                    //dgvConsultantRoom.Columns[0].Visible = false;
                    dgvConsultants.Columns[0].HeaderText = "Consultant Code";
                    dgvConsultants.Columns[0].Width = 123;
                    dgvConsultants.Columns[1].HeaderText = "Consultant Name";
                    dgvConsultants.Columns[1].Width = 200;
                    dgvConsultants.Columns[2].HeaderText = "Consultant Type";
                    dgvConsultants.Columns[2].Width = 200;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgvConsultants.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString();
                        dgvConsultants.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString();
                        dgvConsultants.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtsearch.Text ="";
            frmChannellingConsultantList_Load(sender,e);

        }
        Class1 a = new Class1();
        private void dgvConsultants_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string val = dgvConsultants[0, dgvConsultants.CurrentRow.Index].Value.ToString().Trim();
            string val2 = dgvConsultants[1, dgvConsultants.CurrentRow.Index].Value.ToString().Trim();
            string val1 = dgvConsultants[2, dgvConsultants.CurrentRow.Index].Value.ToString().Trim();
            Class1.myvalue = a.GetNext6(val, val1, val2);
            this.Close();
        }
    }
}
