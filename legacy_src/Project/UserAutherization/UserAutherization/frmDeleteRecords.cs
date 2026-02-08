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
    public partial class frmDeleteRecords : Form
    {
        public frmDeleteRecords()
        {
            InitializeComponent();
             setConnectionString();
        }

        public static string ConnectionString;
        public void setConnectionString()
        {
            clsDataAccess objclsDataAccess = new clsDataAccess();
            ConnectionString = objclsDataAccess.StrConectionStringLocal;
            //TextReader tr = new StreamReader("Connection.txt");
            //ConnectionString = tr.ReadLine();
            //tr.Close();
        }
        private void frmDeleteRecords_Load(object sender, EventArgs e)
        {
            try
            {
                LoadPO();
                LoadSO();
            }
            catch { }
        }

        public void LoadPO()
        {
            try
            {
                String S = "Select Distinct PONumber from tblPurchaseOrder order by PONumber";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                     cmbPO.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                }
            }
            catch { }
        }
        public void LoadSO()
        {
            try
            {
                String S = "Select Distinct SalesOrderNo from tblSalesOrderTemp order by SalesOrderNo";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    cmbSalesOrder.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
                   // cmbSalesOrder = dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim();
                }
            }
            catch { }

        }

        private void btnPoDelete_Click(object sender, EventArgs e)
        {
            DialogResult reply = MessageBox.Show("Do you want Delete this Record", "Are You Sure", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (reply == DialogResult.Yes)
            {
                if (cmbPO.Text == "")
                {
                    MessageBox.Show("Select a PO");
                }
                else
                {
                    String S = "delete  from tblPurchaseOrder where PONumber = '" + cmbPO.Text.ToString().Trim() + "' ";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                   // LoadPO();
                }

            }
            else if (reply == DialogResult.No)
            {
                btnPoDelete.Focus();
            }
        }

        private void btnDeleteSO_Click(object sender, EventArgs e)
        {
            DialogResult reply = MessageBox.Show("Do you want Delete this Record", "Are You Sure", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (reply == DialogResult.Yes)
            {
                if (cmbSalesOrder.Text== "")
                {
                    MessageBox.Show("Select a SO");
                }
                else
                {
                    String S = "delete  from tblSalesOrderTemp where SalesOrderNo = '" + cmbSalesOrder.Text.ToString().Trim() + "' ";
                    SqlCommand cmd = new SqlCommand(S);
                    SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                   // LoadSO();
                }

            }
            else if (reply == DialogResult.No)
            {
                btnPoDelete.Focus();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbPO_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}