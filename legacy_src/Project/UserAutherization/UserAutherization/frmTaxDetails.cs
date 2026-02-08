using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using DataAccess;
using System.Xml;
using System.Collections;
using System.Threading;
using Interop.PeachwServer;


namespace UserAutherization
{
    public partial class frmTaxDetails : Form
    {
        public frmTaxDetails()
        {
            InitializeComponent();
            setConnectionString();
        }

        public static string ConnectionString;
        public void setConnectionString()
        {
            try
            {
                //TextReader tr = new StreamReader("Connection.txt");
                //ConnectionString = tr.ReadLine();
                //tr.Close();
                clsDataAccess objclsDataAccess = new clsDataAccess();
                ConnectionString = objclsDataAccess.StrConectionStringLocal;
            }
            catch { }
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        public int Flag = 0;
      //  public int checkint = 0;
        private void button1_Click(object sender, EventArgs e)
        {
           int  checkint = 0;
            try
            {
                Convert.ToDouble(txtTaxRate.Text);

            }
            catch { checkint = 1; }


            if (txtTaxCode.Text == "" || txtTaxDescription.Text == "" || txtTaxRate.Text == "" || txttaxGLAccount.Text == "" || checkint==1)
            {
                if (checkint == 1)
                {
                    MessageBox.Show("Tax Rate must be a number");
                    btnSave.Focus();
                }
                else
                {
                    MessageBox.Show("Enter all Detals");
                    btnSave.Focus();
                }

            }
            else
            {
                try
                {
                    if (Flag == 0)
                    {

                        string ConnString = ConnectionString;
                        String S2 = "insert into tblTax(TaxCode,TaxName,TaxRate,GLAccount) values ('" + txtTaxCode.Text.ToString().Trim() + "','" + txtTaxDescription.Text.ToString().Trim() + "','" + Convert.ToDouble(txtTaxRate.Text) + "', '" + txttaxGLAccount.Text.ToString().Trim() + "')";
                        SqlCommand cmd2 = new SqlCommand(S2);
                        SqlDataAdapter da2 = new SqlDataAdapter(S2, ConnString);
                        DataSet ds2 = new DataSet();
                        da2.Fill(ds2);

                        MessageBox.Show("Saved Successfully");
                        txtTaxCode.Text ="";
                        txtTaxDescription.Text = "";
                        txtTaxRate.Text = "";
                        txttaxGLAccount.Text = "";
                        btnSave.Enabled = false;
                        btnNew.Focus();
                       
                    }
                    else
                    {

                        String S = "Update tblTax SET TaxName = '" + txtTaxDescription.Text.ToString().Trim() + "', TaxRate = '" + Convert.ToDouble(txtTaxRate.Text) + "', GLAccount  = '" + txttaxGLAccount.Text.ToString().Trim() + "' where TaxCode = '" + txtTaxCode.Text.ToString().Trim() + "'";
                        SqlCommand cmd = new SqlCommand(S);
                        SqlConnection con = new SqlConnection(ConnectionString);
                        SqlDataAdapter da = new SqlDataAdapter(S, con);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        MessageBox.Show("Updated Successfully");
                        txtTaxCode.Text = "";
                        txtTaxDescription.Text = "";
                        txtTaxRate.Text = "";
                        txttaxGLAccount.Text = "";
                        btnSave.Enabled = false;
                        cmbtaxCode.Focus();
                    }
                }
                catch { }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            txtTaxDescription.Enabled = true;
            txtTaxRate.Enabled = true;
            txttaxGLAccount.Enabled = true;
            Flag = 1;
            btnSave.Enabled = true;
        }

        private void frmTaxDetails_Load(object sender, EventArgs e)
        {
            TaxcodeLoad();
        }

        public void TaxcodeLoad()
        {
            String S = "Select TaxCode from tblTax";
            SqlCommand cmd = new SqlCommand(S);
            SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
            DataSet dt = new DataSet();
            da.Fill(dt);

            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                cmbtaxCode.Items.Add(dt.Tables[0].Rows[i].ItemArray[0].ToString().Trim());
            }

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtTaxCode.Enabled = true;
            txtTaxDescription.Enabled = true;
            txtTaxRate.Enabled = true;
            txttaxGLAccount.Enabled = true;

            //txtTaxCode.ReadOnly = false;
            //txtTaxDescription.ReadOnly = false;
            //txtTaxRate.ReadOnly = false;
            //txttaxGLAccount.ReadOnly = false;

            txtTaxCode.Text = "";
            txtTaxDescription.Text = "";
            txtTaxRate.Text = "";
            txttaxGLAccount.Text = "";

            btnSave.Enabled = true;
            btnClose.Enabled = true;

            cmbtaxCode.Enabled = false;
           // txtSearch.Enabled = false;
           // txtSearch.Text = "";
           
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbtaxCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                String S = "Select * from tblTax where TaxCode = '" + cmbtaxCode.Text.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(S);
                SqlDataAdapter da = new SqlDataAdapter(S, ConnectionString);
                DataSet dt = new DataSet();
                da.Fill(dt);

                if (dt.Tables[0].Rows.Count > 0)
                {

                    txtTaxCode.Text = dt.Tables[0].Rows[0].ItemArray[0].ToString();
                    txtTaxDescription.Text = dt.Tables[0].Rows[0].ItemArray[1].ToString();
                    txtTaxRate.Text = dt.Tables[0].Rows[0].ItemArray[2].ToString();
                    txttaxGLAccount.Text = dt.Tables[0].Rows[0].ItemArray[3].ToString();
                }
            }
            catch { }
            btnNew.Enabled = false;
            btnEdit.Enabled = true;
            btnSave.Enabled = false;
            btnDelete.Enabled = true;
            btnClose.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult reply = MessageBox.Show("Do you want Delete this Record", "Are You Sure", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (reply == DialogResult.Yes)
                {
                    String S1 = "delete from tblTax where TaxCode = '" + txtTaxCode.Text.ToString().Trim() + "'";
                    SqlCommand cmd1 = new SqlCommand(S1);
                    SqlDataAdapter da1 = new SqlDataAdapter(S1, ConnectionString);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);
                    MessageBox.Show("Selected Tax deleted");

                }
                else if (reply == DialogResult.No)
                {
                    btnDelete.Focus();
                }
            }
            catch { }
           // cmbtaxCode.Refresh();
            txtTaxCode.Text = "";
            txtTaxDescription.Text = "";
            txtTaxRate.Text = "";
            txttaxGLAccount.Text = "";
            btnEdit.Enabled = false;
            cmbtaxCode.Text = "";
            cmbtaxCode.Items.Clear();
            TaxcodeLoad();
        }
    }
}